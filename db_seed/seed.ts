import { deleteAllIdentities } from "./seed_parts/delete_kratos_identities.ts";
import { generateAccessTokens, resetAccessTokens } from "./seed_parts/generate_identity_and_tokens.ts";
import { initDatabase, resetDatabase, seedLikes, seedPosts, seedUsers } from "./seed_parts/local_db_seed.ts";
import { close, fetchMeForAllUsers, postPosts, publishLikes } from "./seed_parts/send_to_backend.ts";

const USERCOUNT = 10
const POSTCOUNT = 20
const LIKECOUNT = 50

resetDatabase()
initDatabase()

await seedUsers(USERCOUNT)
await seedPosts(POSTCOUNT)
await seedLikes(LIKECOUNT)

// Kratos db
await deleteAllIdentities()
// Local db
resetAccessTokens()
await generateAccessTokens()

await fetchMeForAllUsers(USERCOUNT)

await postPosts()

await publishLikes()

await close()
