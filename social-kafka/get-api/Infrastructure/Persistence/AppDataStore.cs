using Microsoft.EntityFrameworkCore;
using Social.Domain.Models;

namespace Social.Persistence;

public class AppDataStore : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Like> Likes { get; set; }

    public AppDataStore(DbContextOptions<AppDataStore> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDataStore).Assembly);
}
