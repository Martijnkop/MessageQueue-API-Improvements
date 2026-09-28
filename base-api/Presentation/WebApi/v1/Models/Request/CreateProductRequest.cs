namespace POS.WebApi.v1.Models.Request;

public record CreateProductRequest
{
    public required string Name { get; set; }
}
