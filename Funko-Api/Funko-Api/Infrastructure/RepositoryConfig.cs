namespace Funko_Api.Infrastructure;


public static class RepositoryConfig {
    public static IServiceCollection AddRepositories(this IServiceCollection service) {
        service.AddScoped<IFunkoRepository, FunkoRepository>();
        return service;
    }
}