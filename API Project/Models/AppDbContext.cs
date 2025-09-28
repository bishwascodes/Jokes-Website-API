using Microsoft.EntityFrameworkCore;

namespace Jokes.Models;

public class AppDbContext : DbContext
{
    //Correctly scaffold your DBContext and associated classes, appropriately inject DBContext into your classes
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<DbJoke> Jokes => Set<DbJoke>();
    public DbSet<DbCategory> Categories => Set<DbCategory>();
    public DbSet<DbAudience> Audiences => Set<DbAudience>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Default schema for all tables
        modelBuilder.HasDefaultSchema("jokes");
    }
}
