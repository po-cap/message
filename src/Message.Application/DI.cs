using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Po.Media;
using Shared.Mediator;

namespace Message.Application;

public static class DI
{
    public static IServiceCollection AddApplication(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddMediaService(configuration);

        services.AddMediator();
        
        return services;
    }
}