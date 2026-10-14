using Social.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social.WebApi.v1.Models.Response;

public record GetFeedResponse
{
    public required Guid Id { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }

    public Guid UserId { get; set; }
    public required UserResponse User { get; set; }
    public int LikeCount { get; set; }
}
