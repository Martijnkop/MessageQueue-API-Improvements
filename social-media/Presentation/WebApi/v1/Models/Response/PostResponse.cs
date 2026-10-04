namespace Social.WebApi.v1.Models.Response;

public record PostResponse
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
}
