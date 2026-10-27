using System.Text.Json.Serialization;
using Funko_Api.Dto;
using Funko_Api.Infrastructure;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddOpenApi();

var app = builder.Build();

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
