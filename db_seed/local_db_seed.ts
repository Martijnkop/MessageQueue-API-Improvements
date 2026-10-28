import { Utf8Stream } from "fs"
import { DatabaseSync } from "node:sqlite"
const database = new DatabaseSync("database.db")

const USERCOUNT = 1000
const POSTCOUNT = 5000
const LIKECOUNT = 100000

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

type JokeResponse = { jokes: { joke: string }[] } | { joke: string }

function resetDatabase(): void {
  database.exec(`
    DROP TABLE IF EXISTS likes;
    DROP TABLE IF EXISTS posts;
    DROP TABLE IF EXISTS users;
  `)
}

function initDatabase() {
  database.exec(`
    CREATE TABLE IF NOT EXISTS users(
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      email TEXT NOT NULL,
      password TEXT NOT NULL,
      accessToken TEXT
    )
    `)

  database.exec(`
    CREATE TABLE IF NOT EXISTS posts(
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      userId INTEGER NOT NULL,
      content TEXT NOT NULL,
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

function createProgressLogger(
  label: string,
  target: number,
  stepPercent = 10,
): (current: number) => void {
  let lastBucket = -1

  return (current: number) => {
    const percent = Math.min(100, Math.floor((current / target) * 100))
    const bucket = percent === 100 ? 100 : Math.floor(percent / stepPercent)
    if (bucket === lastBucket) return
    lastBucket = bucket

    const width = 20
    const filled = Math.round((percent / 100) * width)
    const bar = "#".repeat(filled) + ".".repeat(width - filled)
    console.log(
      `${label.padEnd(6)} [${bar}] ${String(percent).padStart(3)}%  (${current}/${target})`,
    )
  }
}


async function seedUsers(userCount: number) {
  let remaining = userCount - countRows("users")
  if (remaining <= 0) {
    console.log(`users: already has ${countRows("users")} users`)
    return
  }

  const insertUser = database.prepare(
    "INSERT INTO users (email, password) VALUES (?, ?)",
  )

  const logProgress = createProgressLogger("users", userCount)
  logProgress(countRows("users"))


  while (userCount > 0) {
    const batchSize = Math.min(userCount, 1000)
    const response = await fetch(`https://randomuser.me/api/?results=${batchSize}`);

    const data = (await response.json()) as RandomUserResponse
    userCount -= batchSize

    database.exec("BEGIN")
    for (const user of data.results) {
      insertUser.run(user.email, user.login.username) // We use username as the password
    }
    database.exec("COMMIT")
    logProgress(countRows("users"))
  }
}


async function seedPosts(postCount: number) {
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


  while (remaining > 0) {
    const batchSize = Math.min(remaining, 10) // 10 per request is the max
    const response = await fetch(
      `https://v2.jokeapi.dev/joke/Any?type=single&amount=${batchSize}`,
    )
    const data = (await response.json()) as JokeResponse
    remaining -= batchSize

    const jokes = "jokes" in data ? data.jokes : [data] // amount=1 returns one object

    database.exec("BEGIN")
    for (const post of jokes) {
      insertPost.run(pickUserId(), post.joke)
    }
    database.exec("COMMIT")
    logProgress(countRows("posts"))
  }

  console.log(`posts: now has ${countRows("posts")} posts`)
}

async function seedLikes(likesCount: number) {
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


resetDatabase()
initDatabase()

await seedUsers(USERCOUNT);
await seedPosts(POSTCOUNT);
await seedLikes(LIKECOUNT);
