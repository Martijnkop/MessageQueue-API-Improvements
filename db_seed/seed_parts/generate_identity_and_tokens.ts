import { DatabaseSync } from "node:sqlite"

const database = new DatabaseSync("database.db")

const KRATOS_API = "http://localhost:4433"
const BATCH_SIZE = 1000

interface UserRow {
  id: number
  email: string
  password: string
}


// Sets accessToken to NULL for every user. Returns how many rows were updated.
export function resetAccessTokens(): number {
  const result = database.prepare("UPDATE users SET accessToken = NULL").run()
  const updated = Number(result.changes)
  console.log(`accessTokens: reset for ${updated} users`)
  return updated
}


async function startFlow(type: "registration" | "login"): Promise<string> {
  const response = await fetch(`${KRATOS_API}/self-service/${type}/api`, {
    headers: { Accept: "application/json" },
  })
  if (!response.ok) {
    throw new Error(`Kratos ${type} flow failed: ${response.status} ${await response.text()}`)
  }
  const flow = (await response.json()) as { id: string }
  return flow.id
}

async function registerUser(user: UserRow): Promise<void> {
  const flowId = await startFlow("registration")
  const response = await fetch(
    `${KRATOS_API}/self-service/registration?flow=${flowId}`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json", Accept: "application/json" },
      body: JSON.stringify({
        method: "password",
        password: user.password,
        traits: { email: user.email }, // must match your identity schema
      }),
    },
  )
  if (response.ok) return

  const body = await response.text()
  if (body.includes("4000007")) return // Kratos message id: identifier already exists
  throw new Error(`Registering ${user.email} failed: ${response.status} ${body}`)
}

async function createSession(user: UserRow): Promise<string> {
  const flowId = await startFlow("login")
  const response = await fetch(
    `${KRATOS_API}/self-service/login?flow=${flowId}`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json", Accept: "application/json" },
      body: JSON.stringify({
        method: "password",
        identifier: user.email,
        password: user.password,
      }),
    },
  )
  if (!response.ok) {
    throw new Error(`Login for ${user.email} failed: ${response.status} ${await response.text()}`)
  }
  const data = (await response.json()) as { session_token?: string }
  if (!data.session_token) {
    throw new Error(`Login for ${user.email} returned no session_token`)
  }
  return data.session_token
}

export async function generateAccessTokens() {
  const total = (
    database
      .prepare("SELECT COUNT(*) AS count FROM users WHERE accessToken IS NULL")
      .get() as { count: number }
  ).count
  if (total === 0) {
    console.log("accessTokens: every user already has one, nothing to do")
    return
  }

  // Keyset pagination (id > lastId) instead of OFFSET: finished users drop out
  // of the WHERE clause, so an OFFSET would skip rows.
  const selectBatch = database.prepare(
    "SELECT id, email, password FROM users WHERE accessToken IS NULL AND id > ? ORDER BY id LIMIT ?",
  )
  const storeToken = database.prepare(
    "UPDATE users SET accessToken = ? WHERE id = ?",
  )

  let lastId = 0
  let done = 0

  while (true) {
    const users = selectBatch.all(lastId, BATCH_SIZE) as unknown as UserRow[]
    if (users.length === 0) break

    for (const user of users) {
      await registerUser(user)
      const token = await createSession(user)
      storeToken.run(token, user.id)
    }

    lastId = users[users.length - 1].id
    done += users.length
    console.log(
      `accessTokens: ${done}/${total} (${Math.floor((done / total) * 100)}%)`,
    )
  }
}

// resetAccessTokens()

// await generateAccessTokens()
