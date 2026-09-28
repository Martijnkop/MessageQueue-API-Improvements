using Microsoft.Extensions.Logging;
using POS.Business.Abstractions.Services;
using POS.Domain.Models;
using POS.Domain.Models.DTOs;
using POS.WebApi.Mappers;
using POS.WebApi.v1.Controllers.Base;
using POS.WebApi.v1.Models.Request;
using POS.WebApi.v1.Models.Response;

namespace POS.WebApi.v1.Controllers;

public class ProductController : Controller<Product, ProductResponse, CreateProductRequest, EditProductRequest, CreateProductDTO, EditProductDTO>
{
    public ProductController(
        IProductService productService,
        ILogger<ProductController> logger
    ) : base(
        productService,
        new ProductMapper(),
        logger)
    {
    }
}
