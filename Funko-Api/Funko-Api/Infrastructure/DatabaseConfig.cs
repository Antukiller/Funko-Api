using Funko_Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Funko_Api.Infrastructure;

/// <summary>
/// Configuración de la base de datos de la aplicación.
/// </summary>
public static class DatabaseConfig {
    /// <summary>
    /// Registra el <see cref="AppDbContext"/> usando PostgreSQL.
    /// </summary>
    /// <param name="services">Contenedor de dependencias.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    /// <returns>El mismo contenedor para encadenar llamadas.</returns>
    /// <exception cref="InvalidOperationException">
    /// Si no existe la cadena de conexión <c>DefaultConnection</c>.
    /// </exception>
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    ) {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se ha configurado la cadena de conexión 'DefaultConnection'.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}
