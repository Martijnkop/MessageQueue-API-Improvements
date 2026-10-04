namespace Social.Domain.Models.DTOs;

public record CreateUserDTO
{
    public required string Username { get; set; }
}
