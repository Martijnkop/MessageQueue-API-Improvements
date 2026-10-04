namespace Social.Domain.Models.DTOs;

public record CreatePostDTO
{
    public required string Title { get; set; }
    public required string Message { get; set; }
}
