using POS.Domain.Models.Base;

namespace POS.Domain.Models;

public record Product : Entity
{
    public required string Name { get; set; }
}
