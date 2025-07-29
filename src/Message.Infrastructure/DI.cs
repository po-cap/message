using Message.Application.Services;
using Message.Domain.Entities;
using Message.Infrastructure.Persistence;
using Message.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Message.Infrastructure;

public static class DI
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // description - 主資料庫配置
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseNpgsql(config.GetConnectionString("Main"), o =>
            {
                o.MapEnum<DataType>("message_type");
            });
        });
        
        // description - Message Mediator
        services.AddSingleton<IMessenger, Messenger>();
        
        
        return services;
    }
}