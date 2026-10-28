import { DatabaseSync } from "node:sqlite"
import { Client } from "pg"
import { v7 as uuidv7 } from "uuid"

const backend = new Client({ connectionString: "postgres://postgres:postgres@localhost:5432/database" })
await backend.connect()

const database = new DatabaseSync("database.db")

const BACKEND_URL = "http://localhost:32770"

interface UserRow {
  id: number
  email: string
  password: string
  accessToken: string
}

interface UserPost {
  id: number
  accessToken: string
  content: string
}


const updateSocialUserId = database.prepare(
  "UPDATE users SET social_userid = ? WHERE id = ?",
)

async function fetchMe(user: UserRow) {
  const response = await fetch(`${BACKEND_URL}/users/me`, {
    headers: {
      "Authorization": `Bearer ${user.accessToken}`
    },
  })

  if (!response.ok) {
    throw new Error()
  }

  if (!response.ok) {
    throw new Error()
  }

  const created = (await response.json()) as { id: string }
  updateSocialUserId.run(created.id, user.id)

  return created

}



export async function fetchMeForAllUsers(limit = 1000) {
  const users = database
    .prepare("SELECT id, email, password, accessToken FROM users LIMIT ?")
    .all(limit) as unknown as UserRow[]

  for (const user of users) {
    try {
      const me = await fetchMe(user)
      console.log(user.email, me.id)
    } catch (err) {
      console.error(err)
    }
  }
}

const updateSocialId = database.prepare(
  "UPDATE posts SET social_id = ? WHERE id = ?",
)


async function postPost(post: UserPost) {
  const response = await fetch(`${BACKEND_URL}/posts`, {
    method: "POST",
    headers: {
      "Authorization": `Bearer ${post.accessToken}`,
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      message: post.content,
      title: post.content.split(" ").slice(0, 5).join(" ")
    })
  })

  if (!response.ok) {
    throw new Error()
  }

  const created = (await response.json()) as { id: string }
  updateSocialId.run(created.id, post.id)

  return created
}


export async function postPosts() {
  const posts = database
    .prepare("SELECT posts.id, posts.content, users.accessToken FROM posts JOIN users ON posts.userId = users.id WHERE posts.social_id IS NULL;")
    .all() as unknown as UserPost[]

  for (const post of posts) {
    try {
      const posted = await postPost(post)
      console.log(posted)
    } catch (err) {
      console.error(err)
    }
  }
}

interface UserLike {
  socialUserId: string  // users.social_userid
  socialPostId: string  // posts.social_id
}

export async function publishLikes() {
  const likes = database
      .prepare(
        `SELECT u.social_userid AS socialUserId, p.social_id AS socialPostId
         FROM likes l
         JOIN users u ON u.id = l.userId
         JOIN posts p ON p.id = l.postId
         WHERE u.social_userid IS NOT NULL
           AND p.social_id IS NOT NULL`,
      )
      .all() as unknown as UserLike[]

  await backend.query("BEGIN")
  try {
    for (const like of likes) {
          await backend.query(
            `INSERT INTO "Likes" ("Id", "UserId", "PostId", "CreatedTime")
             VALUES ($1, $2, $3, now())
             ON CONFLICT DO NOTHING`,
            [uuidv7(), like.socialUserId, like.socialPostId],
          )
        }

        await backend.query("COMMIT")
      } catch (err) {
        await backend.query("ROLLBACK")
        throw err
      }
    }

// await fetchMeForAllUsers(1000)

// await postPosts()

// await publishLikes()

export async function close() {

  await backend.end()
}
