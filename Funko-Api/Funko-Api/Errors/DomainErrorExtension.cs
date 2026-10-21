using Microsoft.AspNetCore.Http;

namespace Funko_Api.Errors;

public static class DomainErrorExtensions {
    public static IResult ToHttpResult(this DomainError error) => error switch {
        NotFound notFound => Results.Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Recurso no encontrado",
            detail: notFound.Message),
        ValidationError validationError => Results.ValidationProblem(
            validationError.Errors,
            statusCode: StatusCodes.Status400BadRequest,
            title: validationError.Message),
        _ => Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Error interno del servidor")
    };
}