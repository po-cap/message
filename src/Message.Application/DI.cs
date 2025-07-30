using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Po.Media;
using Shared.Mediator;

namespace Message.Application;

public static class DI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator();
        
        return services;
    }
}