using Social.Domain.Models.Base;

namespace Social.Domain.Models;

public record Post : Entity
{
    public required string Title { get; set; }
    public required string Message { get; set; }
}
