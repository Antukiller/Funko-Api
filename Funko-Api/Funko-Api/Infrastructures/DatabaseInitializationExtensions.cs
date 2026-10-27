using Funko_Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Funko_Api.Infrastructures;

/// <summary>
/// Inicialización de la base de datos al arrancar la aplicación.
/// </summary>
public static class DatabaseInitializationExtensions {
    /// <summary>
    /// Aplica las migraciones pendientes de Entity Framework Core.
    /// </summary>
    /// <param name="services">Proveedor de servicios de la aplicación.</param>
    /// <returns>Tarea que representa la operación asíncrona.</returns>
    public static async Task InitializeDatabaseAsync(this IServiceProvider services) {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        logger.LogInformation("Applying pending database migrations");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database ready");
    }
}
