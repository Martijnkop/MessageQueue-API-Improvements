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

        const keycloakUrl = "http://localhost:8080/realms/social/protocol/openid-connect/token";

        const body = new URLSearchParams({
            grant_type: "password",
            client_id: "my-client",
            username: "myuser",
            password: "mypassword",
            scope: "openid",
            // client_secret: "YOUR_CLIENT_SECRET", // only for confidential clients
        });
    }
}

seedUsers()


async function seedPosts() {
    const response = await fetch("https://v2.jokeapi.dev/joke/Any?type=single&amount=10") // 10 per request is the max
}