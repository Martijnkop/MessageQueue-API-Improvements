using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using POS.Domain.Abstractions.Repositories;
using POS.Domain.Models;
using POS.Persistence.Repositories.Base;

namespace POS.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDataStore dbContext, ILogger<ProductRepository> logger) : base(dbContext.Products, logger)
    {
    }
}
