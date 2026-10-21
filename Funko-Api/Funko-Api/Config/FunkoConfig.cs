using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Funko_Api.Config;


public static class FunkoConfig {
    public static IConfiguration Config { get; private set; } = null!;
    
    public static void Init(string[] args) {
        var archivoConfiguracion = (args.FirstOrDefault() ?? "Development").ToLowerInvariant() switch {
            "development" => "appsettings.Development.json",
            "production" => "appsettings.Production.json",
            var entorno => throw new ArgumentException($"Entorno de configuración no soportado: {entorno}.", nameof(args))
        };
        
        Config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(archivoConfiguracion, false, true)
            .AddEnvironmentVariables()
            .Build();
    }

    // Obligamos a que se lea estrictamente del JSON
    public static string ApiName => 
        Config.GetValue<string>("ApiSettings:Name") 
        ?? throw new InvalidOperationException("Falta ApiSettings:Name en el archivo JSON.");

    public static string RepositoryName => 
        Config.GetValue<string>("Repository:Name") 
        ?? throw new InvalidOperationException("Falta Repository:Name en el archivo JSON.");

    public static string DbConnection =>
        RepositoryName.Equals("SQLite", StringComparison.OrdinalIgnoreCase)
            ? $"Data Source={Path.Combine(DataFolder, "funkos.db")}"
            : Config.GetValue<string>("Repository:ConnectionString") is { } connectionString
              && !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new InvalidOperationException(
                    "Falta la variable de entorno Repository__ConnectionString para la conexión de producción.");
    
    public static string DataFolder => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "data"
    );
}