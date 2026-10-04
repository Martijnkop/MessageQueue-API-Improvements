using System.ComponentModel.DataAnnotations;

namespace Social.WebApi.v1.Models.Request;

public record CreateUserRequest
{
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
    public required string Username { get; set; }
}
