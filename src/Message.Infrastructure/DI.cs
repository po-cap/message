using Message.Application.Services;
using Message.Domain.Entities;
using Message.Domain.Repositories;
using Message.Infrastructure.Persistence;
using Message.Infrastructure.Repositories;
using Message.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Mediator;

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
                o.MapEnum<NoteType>("message_type");
            });
        });
        
        // description - Auth資料庫配置
        services.AddDbContext<AuthDbContext>(opts =>
        {
            opts.UseNpgsql(config.GetConnectionString("Main"));
        });
        
        // description - Message Mediator
        services.AddScoped<IMessenger, Messenger>();
        services.AddSingleton<IConnection,Connection>();
        
        // description - snowflake id
        services.AddSingleton<ISnowFlake>(provider => 
            new SnowflakeId(workerId: 1, datacenterId: 1)
        );

        // description - repositories
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();

        // description - 使用者 Repository 使用了 Http Client
        services.AddHttpClient<AuthClient>(client =>
        {
            client.BaseAddress = new Uri($"{config["OIDC"]}");
        });
        
        
        
        // 加入中介者
        services.AddMediator();
        
        return services;
    }
}