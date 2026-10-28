using System.Text;
using System.Text.Json.Serialization;
using Funko_Api.Dtos;
using Funko_Api.Infrastructures;
using Funko_Api.Middleware;
using Microsoft.AspNetCore.Mvc;
using Serilog;

Console.OutputEncoding = Encoding.UTF8;

Log.Logger = SerilogConfig.Configure().CreateLogger();

var builder = WebApplication.CreateSlimBuilder(args);

builder.Host.UseSerilog(Log.Logger);

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddRepositoriesConfiguration();
builder.Services.AddServicesConfiguration();
builder.Services.AddValidatorsConfiguration();
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.Configure<ApiBehaviorOptions>(options => {
    options.InvalidModelStateResponseFactory = context => {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        var problem = new ValidationProblemDetails(errors) {
            Status = StatusCodes.Status400BadRequest,
            Title = "Errores de validación",
            Instance = context.HttpContext.Request.Path
        };
        problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        return new BadRequestObjectResult(problem);
    };
});
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Funko API v1"));
}

app.MapControllers();

await app.Services.InitializeDatabaseAsync();

app.Run();

[JsonSerializable(typeof(FunkoDto))]
[JsonSerializable(typeof(IReadOnlyList<FunkoDto>))]
[JsonSerializable(typeof(CreateFunkoDto))]
[JsonSerializable(typeof(UpdateFunkoDto))]
[JsonSerializable(typeof(CategoriaDto))]
[JsonSerializable(typeof(IReadOnlyList<CategoriaDto>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext {
}

/// <summary>
/// Punto de entrada público para que los tests de integración puedan usar
/// <c>WebApplicationFactory&lt;Program&gt;</c>.
/// </summary>
public partial class Program;
