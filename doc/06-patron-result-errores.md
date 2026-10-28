# 6. Patrón Result y errores

## Índice

- [6.1. `Result<T, DomainError>`](#61-resultt-domainerror)
- [6.2. Errores de dominio](#62-errores-de-dominio)
- [6.3. Traducción a HTTP](#63-traducción-a-http)
- [6.4. Excepciones no controladas](#64-excepciones-no-controladas)

---

## 6.1. `Result<T, DomainError>`

Los servicios no lanzan excepciones para errores esperables; devuelven
`Result<T, DomainError>` de `CSharpFunctionalExtensions`:

```csharp
public async Task<Result<FunkoDto, DomainError>> GetByIdAsync(long id) {
    var funko = await repository.GetByIdAsync(id);
    return funko is null
        ? Result.Failure<FunkoDto, DomainError>(NotFound.FromId(id, "Funko"))
        : Result.Success<FunkoDto, DomainError>(funko.ToDto());
}
```

El controlador no decide códigos: delega en `ToHttpResult`.

## 6.2. Errores de dominio

| Error | Significado | HTTP |
|---|---|---|
| `NotFound` | El recurso no existe | 404 |
| `ValidationError` | Datos inválidos (con `Errors` por campo) | 400 |
| `ConflictError` | Nombre duplicado | 409 |
| `BusinessRuleError` | Regla de negocio (stock insuficiente) | 422 |
| Desconocido | Fallo inesperado | 500 |

Los errores son `record` inmutables y no conocen `HttpContext`.

## 6.3. Traducción a HTTP

`Errors/DomainErrorExtension.cs` es el único punto de traducción:

```csharp
public static ActionResult ToHttpResult(this DomainError error) => error switch {
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
    // 409, 422 y 500...
};
```

Los errores se serializan como `application/problem+json` (RFC 7807) y los
correctos como `application/json`. Por eso **no** se fuerza
`[Produces("application/json")]` en el controlador.

## 6.4. Excepciones no controladas

`Middleware/GlobalExceptionHandler.cs` captura lo que nadie esperaba:

| Excepción | HTTP |
|---|---|
| `DbUpdateException` | 409 |
| `BadHttpRequestException` | Su `StatusCode` |
| `TimeoutException` | 408 |
| Cualquier otra | 500 |

Devuelve `ProblemDetails` con `traceId` y `timestamp` para correlacionar con
Serilog. Si la respuesta ya empezó a enviarse, registra y propaga.
