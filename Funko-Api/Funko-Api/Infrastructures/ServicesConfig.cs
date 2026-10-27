using Funko_Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Funko_Api.Infrastructures;

/// <summary>
/// Registro de dependencias de la capa de negocio.
/// </summary>
public static class ServicesConfig {
    /// <summary>
    /// Registra los servicios de la aplicación.
    /// </summary>
    /// <param name="services">Contenedor de dependencias.</param>
    /// <returns>El mismo contenedor para encadenar llamadas.</returns>
    public static IServiceCollection AddServicesConfiguration(this IServiceCollection services) {
        services.AddScoped<IFunkoService, FunkoService>();
        services.AddScoped<ICategoriaService, CategoriaService>();

        return services;
    }
}
