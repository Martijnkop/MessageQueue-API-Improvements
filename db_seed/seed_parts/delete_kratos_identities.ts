const KRATOS_ADMIN_URL = "http://localhost:4434"
const PAGE_SIZE = 500

export async function deleteAllIdentities() {
  let deleted = 0

  while (true) {
    // Always fetch the first page: deleting shifts everything up,
    // so paginating with a token would skip identities.
    const res = await fetch(
      `${KRATOS_ADMIN_URL}/admin/identities?page_size=${PAGE_SIZE}`,
    )
    if (!res.ok) throw new Error(`List failed: ${res.status}`)

    const identities = (await res.json()) as { id: string }[]
    if (identities.length === 0) break

    let deletedThisPage = 0
    for (const { id } of identities) {
      const del = await fetch(`${KRATOS_ADMIN_URL}/admin/identities/${id}`, {
        method: "DELETE",
      })
      if (del.ok || del.status === 404) {
        deleted++
        deletedThisPage++
      } else {
        console.error(`Failed to delete ${id}: ${del.status}`)
      }
    }

    // Avoid looping forever if every delete in a page fails
    if (deletedThisPage === 0) throw new Error("No progress, aborting")
    console.log(`Deleted ${deleted} identities so far`)
  }

  console.log(`Done, deleted ${deleted} identities`)
}


// await deleteAllIdentities()
