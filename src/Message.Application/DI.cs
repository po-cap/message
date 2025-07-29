using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Po.Media;

namespace Message.Application;

public static class DI
{
    public static IServiceCollection AddApplication(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddMessenger();
        services.AddMediaService(configuration);
        return services;
    }
}