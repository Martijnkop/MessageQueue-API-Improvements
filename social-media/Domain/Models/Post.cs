using Social.Domain.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Social.Domain.Models;

public record Post : Entity
{
    public required string Title { get; set; }
    public required string Message { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
