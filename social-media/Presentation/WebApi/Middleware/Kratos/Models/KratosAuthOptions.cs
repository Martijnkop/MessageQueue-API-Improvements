using Microsoft.AspNetCore.Authentication;

namespace Social.WebApi.Middleware.Kratos.Models;

public class KratosAuthOptions : AuthenticationSchemeOptions
{
    public string PublicUrl { get; set; } = "http://localhost:4433";
    public string EmailTrait { get; set; } = "email";
    public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(5);
}
