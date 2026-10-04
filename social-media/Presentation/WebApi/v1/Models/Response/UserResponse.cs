namespace Social.WebApi.v1.Models.Response;

public record UserResponse
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public List<PostResponse>? Posts { get; set; }
    public List<LikeResponse>? Likes { get; set; }
}
