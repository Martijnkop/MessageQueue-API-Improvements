using Microsoft.Extensions.Logging;
using POS.Business.Abstractions.Services;
using POS.Business.Services.Base;
using POS.Domain.Abstractions;
using POS.Domain.Abstractions.Repositories;
using POS.Domain.Abstractions.Repositories.Base;
using POS.Domain.Models;
using POS.Domain.Models.DTOs;

namespace POS.Business.Services;

public class ProductService : Service<Product, CreateProductDTO, EditProductDTO>, IProductService
{
    public ProductService(IProductRepository repository, IUnitOfWork unitOfWork, ILogger<ProductService> logger) : base(repository, unitOfWork, logger)
    {
    }

    protected override Product Map(CreateProductDTO entity)
    {
        return new Product
        {
            Name = entity.Name
        };
    }

    protected override Product Update(Product value, EditProductDTO entity)
    {
        value.Name = entity.Name ?? value.Name;
        return value;
    }
}
