using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funko_Api.Middleware;

/// <summary>
/// Manejador global de excepciones no controladas.
/// Devuelve siempre un <see cref="ProblemDetails"/> con un <c>traceId</c> para poder
/// correlacionar la respuesta con la traza del log.
/// </summary>
public class GlobalExceptionHandler(
    RequestDelegate next,
    ILogger<GlobalExceptionHandler> logger
) {
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Ejecuta el resto del pipeline y captura cualquier excepción no controlada.
    /// </summary>
    /// <param name="context">Contexto HTTP de la petición en curso.</param>
    public async Task InvokeAsync(HttpContext context) {
        try {
            await next(context);
        }
        catch (Exception exception) {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception) {
        // Si la respuesta ya ha empezado a enviarse no se puede reescribir: se propaga.
        if (context.Response.HasStarted) {
            logger.LogError(exception, "Excepción con la respuesta ya iniciada en {Path}",
                context.Request.Path);
            throw exception;
        }

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        logger.LogError(exception, "Excepción no controlada. TraceId: {TraceId}, Path: {Path}",
            traceId, context.Request.Path);

        var (statusCode, title, detail) = exception switch {
            DbUpdateException => (
                StatusCodes.Status409Conflict,
                "Conflicto al guardar en la base de datos",
                "Los datos no cumplen las restricciones de la base de datos."),
            BadHttpRequestException badRequest => (
                badRequest.StatusCode,
                "Petición incorrecta",
                badRequest.Message),
            TimeoutException => (
                StatusCodes.Status408RequestTimeout,
                "Tiempo de espera agotado",
                "La operación ha tardado demasiado."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Se ha producido un error inesperado.")
        };

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.io/{statusCode}"
        };
        problem.Extensions["traceId"] = traceId;
        problem.Extensions["timestamp"] = DateTime.UtcNow;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }
}

/// <summary>
/// Extensiones para registrar el middleware de excepciones.
/// </summary>
public static class GlobalExceptionHandlerExtensions {
    /// <summary>
    /// Añade el manejador global de excepciones al pipeline.
    /// </summary>
    /// <param name="app">Constructor de la aplicación.</param>
    /// <returns>El mismo <see cref="IApplicationBuilder"/> para encadenar.</returns>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app) =>
        app.UseMiddleware<GlobalExceptionHandler>();
}
