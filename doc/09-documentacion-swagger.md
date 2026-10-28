# 9. Documentación con Swagger

## Índice

- [9.1. Swashbuckle](#91-swashbuckle)
- [9.2. Comentarios XML](#92-comentarios-xml)
- [9.3. Interfaz](#93-interfaz)
- [9.4. Qué se documenta](#94-qué-se-documenta)

---

## 9.1. Swashbuckle

Se usa **Swashbuckle.AspNetCore 10.2.3**, la misma librería que el profesor, en
lugar de `AddOpenApi`/`MapOpenApi`.

`Infrastructures/SwaggerConfig.cs`:

```csharp
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
```

Con Swashbuckle 10 (`Microsoft.OpenApi` v2), `OpenApiInfo` está en
`Microsoft.OpenApi`, no en `Microsoft.OpenApi.Models`.

En `Program.cs`, solo en desarrollo:

```csharp
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Funko API v1"));
}
```

## 9.2. Comentarios XML

El proyecto activa `GenerateDocumentationFile`, así que todo miembro público
tiene `<summary>`, `<param>` y `<returns>`. El build está en
`TreatWarningsAsErrors`: un comentario que falte (CS1591) rompe la compilación.

El XML generado (`Funko-Api.xml`) se copia a `bin/` y Swashbuckle lo lee desde
`AppContext.BaseDirectory`.

## 9.3. Interfaz

| Recurso | URL |
|---|---|
| Swagger UI | `http://localhost:5052/swagger` |
| Documento OpenAPI | `http://localhost:5052/swagger/v1/swagger.json` |

Comprobación rápida:

```powershell
Invoke-WebRequest http://localhost:5052/swagger/v1/swagger.json -UseBasicParsing
```

## 9.4. Qué se documenta

- Título, versión y descripción de la API.
- Cada endpoint con su `summary`, parámetros y códigos de respuesta
  (`[ProducesResponseType]`).
- Esquemas de los DTOs de entrada y salida.
- Los errores aparecen como `ProblemDetails` con su content type
  `application/problem+json`.

`Funko-Api.http` sirve como alternativa para probar los endpoints sin abrir la
UI.
