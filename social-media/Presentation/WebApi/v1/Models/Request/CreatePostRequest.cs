using System.ComponentModel.DataAnnotations;

namespace Social.WebApi.v1.Models.Request;

public record CreatePostRequest
{
    [MinLength(8, ErrorMessage = "Post title must be at least 8 characters long")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Post must include a message")]
    public required string Message { get; set; }
}
