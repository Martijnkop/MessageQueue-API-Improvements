async function seedUsers() {
    
    const response = await fetch("https://randomuser.me/api/?results=1000")
    const data = await response.json()
    
    for(let i = 0; i < 1000; i++) {
        const user = data.results[i]
    
        const response2 = await fetch("http://localhost:5015/users", {
            method: "POST",
            body: JSON.stringify({
                username: user.login.username
            }),
            headers: {
                "Content-Type": "application/json"
            }
        })
    
        console.log(i)
    }
}

seedUsers()


async function seedPosts() {
    const response = await fetch("https://v2.jokeapi.dev/joke/Any?type=single&amount=10") // 10 per request is the max
}