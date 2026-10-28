using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace Funko_Api.Infrastructures;

/// <summary>
/// Configuración de Swagger/OpenAPI.
/// </summary>
public static class SwaggerConfig {
    /// <summary>
    /// Registra Swagger e incluye los comentarios XML del proyecto.
    /// </summary>
    /// <param name="services">Contenedor de dependencias.</param>
    /// <returns>El mismo contenedor para encadenar llamadas.</returns>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services) {
        services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo {
                Title = "Funko API",
                Version = "v1",
                Description = "API para gestionar el catálogo de funkos y sus categorías."
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        return services;
    }
}
