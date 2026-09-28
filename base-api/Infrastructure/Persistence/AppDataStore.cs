using Microsoft.EntityFrameworkCore;
using POS.Domain.Models;

namespace POS.Persistence;

public class AppDataStore : DbContext
{
    public DbSet<Product> Products { get; set; }

    public AppDataStore(DbContextOptions<AppDataStore> options) : base(options)
    {
    }
}
