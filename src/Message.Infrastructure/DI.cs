using Message.Application.Services;
using Message.Domain.Entities;
using Message.Domain.Repositories;
using Message.Infrastructure.Persistence;
using Message.Infrastructure.Repositories;
using Message.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Message.Infrastructure;

public static class DI
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // description - 主資料庫配置
        //services.AddDbContextFactory<AppDbContext>(options => 
        //    options.UseNpgsql(config.GetConnectionString("Main"), o =>
        //    {
        //        o.MapEnum<DataType>("message_type");
        //    }));
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseNpgsql(config.GetConnectionString("Main"), o =>
            {
                o.MapEnum<DataType>("message_type");
            });
        });
        
        // description - Auth資料庫配置
        services.AddDbContext<AuthDbContext>(opts =>
        {
            opts.UseNpgsql(config.GetConnectionString("Main"));
        });
        
        // description - Message Mediator
        services.AddSingleton<IMessenger, Messenger>();
        
        // description - snowflake id
        services.AddSingleton<SnowflakeId>(provider => 
            new SnowflakeId(workerId: 1, datacenterId: 1)
        );

        // description - repositories
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();

        // description - 使用者 Repository 使用了 Http Client
        services.AddHttpClient<AuthClient>(client =>
        {
            client.BaseAddress = new Uri($"{config["OIDC"]}");
        });
        
        return services;
    }
}