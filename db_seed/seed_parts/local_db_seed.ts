import { Utf8Stream } from "fs"
import { DatabaseSync } from "node:sqlite"
import { createProgressLogger } from "./progress_logger.ts"
const database = new DatabaseSync("database.db")

// const USERCOUNT = 1000
// const POSTCOUNT = 5000
// const LIKECOUNT = 100000

const USERCOUNT = 10
const POSTCOUNT = 50
const LIKECOUNT = 100

const POST_SKEW = 0.75
const LIKE_USER_SKEW = 0.75
const LIKE_POST_SKEW = 1

type RandomUserResponse = {
  results: {
    email: string,
    login: {
      username: string
    }
    }[]
}

type PostResponse = {
  posts: {
    title: string,
    body: string
    }[]
  }

export function resetDatabase(): void {
  database.exec(`
    DROP TABLE IF EXISTS likes;
    DROP TABLE IF EXISTS posts;
    DROP TABLE IF EXISTS users;
  `)
}

export function initDatabase() {
  database.exec(`
    CREATE TABLE IF NOT EXISTS users(
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      social_userid TEXT,
      email TEXT NOT NULL,
      password TEXT NOT NULL,
      accessToken TEXT,
      CONSTRAINT unique_email UNIQUE (email)
    )
    `)

  database.exec(`
    CREATE TABLE IF NOT EXISTS posts(
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      userId INTEGER NOT NULL,
      content TEXT NOT NULL,
      social_id TEXT,
      CONSTRAINT fk_post_user FOREIGN KEY (userId) REFERENCES users(id)
    )
    `)

  database.exec(`
    CREATE TABLE IF NOT EXISTS likes(
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      userId INTEGER NOT NULL,
      postId INTEGER NOT NULL,
      CONSTRAINT fk_user FOREIGN KEY (userId) REFERENCES users(id),
      CONSTRAINT fk_post FOREIGN KEY (postId) REFERENCES posts(id),
      CONSTRAINT one_like_per_post UNIQUE (userId, postId)
    )`)
}

function countRows(table: "users" | "posts" | "likes"): number {
  // Table names can't be bound as parameters, so the union type restricts the input
  const row = database
    .prepare(`SELECT COUNT(*) AS count FROM ${table}`)
    .get() as { count: number }
  return row.count
}

// AI GENERATED
function createWeightedPicker(ids: number[], skew: number): () => number {
  const cumulative: number[] = []
  let total = 0
  for (let rank = 0; rank < ids.length; rank++) {
    total += 1 / (rank + 1) ** skew
    cumulative.push(total)
  }

  return () => {
    const target = Math.random() * total
    let low = 0
    let high = cumulative.length - 1
    while (low < high) {
      const mid = (low + high) >> 1
      if (cumulative[mid] < target) low = mid + 1
      else high = mid
    }
    return ids[low]
  }
}

export async function seedUsers(userCount: number) {
  console.log(countRows("users"))
  let remaining = userCount - countRows("users")
  if (remaining <= 0) {
    console.log(`users: already has ${countRows("users")} users`)
    return
  }

  const insertUser = database.prepare(
    "INSERT OR IGNORE INTO users (email, password) VALUES (?, ?)",
  )

  const logProgress = createProgressLogger("users", remaining)
  logProgress(countRows("users"))


  while (remaining > 0) {
    const batchSize = Math.min(remaining, 1000)
    const response = await fetch(`https://randomuser.me/api/?results=${batchSize}`);

    const data = (await response.json()) as RandomUserResponse
    remaining -= batchSize

    database.exec("BEGIN")
    for (const user of data.results) {
      insertUser.run(user.email, user.login.username) // We use username as the password
    }
    database.exec("COMMIT")
    logProgress(countRows("users"))
  }
}


