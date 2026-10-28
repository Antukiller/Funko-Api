# 8. Testing

## Índice

- [8.1. Estructura](#81-estructura)
- [8.2. Categorías y filtros](#82-categorías-y-filtros)
- [8.3. Tests unitarios](#83-tests-unitarios)
- [8.4. Tests de integración](#84-tests-de-integración)
- [8.5. Convenciones](#85-convenciones)

---

## 8.1. Estructura

```text
Funko-Api.Test/
├── Integration/
│   ├── FunkosEndpointsPostgreTests.cs      # WebApplicationFactory + PostgreSQL
│   └── TestContainers/
│       └── FunkoRepositoryPostgreTests.cs  # repositorio contra contenedor
├── Unit/
│   ├── Middleware/GlobalExceptionHandlerTests.cs
│   ├── Repositories/CategoriaRepositoryTests.cs
│   ├── Repositories/FunkoRepositoryTests.cs
│   ├── Services/CategoriaServiceTests.cs
│   ├── Services/FunkoServiceTests.cs
│   ├── Validators/CreateFunkoValidatorTests.cs
│   └── Validators/UpdateFunkoValidatorTests.cs
├── TestCategories.cs
└── Funko-Api.Test.csproj
```

Total actual: **90 tests** (70 unitarios + 20 de integración).

## 8.2. Categorías y filtros

`TestCategories.cs` centraliza los nombres:

```csharp
public static class TestCategories {
    public const string Unit = "Unit";
    public const string Integration = "Integration";
}
```

Cada fixture lleva `[Category(TestCategories.Unit)]` o
`[Category(TestCategories.Integration)]`.

```powershell
dotnet test --filter "FullyQualifiedName~Unit"         # sin Docker
dotnet test --filter "FullyQualifiedName~Integration"  # requiere Docker
dotnet test                                            # todo
```

## 8.3. Tests unitarios

- **Servicios**: repositorio simulado con `Moq`.
- **Validadores**: entradas válidas e inválidas con FluentValidation.
- **Repositorios**: EF Core en memoria con SQLite.
- **Middleware**: `DefaultHttpContext` y comprobación del `ProblemDetails`.

Se usa `FluentAssertions` para aserciones legibles:

```csharp
result.IsFailure.Should().BeTrue();
result.Error.Should().BeOfType<NotFound>();
```

## 8.4. Tests de integración

- **`FunkosEndpointsPostgreTests`**: `WebApplicationFactory<Program>` contra un
  PostgreSQL real; recorre 200/201/204/400/404/409/422 y comprueba el content
  type (`application/json` en éxitos, `application/problem+json` en errores).
- **`FunkoRepositoryPostgreTests`**: Testcontainers arranca PostgreSQL y prueba
  el repositorio de verdad (incluido el borrado lógico).

Docker debe estar arrancado o los tests de integración fallan en
`OneTimeSetUp`.

## 8.5. Convenciones

- Nombre: `Metodo_Escenario_ResultadoEsperado`.
- Un test comprueba un comportamiento.
- Los fixtures que comparten contenedor usan `[SetUpFixture]` o
  `IClassFixture`.
- `dotnet test` debe quedar en verde antes de cada commit.
