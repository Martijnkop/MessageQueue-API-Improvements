using Microsoft.EntityFrameworkCore;
using Social.Domain.Models;

namespace Social.Persistence;

public class AppDataStore : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDataStore(DbContextOptions<AppDataStore> options) : base(options)
    {
    }
}