export async function seedPosts(postCount: number) {
  let remaining = postCount - countRows("posts")
  if (remaining <= 0) {
    console.log(`posts: already has ${countRows("posts")} posts`)
    return
  }

  const userIds = (
      database.prepare("SELECT id FROM users ORDER BY id").all() as { id: number }[]
    ).map((row) => row.id)
    if (userIds.length === 0) {
      throw new Error("Seed users before posts: the users table is empty")
    }

  const pickUserId = createWeightedPicker(userIds, POST_SKEW)

  const insertPost = database.prepare("INSERT INTO posts (userId, content) VALUES (?, ?)")
  const logProgress = createProgressLogger("posts", postCount)
  logProgress(countRows("posts"))

  const WORDS_PER_POST = 25

  while (remaining > 0) {
    const batchSize = Math.min(remaining, 4000)
    const response = await fetch(
      `https://fakeapi.net/lorem/${batchSize * WORDS_PER_POST}`,
    )
    const { text } = (await response.json()) as { text: string }
    const words = text.split(/\s+/).filter(Boolean)

    // Guard against the API returning fewer words than requested
    const postCount = Math.min(batchSize, Math.floor(words.length / WORDS_PER_POST))
    if (postCount === 0) throw new Error("API returned too few words")

    database.exec("BEGIN")
    for (let i = 0; i < postCount; i++) {
      const body = words
        .slice(i * WORDS_PER_POST, (i + 1) * WORDS_PER_POST)
        .join(" ")
      insertPost.run(pickUserId(), body)
    }
    database.exec("COMMIT")

    remaining -= postCount
    logProgress(countRows("posts"))
  }

  console.log(`posts: now has ${countRows("posts")} posts`)
}

export async function seedLikes(likesCount: number) {
  const remaining = likesCount - countRows("likes")
  if (remaining <= 0) {
    console.log(`likes: already has ${countRows("likes")}, nothing to seed`)
    return
  }

  // Users are ranked in REVERSE id order here (posts use ascending), so the
  // most active likers aren't the same people who write the most posts.
  const userIds = (
    database.prepare("SELECT id FROM users ORDER BY id DESC").all() as {
      id: number
    }[]
  ).map((row) => row.id)
  const postIds = (
    database.prepare("SELECT id FROM posts ORDER BY id").all() as {
      id: number
    }[]
  ).map((row) => row.id)

  if (userIds.length === 0 || postIds.length === 0) {
    throw new Error("Seed users and posts before likes")
  }
  if (likesCount > userIds.length * postIds.length) {
    throw new Error(
      `Can't fit ${likesCount} unique likes: only ${userIds.length * postIds.length} user/post pairs exist`,
    )
  }

  const pickUserId = createWeightedPicker(userIds, LIKE_USER_SKEW)
  const pickPostId = createWeightedPicker(postIds, LIKE_POST_SKEW)

  // OR IGNORE skips duplicate (user, post) pairs; we keep drawing until we
  // have inserted enough new ones.
  const insertLike = database.prepare(
    "INSERT OR IGNORE INTO likes (userId, postId) VALUES (?, ?)",
  )

  let inserted = 0
  let attempts = 0
  const maxAttempts = remaining * 100
  const logProgress = createProgressLogger("likes", likesCount)
  logProgress(likesCount - remaining)


  database.exec("BEGIN")
  try {
    while (inserted < remaining) {
      if (++attempts > maxAttempts) {
        throw new Error(
          "Too many duplicate picks; lower the skew or the like count",
        )
      }
      const result = insertLike.run(pickUserId(), pickPostId())
      inserted += Number(result.changes)
      logProgress(likesCount - remaining + inserted)
    }
    database.exec("COMMIT")
  } catch (error) {
    database.exec("ROLLBACK")
    throw error
  }

  console.log(`likes: now has ${countRows("likes")}`)
}


// resetDatabase()
// initDatabase()

// await seedUsers(USERCOUNT);
// await seedPosts(POSTCOUNT);
// await seedLikes(LIKECOUNT);
