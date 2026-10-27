using System.Text.Json.Serialization;
using Funko_Api.Dto;
using Funko_Api.Infrastructure;
using Funko_Api.Middleware;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
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
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
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
