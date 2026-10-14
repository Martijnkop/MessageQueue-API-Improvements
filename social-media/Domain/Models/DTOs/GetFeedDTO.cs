using System.ComponentModel.DataAnnotations.Schema;

namespace Social.Domain.Models.DTOs;

public record GetFeedDTO
{
    public required Guid Id { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public int LikeCount { get; set; }
}
