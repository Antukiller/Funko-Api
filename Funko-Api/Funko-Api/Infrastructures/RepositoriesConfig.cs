using Funko_Api.Repositories;
using Funko_Api.Repositories.Categoria;
using Microsoft.Extensions.DependencyInjection;

namespace Funko_Api.Infrastructures;

/// <summary>
/// Registro de dependencias de la capa de acceso a datos.
/// </summary>
public static class RepositoriesConfig {
    /// <summary>
    /// Registra los repositorios de la aplicación.
    /// </summary>
    /// <param name="services">Contenedor de dependencias.</param>
    /// <returns>El mismo contenedor para encadenar llamadas.</returns>
    public static IServiceCollection AddRepositoriesConfiguration(this IServiceCollection services) {
        services.AddScoped<IFunkoRepository, FunkoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();

        return services;
    }
}
