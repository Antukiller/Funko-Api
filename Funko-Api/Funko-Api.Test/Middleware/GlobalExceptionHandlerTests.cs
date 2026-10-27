using System.Text.Json;
using FluentAssertions;
using Funko_Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Funko_Api.Test.Middleware;

[TestFixture]
public class GlobalExceptionHandlerTests {
    [Test]
    public async Task InvokeAsync_SinExcepcion_DejaPasarLaPeticion() {
        var context = CrearContexto();
        var nextLlamado = false;
        var handler = new GlobalExceptionHandler(_ => {
            nextLlamado = true;
            return Task.CompletedTask;
        }, NullLogger<GlobalExceptionHandler>.Instance);

        await handler.InvokeAsync(context);

        nextLlamado.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        context.Response.Body.Length.Should().Be(0);
    }

    [Test]
    public async Task InvokeAsync_CuandoExcepcionInesperada_DevuelveProblemDetailsConTraceId() {
        var context = CrearContexto();
        var handler = CrearHandler(new InvalidOperationException("boom"));

        await handler.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        context.Response.ContentType.Should().Be("application/problem+json");

        var problem = LeerProblem(context);
        problem.GetProperty("status").GetInt32().Should().Be(500);
        problem.GetProperty("title").GetString().Should().Be("Error interno del servidor");
        problem.GetProperty("instance").GetString().Should().Be("/api/funkos");
        problem.GetProperty("traceId").GetString().Should().NotBeNullOrWhiteSpace();
        problem.TryGetProperty("timestamp", out _).Should().BeTrue();
    }

    [Test]
    public async Task InvokeAsync_CuandoDbUpdateException_DevuelveConflict() {
        var context = CrearContexto();
        var handler = CrearHandler(new DbUpdateException("violación de unicidad"));

        await handler.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        LeerProblem(context).GetProperty("title").GetString()
            .Should().Be("Conflicto al guardar en la base de datos");
    }

    private static GlobalExceptionHandler CrearHandler(Exception exception) =>
        new(_ => Task.FromException(exception), NullLogger<GlobalExceptionHandler>.Instance);

    private static DefaultHttpContext CrearContexto() {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/funkos";
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static JsonElement LeerProblem(DefaultHttpContext context) {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var document = JsonDocument.Parse(context.Response.Body);
        return document.RootElement.Clone();
    }
}
