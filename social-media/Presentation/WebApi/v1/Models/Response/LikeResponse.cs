namespace Social.WebApi.v1.Models.Response;

public record LikeResponse
{
    public Guid Id { get; set; }
    public UserResponse? User { get; set; }
    public PostResponse? Post { get; set; }
}
