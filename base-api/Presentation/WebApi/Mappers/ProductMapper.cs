using POS.Domain.Models;
using POS.Domain.Models.DTOs;
using POS.WebApi.Abstractions;
using POS.WebApi.v1.Models.Request;
using POS.WebApi.v1.Models.Response;

namespace POS.WebApi.Mappers;

public class ProductMapper : IMapper<Product, ProductResponse, CreateProductRequest, CreateProductDTO, EditProductRequest, EditProductDTO>
{
    public ProductResponse Map(Product input) => _Map(input, new List<Types>())!;

    internal static ProductResponse? _Map(Product input, List<Types> types)
    {
        if (input is null) return null;
        List<Types> excludeTypes = [.. types];

        excludeTypes.Add(Types.Product);

        return new ProductResponse
        {
            Id = input.Id,
            Name = input.Name
        };
    }

    public EditProductDTO Map(EditProductRequest input) => new EditProductDTO
    {
        Name = input.Name
    };
    public CreateProductDTO Map(CreateProductRequest input) => new CreateProductDTO
    {
        Name = input.Name
    };
}
