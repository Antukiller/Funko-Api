using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Funko_Api.Errors;

/// <summary>
/// Traduce errores de dominio y resultados a respuestas HTTP con Problem Details.
/// Es el único punto donde el dominio se convierte en códigos de estado.
/// </summary>
public static class DomainErrorExtensions {
    /// <summary>
    /// Convierte un error de dominio en una respuesta HTTP.
    /// </summary>
    public static IActionResult ToHttpResult(this DomainError error) => error switch {
        NotFound notFound => new NotFoundObjectResult(new ProblemDetails {
            Status = StatusCodes.Status404NotFound,
            Title = "Recurso no encontrado",
            Detail = notFound.Message
        }),
        ValidationError validationError => new BadRequestObjectResult(
            new ValidationProblemDetails(validationError.Errors) {
                Status = StatusCodes.Status400BadRequest,
                Title = validationError.Message
            }),
        ConflictError conflict => new ConflictObjectResult(new ProblemDetails {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflicto",
            Detail = conflict.Message
        }),
        BusinessRuleError businessRule => new UnprocessableEntityObjectResult(new ProblemDetails {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Regla de negocio incumplida",
            Detail = businessRule.Message
        }),
        _ => new ObjectResult(new ProblemDetails {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Error interno del servidor"
        }) {
            StatusCode = StatusCodes.Status500InternalServerError
        }
    };

    /// <summary>
    /// Devuelve <c>200 OK</c> con el valor si el resultado es correcto o el error traducido si no.
    /// </summary>
    public static IActionResult ToHttpResult<T>(this Result<T, DomainError> result) =>
        result.IsSuccess
            ? new OkObjectResult(result.Value)
            : result.Error.ToHttpResult();
}
