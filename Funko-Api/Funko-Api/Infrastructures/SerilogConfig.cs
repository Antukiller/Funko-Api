using Serilog;
using Serilog.Events;

namespace Funko_Api.Infrastructures;

/// <summary>
/// Configuración de Serilog: consola para desarrollo y fichero diario para trazas.
/// </summary>
public static class SerilogConfig {
    /// <summary>
    /// Crea la configuración del logger con salida a consola y fichero rotativo.
    /// </summary>
    public static LoggerConfiguration Configure() =>
        new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Npgsql", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: Path.Combine("logs", "funko-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
}
