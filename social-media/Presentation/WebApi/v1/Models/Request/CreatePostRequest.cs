namespace Social.WebApi.v1.Models.Request;

public record CreatePostRequest
{
    public required string Title { get; set; }
    public required string Message { get; set; }
}
