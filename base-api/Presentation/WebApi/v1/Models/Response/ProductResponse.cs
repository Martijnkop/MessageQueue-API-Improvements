namespace POS.WebApi.v1.Models.Response;

public record ProductResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
