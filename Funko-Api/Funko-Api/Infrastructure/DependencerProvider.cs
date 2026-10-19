using Funko_Api.Infrastructure.Interface;

namespace Funko_Api.Infrastructure;

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