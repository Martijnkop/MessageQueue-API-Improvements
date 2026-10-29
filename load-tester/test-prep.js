import { DatabaseSync } from "node:sqlite"
import { writeFileSync } from 'fs';

const args = Object.fromEntries(
  process.argv.slice(2).reduce((acc, a, i, all) => {
    if (a.startsWith('--')) acc.push([a.slice(2), all[i + 1]])
    return acc
  }, [])
);

const DB_PATH = args.db || '../db_seed/database.db'
const USER_COUNT = parseInt(args.users || '10', 10) // AT LEAST the amount of VU's
const TERM_COUNT = parseInt(args.terms || '30', 10)
const MIN_WORD_LENGTH = parseInt(args.minlen || '5', 10)
const SAMPLE_POSTS = parseInt(args.sample || '20', 10) // How many posts to get words from

const db = new DatabaseSync(DB_PATH, { readOnly: true })

const users = db
  .prepare(
    `SELECT id, social_userid, accessToken
     FROM users
     WHERE accessToken IS NOT NULL AND accessToken != ''
     ORDER BY id
     LIMIT ?
    `
).all(USER_COUNT)

if (users.length < USER_COUNT) {
  throw new Error("Too few users with accessToken found")
}

const posts = db
  .prepare(`SELECT content FROM posts ORDER BY RANDOM() LIMIT ?`)
  .all(SAMPLE_POSTS)

const words = new Set()
for (const { content } of posts) {
  for (const w of content.toLowerCase().match(/[\p{L}\p{N}]+/gu) || []) {
    if (w.length >= MIN_WORD_LENGTH) words.add(w)
  }
}

const terms = [...words].sort(() => Math.random() - 0.5).slice(0, TERM_COUNT);

if (terms.length < TERM_COUNT) throw new Error("Not enough valid search words in posts")

writeFileSync('users.json', JSON.stringify(users));
writeFileSync('searchterms.json', JSON.stringify(terms));

console.log(`users.json: ${users.length} users`);
console.log(`searchterms.json: ${terms.length} terms`);
