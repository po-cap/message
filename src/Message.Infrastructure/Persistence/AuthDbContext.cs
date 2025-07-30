using Message.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Message.Infrastructure.Persistence;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> opts) : base(opts) { }

    /// <summary>
    /// 訊息
    /// </summary>
    public DbSet<User> Users { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var config = new DbConfig();
        modelBuilder.ApplyConfiguration<User>(config);
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetToUtcConverter>();
    }
}