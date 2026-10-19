namespace Funko_Api.Infrastructure;

public static class ServiceConfig {
    public static IServiceCollection AddServices(this IServiceCollection service) {
        service.AddSingleton<IFunkoService, FunkoService>();
        return service;
    }
}