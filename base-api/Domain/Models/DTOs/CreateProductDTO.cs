namespace POS.Domain.Models.DTOs;

public record CreateProductDTO
{
    public required string Name { get; set; }
}
