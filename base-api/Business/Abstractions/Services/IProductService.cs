using POS.Business.Abstractions.Services.Base;
using POS.Domain.Models;
using POS.Domain.Models.DTOs;

namespace POS.Business.Abstractions.Services;

public interface IProductService : IService<Product, CreateProductDTO, EditProductDTO>
{
}
