using Message.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Message.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    /// <summary>
    /// 訊息
    /// </summary>
    public DbSet<Note> Notes { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public DbSet<Item> Items { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var config = new DbConfig();
        modelBuilder.ApplyConfiguration<Note>(config);
        modelBuilder.ApplyConfiguration<Item>(config);
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetToUtcConverter>();
    }
}