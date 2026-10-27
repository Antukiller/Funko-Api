using Funko_Api.Infrastructures.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Funko_Api.Infrastructures;

public static class DependencerProvider {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
        services.Scan(scan => scan
            .FromAssemblyOf<IScopedService>()
            .AddClasses(classes => classes.AssignableTo<IScopedService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ITransientService>())
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        return services;
    }
}