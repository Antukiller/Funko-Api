using FluentValidation;
using Funko_Api.Dtos;
using Funko_Api.Models;
using Funko_Api.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Funko_Api.Infrastructures;

/// <summary>
/// Registro de dependencias de los validadores de FluentValidation.
/// </summary>
public static class ValidatorsConfig {
    /// <summary>
    /// Registra los validadores de la aplicación.
    /// </summary>
    /// <param name="services">Contenedor de dependencias.</param>
    /// <returns>El mismo contenedor para encadenar llamadas.</returns>
    public static IServiceCollection AddValidatorsConfiguration(this IServiceCollection services) {
        services.AddScoped<IValidator<Funko>, FunkoRequestValidator>();
        services.AddScoped<IValidator<CreateFunkoDto>, CreateFunkoValidator>();
        services.AddScoped<IValidator<UpdateFunkoDto>, UpdateFunkoValidator>();

        return services;
    }
}
