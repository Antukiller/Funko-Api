using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace Funko_Api.Test.Integration;

/// <summary>
/// Tests de integración de los endpoints de Funkos sobre la aplicación real
/// (<c>WebApplicationFactory&lt;Program&gt;</c>) y un PostgreSQL de Testcontainers.
/// Arranca la API como en producción: aplica las migraciones al iniciar.
/// </summary>
[Category(TestCategories.Integration)]
[TestFixture]
public sealed class FunkosEndpointsPostgreTests {
    private PostgreSqlContainer _container = null!;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp() {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("funkos_api_test")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        await _container.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, config) => {
                config.AddInMemoryCollection(new Dictionary<string, string?> {
                    ["ConnectionStrings:DefaultConnection"] = _container.GetConnectionString()
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown() {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _container.DisposeAsync();
    }

    [Test]
    public async Task GetAll_DevuelveLasSemillas() {
        var response = await _client.GetAsync("/api/funkos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var nombres = body.EnumerateArray()
            .Select(funko => funko.GetProperty("name").GetString())
            .ToList();

        nombres.Should().Contain(new[] {
            "Naruto Uzumaki", "Sailor Moon", "Lara Croft", "Sonic", "Wonder Woman"
        });
    }

    [Test]
    public async Task GetById_Existente_DevuelveFunkoConCategoria() {
        var response = await _client.GetAsync("/api/funkos/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("id").GetInt64().Should().Be(1);
        body.GetProperty("name").GetString().Should().Be("Naruto Uzumaki");
        body.GetProperty("categoriaName").GetString().Should().Be("Anime");
    }

    [Test]
    public async Task GetById_Inexistente_Devuelve404ConProblemDetails() {
        var response = await _client.GetAsync("/api/funkos/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("status").GetInt32().Should().Be(404);
        body.GetProperty("title").GetString().Should().Be("Recurso no encontrado");
        body.GetProperty("detail").GetString().Should().Contain("9999");
    }

    [Test]
    public async Task Create_Valido_Devuelve201ConLocation() {
        var nombre = NombreUnico();

        var response = await _client.PostAsJsonAsync("/api/funkos", new {
            name = nombre,
            price = 19.99m,
            stock = 3,
            categoriaName = "Anime"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetInt64();
        id.Should().BeGreaterThan(0);
        body.GetProperty("name").GetString().Should().Be(nombre);

        var creado = await _client.GetAsync($"/api/funkos/{id}");
        creado.StatusCode.Should().Be(HttpStatusCode.OK);

        await _client.DeleteAsync($"/api/funkos/{id}");
    }

    [Test]
    public async Task Create_NombreDuplicado_Devuelve409() {
        var response = await _client.PostAsJsonAsync("/api/funkos", new {
            name = "Naruto Uzumaki",
            price = 19.99m,
            stock = 3,
            categoriaName = "Anime"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("status").GetInt32().Should().Be(409);
        body.GetProperty("title").GetString().Should().Be("Conflicto");
        body.GetProperty("detail").GetString().Should().Contain("Naruto Uzumaki");
    }

    [Test]
    public async Task Create_Invalido_Devuelve400ConErroresPorCampo() {
        var response = await _client.PostAsJsonAsync("/api/funkos", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("status").GetInt32().Should().Be(400);
        body.TryGetProperty("traceId", out _).Should().BeTrue();

        var errors = body.GetProperty("errors");
        errors.TryGetProperty("Name", out var nombre).Should().BeTrue();
        nombre[0].GetString().Should().Be("El nombre es obligatorio.");
        errors.TryGetProperty("Price", out _).Should().BeTrue();
        errors.TryGetProperty("CategoriaName", out _).Should().BeTrue();
    }

    [Test]
    public async Task Delete_Existente_Devuelve204YDejaDeVerse() {
        var creado = await CrearFunkoAsync(NombreUnico());
        var id = creado.GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/funkos/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var consulta = await _client.GetAsync($"/api/funkos/{id}");
        consulta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task PatchStock_Valido_DevuelveElNuevoStock() {
        var creado = await CrearFunkoAsync(NombreUnico(), stock: 2);
        var id = creado.GetProperty("id").GetInt64();

        var response = await _client.PatchAsJsonAsync($"/api/funkos/{id}/stock", 3);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("stock").GetInt32().Should().Be(5);

        await _client.DeleteAsync($"/api/funkos/{id}");
    }

    [Test]
    public async Task PatchStock_Insuficiente_Devuelve422() {
        var creado = await CrearFunkoAsync(NombreUnico(), stock: 2);
        var id = creado.GetProperty("id").GetInt64();

        var response = await _client.PatchAsJsonAsync($"/api/funkos/{id}/stock", -5);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("status").GetInt32().Should().Be(422);
        body.GetProperty("title").GetString().Should().Be("Regla de negocio incumplida");
        body.GetProperty("detail").GetString().Should().Contain("Stock insuficiente");

        await _client.DeleteAsync($"/api/funkos/{id}");
    }

    private async Task<JsonElement> CrearFunkoAsync(string nombre, int stock = 3) {
        var response = await _client.PostAsJsonAsync("/api/funkos", new {
            name = nombre,
            price = 19.99m,
            stock,
            categoriaName = "Anime"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    private static string NombreUnico() => $"Funko {Guid.NewGuid():N}";
}
