# 2. Arquitectura y pipeline HTTP

## Índice

- [2.1. Capas](#21-capas)
- [2.2. Flujo de una petición](#22-flujo-de-una-petición)
- [2.3. Pipeline de `Program.cs`](#23-pipeline-de-programcs)
- [2.4. Responsabilidades](#24-responsabilidades)

---

## 2.1. Capas

```text
Controllers → Services → Repositories → AppDbContext → PostgreSQL
                 │            │
                 └── Errors   └── Models
```

| Carpeta | Contenido |
|---|---|
| `Controllers/` | Traducción HTTP ↔ dominio |
| `Services/` | Reglas de negocio y coordinación de repositorios |
| `Repositories/` | Acceso a datos con EF Core |
| `Data/` | `AppDbContext` y configuración del modelo |
| `Models/` | Entidades de dominio |
| `Dtos/` | Contratos de entrada y salida |
| `Mappers/` | Conversión entidad ↔ DTO |
| `Validators/` | Reglas con FluentValidation |
| `Errors/` | Errores de dominio y traducción a HTTP |
| `Middleware/` | Manejo global de excepciones |
| `Infrastructures/` | Registro de DI, Serilog, Swagger y base de datos |

La dependencia siempre apunta hacia dentro: los controladores conocen los
servicios, pero los servicios no conocen HTTP. Los errores de dominio no saben
qué código de estado les corresponde; eso se decide en `Errors/`.

## 2.2. Flujo de una petición

1. El middleware global de excepciones envuelve todo el pipeline.
2. Serilog registra la petición y su duración.
3. El enrutado selecciona la acción del controlador.
4. `[ApiController]` valida el modelo; si falla, devuelve 400 con
   `ValidationProblemDetails`.
5. El servicio ejecuta la regla de negocio y devuelve
   `Result<T, DomainError>`.
6. `ToHttpResult` traduce el resultado a 200/201/204 o al error correspondiente.
7. La respuesta se serializa con `System.Text.Json`.

## 2.3. Pipeline de `Program.cs`

```csharp
var builder = WebApplication.CreateSlimBuilder(args);

builder.Host.UseSerilog(Log.Logger);

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddRepositoriesConfiguration();
builder.Services.AddServicesConfiguration();
builder.Services.AddValidatorsConfiguration();
builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

await app.Services.InitializeDatabaseAsync();

app.Run();
```

El orden importa: el manejador de excepciones va **el primero** para capturar
también los fallos de los middleware posteriores.

## 2.4. Responsabilidades

- **Controller**: enlaza parámetros, devuelve códigos y tipos documentados.
- **Service**: valida reglas de negocio, decide errores y usa el repositorio.
- **Repository**: consultas con `AsNoTracking` e `Include`; no decide errores.
- **AppDbContext**: mapeo, relaciones, borrado lógico y datos semilla.
- **Middleware**: convierte excepciones no controladas en `ProblemDetails`.
