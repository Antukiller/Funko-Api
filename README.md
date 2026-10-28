# Funko API 🧸

[![.NET](https://img.shields.io/badge/.NET-10-blue)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10-blue)](https://dotnet.microsoft.com/en-us/apps/aspnet)
[![C#](https://img.shields.io/badge/C%23-14-blue)](https://docs.microsoft.com/dotnet/csharp/)
[![EF Core](https://img.shields.io/badge/EF%20Core-10-blue)](https://docs.microsoft.com/ef/core/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-blue)](https://www.postgresql.org/)
[![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-green)](https://swagger.io/)
[![Tests](https://img.shields.io/badge/tests-90%20passing-brightgreen)](#-testing)

**API RESTful para gestionar un catálogo de Funkos**, desarrollada como práctica 3
de la asignatura *Desarrollo Web en Entornos Servidor* (2º de DAW).

Permite consultar, buscar, crear, actualizar y borrar funkos (borrado lógico) y
consultar sus categorías, con persistencia real en PostgreSQL mediante Entity
Framework Core, errores de negocio con el patrón Result y documentación viva en
Swagger.

## 🧭 Tabla de contenidos

- [📋 Descripción](#-descripción)
- [✨ Características](#-características)
- [🛠️ Tecnologías](#-tecnologías)
- [🚀 Inicio rápido](#-inicio-rápido)
- [🔌 Endpoints](#-endpoints)
- [🧱 Arquitectura](#-arquitectura)
- [🗂️ Estructura del proyecto](#-estructura-del-proyecto)
- [🧠 Decisiones de diseño](#-decisiones-de-diseño)
- [🗄️ Modelo de datos](#-modelo-de-datos)
- [⚙️ Configuración](#-configuración)
- [🧪 Testing](#-testing)
- [📖 Documentación](#-documentación)
- [📌 Estado de la práctica](#-estado-de-la-práctica)
- [🐳 Docker](#-docker)
- [📄 Licencia y autor](#-licencia-y-autor)

## 📋 Descripción

El recurso principal es el **Funko**, un muñeco de colección que pertenece a una
**Categoría**. La API expone un CRUD completo más una búsqueda por nombre, aplica
validaciones en dos niveles, devuelve errores de negocio siguiendo el **patrón
Result** y los traduce a HTTP con **Problem Details** (RFC 7807).

- **Servidor** (este repositorio): API REST en ASP.NET Core, **sin estado**. Cada
  petición es independiente; toda la información viaja en la URL, la cabecera y
  el cuerpo JSON.
- **Clientes**: cualquier cliente HTTP. Durante el desarrollo se usa
  `Funko-Api.http`, Swagger UI, `Invoke-RestMethod` (PowerShell) o Bruno/Postman.
- **Contrato**: JSON en peticiones y respuestas; errores en formato
  [Problem Details](https://datatracker.ietf.org/doc/html/rfc7807).

## ✨ Características

- 🧸 **CRUD de Funkos**: listar, consultar, crear, actualizar, parchear el precio
  y el stock, y borrar.
- 🔎 **Búsqueda por nombre**: `GET /api/funkos/buscar?nombre=...`, sin distinguir
  mayúsculas.
- 🗂️ **Categorías**: listado y consulta por id, con su propio controlador y
  servicio.
- 🗑️ **Borrado lógico**: `IsDeleted` + `HasQueryFilter`; los borrados no aparecen
  en ninguna consulta.
- 🧭 **Patrón Result**: `Result<T, DomainError>` en los servicios, con traducción
  centralizada a HTTP (`404`, `400`, `409`, `422`).
- ⚠️ **Problem Details**: validación automática, errores de dominio y middleware
  global de excepciones con `traceId`.
- ✅ **Validación en dos niveles**: Data Annotations en los DTOs y FluentValidation
  en DTOs y dominio, con mensajes en español.
- 🎯 **Retorno tipado**: `ActionResult<T>` y `[ProducesResponseType]` en cada
  endpoint.
- 🐘 **Persistencia real**: EF Core 10 + PostgreSQL 17, migraciones aplicadas al
  arrancar y datos semilla.
- 📝 **Logging estructurado**: Serilog a consola y fichero diario, con una línea
  por petición.
- 📖 **Swagger**: Swashbuckle con los comentarios XML del código.
- 🧪 **Testing**: 90 tests (70 unitarios y 20 de integración con Testcontainers).

## 🛠️ Tecnologías

| Tecnología | Versión | Uso |
|---|---|---|
| .NET / ASP.NET Core | 10.0 | Framework y servidor web |
| C# | 14 | Lenguaje (primary constructors, records) |
| Entity Framework Core | 10.0.13 | ORM y migraciones |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 | Proveedor de PostgreSQL |
| PostgreSQL | 17 | Base de datos relacional |
| FluentValidation | 12.2.0 | Validación de DTOs y dominio |
| CSharpFunctionalExtensions | 3.7.0 | Patrón Result (`Result<T, DomainError>`) |
| Serilog | 10.0.0 | Logging estructurado: consola + fichero diario |
| Swashbuckle.AspNetCore | 10.2.3 | Swagger UI y OpenAPI con comentarios XML |
| NUnit + Moq + FluentAssertions | 4.3.2 / 4.20.72 / 8.11.0 | Testing |
| Testcontainers | 4.3.0 | Tests de integración con PostgreSQL real |
| Coverlet | 6.0.4 | Cobertura de código |

Las versiones se gestionan de forma centralizada con **Central Package
Management** en `Directory.Packages.props`.

## 🚀 Inicio rápido

### Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- Docker (para PostgreSQL en desarrollo y para los tests de integración)
- `dotnet-ef` como herramienta global: `dotnet tool install --global dotnet-ef`

### 1. Base de datos

```powershell
docker run -d --name funko-postgres `
  -e POSTGRES_DB=funkos -e POSTGRES_USER=funkos -e POSTGRES_PASSWORD=funkos `
  -p 5432:5432 postgres:17-alpine
```

### 2. Arrancar la API

```powershell
dotnet run --project Funko-Api/Funko-Api/Funko-Api.csproj
```

Al arrancar, `InitializeDatabaseAsync()` aplica las migraciones pendientes y la
API queda escuchando en `http://localhost:5052`. Las tablas se crean solas.

### 3. Swagger y `.http`

- Swagger UI: `http://localhost:5052/swagger`
- Documento OpenAPI: `http://localhost:5052/swagger/v1/swagger.json`
- Peticiones de ejemplo: `Funko-Api/Funko-Api/Funko-Api.http`

### 4. Tests

```powershell
# Toda la suite (necesita Docker para integración)
dotnet test

# Solo lo que no necesita Docker
dotnet test --filter "FullyQualifiedName~Unit"

# Solo integración (arranca contenedores)
dotnet test --filter "FullyQualifiedName~Integration"
```

## 🔌 Endpoints

URL base en desarrollo: `http://localhost:5052`

### Funkos

| Método | Ruta | Descripción | Éxito | Errores |
|---|---|---|---|---|
| `GET` | `/api/funkos` | Lista todos los funkos activos | `200` | — |
| `GET` | `/api/funkos/buscar?nombre={texto}` | Busca por nombre (contiene, sin distinguir mayúsculas) | `200` | `400` |
| `GET` | `/api/funkos/{id}` | Obtiene un funko por id | `200` | `404` |
| `POST` | `/api/funkos` | Crea un funko | `201` + `Location` | `400`, `404`, `409` |
| `PUT` | `/api/funkos/{id}` | Actualiza un funko completo | `200` | `400`, `404`, `409` |
| `PATCH` | `/api/funkos/{id}/precio` | Actualiza solo el precio | `200` | `400`, `404` |
| `PATCH` | `/api/funkos/{id}/stock` | Ajusta el stock: positivo repone, negativo descuenta | `200` | `400`, `404`, `422` |
| `DELETE` | `/api/funkos/{id}` | Borrado lógico | `204` | `404` |

### Categorías

| Método | Ruta | Descripción | Éxito | Errores |
|---|---|---|---|---|
| `GET` | `/api/categorias` | Lista todas las categorías | `200` | — |
| `GET` | `/api/categorias/{id}` | Obtiene una categoría por id | `200` | `404` |

Significado de los errores:

| Código | Cuándo |
|---|---|
| `400` | Datos inválidos o parámetros incorrectos |
| `404` | El recurso no existe |
| `409` | Nombre duplicado |
| `422` | Regla de negocio incumplida (stock insuficiente) |
| `500` | Error no controlado |

### Ejemplos

```powershell
# Listar
Invoke-RestMethod http://localhost:5052/api/funkos

# Buscar (insensible a mayúsculas)
Invoke-RestMethod "http://localhost:5052/api/funkos/buscar?nombre=naruto"

# Crear
$body = @{ name = "Sasuke Uchiha"; price = 17.50; stock = 4; categoriaName = "Anime" } | ConvertTo-Json
Invoke-RestMethod http://localhost:5052/api/funkos -Method Post -Body $body -ContentType "application/json"

# Actualizar solo el precio (el cuerpo es un número JSON)
Invoke-RestMethod http://localhost:5052/api/funkos/6/precio -Method Patch -Body "19.99" -ContentType "application/json"

# Ajustar stock: -2 descuenta, +5 repone
Invoke-RestMethod http://localhost:5052/api/funkos/6/stock -Method Patch -Body "-2" -ContentType "application/json"

# Borrar
Invoke-RestMethod http://localhost:5052/api/funkos/6 -Method Delete
```

## 🧱 Arquitectura

Arquitectura en capas: **Controller → Service → Repository → EF Core**.

```text
Controllers → Services → Repositories → AppDbContext → PostgreSQL
                 │            │
                 └── Errors   └── Models
```

| Capa | Responsabilidad |
|---|---|
| `Controllers/` | Traduce HTTP ↔ dominio y documenta respuestas |
| `Services/` | Reglas de negocio y coordinación de repositorios |
| `Repositories/` | Acceso a datos con EF Core (`AsNoTracking` + `Include`) |
| `Data/` | `AppDbContext`, configuración del modelo y semilla |
| `Errors/` | Errores de dominio y traducción única a HTTP |
| `Infrastructures/` | Registro de DI, Serilog, Swagger y base de datos |
| `Middleware/` | Manejo global de excepciones |

Flujo de una petición:

1. `GlobalExceptionHandler` envuelve todo el pipeline.
2. Serilog registra la petición y su duración.
3. El enrutado selecciona la acción del controlador.
4. `[ApiController]` valida el modelo; si falla devuelve `400` con
   `ValidationProblemDetails`.
5. El servicio ejecuta la regla de negocio y devuelve
   `Result<T, DomainError>`.
6. `ToHttpResult` traduce el resultado a `200`/`201`/`204` o al error
   correspondiente.
7. La respuesta se serializa con `System.Text.Json`.

Pipeline en `Program.cs`:

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

## 🗂️ Estructura del proyecto

```text
Funko-Api/                          # raíz del repositorio
├── .editorconfig
├── .gitignore
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
├── README.md
├── doc/                            # guías numeradas (01–10)
└── Funko-Api/                      # solución
    ├── Funko-Api.slnx
    ├── Funko-Api/                  # API
    │   ├── Controllers/            # endpoints (Funkos, Categorias)
    │   ├── Data/                   # AppDbContext
    │   ├── Dtos/                   # CreateFunkoDto, UpdateFunkoDto, FunkoDto...
    │   ├── Errors/                 # DomainError, NotFound, ValidationError...
    │   ├── Infrastructures/        # DI, DatabaseConfig, Serilog, Swagger
    │   ├── Mappers/                # métodos de extensión entidad <-> DTO
    │   ├── Migrations/             # migraciones de EF Core
    │   ├── Models/                 # Funko, Categoria
    │   ├── Repositories/           # IFunkoRepository, ICategoriaRepository
    │   ├── Services/               # IFunkoService, ICategoriaService
    │   ├── Validators/             # validadores FluentValidation
    │   ├── Middleware/             # GlobalExceptionHandler
    │   ├── Funko-Api.http          # peticiones de ejemplo
    │   ├── Program.cs
    │   └── appsettings*.json
    └── Funko-Api.Test/
        ├── Integration/            # arrancan la API o Postgres en Docker
        ├── Unit/                   # Middleware, Repositories, Services, Validators
        └── TestCategories.cs       # categorías Unit / Integration
```

## 🧠 Decisiones de diseño

### PostgreSQL en desarrollo y en producción

Se descartó SQLite en desarrollo. Una migración de EF Core **no es portable entre
proveedores** (tipos y SQL distintos), así que mantener dos proveedores obliga a
duplicar migraciones o a usar `EnsureCreated`, que no evoluciona el esquema. Con
PostgreSQL en ambos entornos hay un único juego de migraciones, los tests de
integración prueban el mismo motor y la fase 12 (Docker) no requiere cambios.
El coste es tener Docker arrancado para desarrollar.

### Migraciones en lugar de `EnsureCreated`

`EnsureCreated()` crea el esquema una vez y no registra nada: si la base ya
existe, ignora los cambios del modelo en silencio. Las migraciones son la
historia versionada del esquema, permiten aplicar solo lo pendiente sobre una
base con datos y quedan registradas en `__EFMigrationsHistory`.

### Minimal API vs. controladores

La fase 3 (Minimal API con `List<Funko>`) no se mantiene; se comparó y se optó
por controladores para el proyecto final:

| Criterio | Minimal API | Controladores |
|---|---|---|
| Cantidad de código inicial | Muy poco, todo en `Program.cs` | Más ceremonia (una clase por recurso) |
| Organización al crecer | Se dispersa en métodos y grupos | Cada recurso en su clase |
| Model binding y validación | Manual o con filtros limitados | `[ApiController]`, `[FromBody]`, `[FromQuery]` |
| Problem Details | Hay que construirlo | Integrado con validación automática |
| Documentación XML / Swagger | Requiere configuración extra | Integrada |
| Testing | Más difícil de aislar | Se instancia el controlador o `WebApplicationFactory` |

Con 10 endpoints, DTOs y validadores, los controladores ofrecen una estructura
más clara.

### Patrón Result y Problem Details

Los servicios devuelven `Result<T, DomainError>` de `CSharpFunctionalExtensions`.
El dominio no conoce HTTP: `NotFound`, `ValidationError`, `ConflictError` y
`BusinessRuleError` se traducen a `404`, `400`, `409` y `422` en un único punto,
`Errors/DomainErrorExtension.cs`. Las reglas de negocio se pueden testear sin
levantar el servidor.

Las excepciones no controladas no llegan al cliente:
`Middleware/GlobalExceptionHandler.cs` las captura, las registra y responde con
`ProblemDetails` (`application/problem+json`) incluyendo `traceId` para
correlacionar la respuesta con la traza del servidor.

### DTO sin `IsDeleted`

`FunkoDto` no expone `IsDeleted`: es un detalle de persistencia. El borrado
lógico se aplica con `HasQueryFilter`, así que un funko borrado simplemente no
existe para el cliente (`404`).

### Sin `[Produces]` a nivel de clase

Los endpoints documentan sus códigos con `[ProducesResponseType]`, pero no se
pone `[Produces("application/json")]` en el controlador: ese atributo forzaría
también el content type de los errores a `application/json` y rompería
`application/problem+json`. Swagger deduce el `application/json` de los tipos
declarados.

### FK explícita

`Funko` tiene `CategoriaId` como propiedad real y la relación se configura con
`HasForeignKey(x => x.CategoriaId)`. Evita una *shadow property* y deja la FK
visible en el modelo.

### Logging con Serilog

Serilog sustituye al logger por defecto y escribe en dos sitios: consola (formato
compacto para desarrollo) y `logs/funko-AAAAMMDD.log` (rotación diaria, 7 días).
Los niveles de `Microsoft` y EF Core se suben a `Warning` para que el log de
negocio no se ahogue entre trazas del framework. `UseSerilogRequestLogging()`
deja una línea por petición con método, ruta, código de estado y milisegundos.

### Configuración de build centralizada

`global.json` fija el SDK 10.0.101 con `rollForward: latestFeature`,
`Directory.Build.props` aplica `LangVersion 14`, `Nullable`, `ImplicitUsings` y
`TreatWarningsAsErrors` a todos los proyectos, y `Directory.Packages.props`
centraliza las versiones (CPM): los `.csproj` no llevan `Version`.
`.editorconfig` fija LF, 4 espacios y el estilo de C#.

### Inyección de dependencias explícita

Cada aspecto tiene su `Infrastructures/*Config.cs` con un extension method:
`AddDatabaseConfiguration`, `AddRepositoriesConfiguration`,
`AddServicesConfiguration` y `AddValidatorsConfiguration`. Se descartó el escaneo
por reflexión con Scrutor y las interfaces marcador porque el registro explícito
se lee de un vistazo y falla en el arranque, no en la primera petición.

### Datos semilla con `HasData`

Los seeds viajan dentro de la migración, así que cualquier entorno arranca con
las 3 categorías y los 5 funkos. Los ids son explícitos y la migración incluye un
`setval` para reposicionar las secuencias de identidad; sin él, el primer `POST`
chocaría contra la clave primaria.

## 🗄️ Modelo de datos

### `Funko` — cada propiedad justificada

| Propiedad | Tipo | Justificación |
|---|---|---|
| `Id` | `long` | Identificador único. `bigint` con `IDENTITY` en PostgreSQL; `long` evita quedarse corto y es coherente con las FKs. |
| `Name` | `string` | Nombre comercial del funko. Obligatorio: sin nombre el recurso no tiene sentido. En BD y en el validador está limitado a 200 caracteres. |
| `Price` | `decimal` | Precio en euros. `decimal` y no `double` para no arrastrar errores de redondeo binario; en BD `numeric(20,2)`. |
| `Stock` | `int` | Unidades disponibles. `int` es más que suficiente para una tienda de coleccionables y no admite negativos (lo valida FluentValidation). |
| `CategoriaId` | `long` | FK explícita a la categoría. |
| `Categoria` | `Categoria` | Relación **1:N** obligatoria: todo funko pertenece a una categoría. Se configura con `HasOne/WithMany` y `DeleteBehavior.Restrict` para no borrar categorías en uso. |
| `CreatedAt` | `DateTime` | Fecha de alta (auditoría). Se guarda en **UTC** (`DateTime.UtcNow`); Npgsql lo mapea a `timestamptz`. |
| `UpdatedAt` | `DateTime` | Fecha de la última modificación. También UTC, para poder ordenar y auditar. |
| `IsDeleted` | `bool` | Borrado lógico. `HasQueryFilter(x => !x.IsDeleted)` hace que EF excluya los borrados sin que el resto del código lo recuerde. Valor por defecto `false`. |

### `Categoria`

| Propiedad | Tipo | Justificación |
|---|---|---|
| `Id` | `long` | Identificador único de la categoría. |
| `Name` | `string` | Nombre visible (Anime, Videojuegos, Cómics...). Obligatorio, máximo 100 caracteres. |
| `Funkos` | `ICollection<Funko>` | Navegación inversa de la relación 1:N. |

**Relación:** una categoría tiene muchos funkos; un funko tiene exactamente una
categoría. La FK es `CategoriaId` y el borrado en cascada **no** está permitido
(`Restrict`): primero se reasignan o borran los funkos.

### Migraciones

| Migración | Cambio |
|---|---|
| `InitialCreate` | Tablas, relaciones, índices, semilla y `setval` |
| `AmpliarLongitudNombreFunko` | Nombre de 100 a 200 caracteres |

```powershell
# Crear una migración tras cambiar el modelo
dotnet ef migrations add NombreDescritivo --project Funko-Api/Funko-Api/Funko-Api.csproj

# Aplicarlas sin arrancar la API
dotnet ef database update --project Funko-Api/Funko-Api/Funko-Api.csproj
```

## ⚙️ Configuración

La cadena de conexión se lee de `ConnectionStrings:DefaultConnection`.

| Entorno | Fichero | Valor de ejemplo |
|---|---|---|
| Desarrollo | `appsettings.Development.json` | `Host=localhost;Port=5432;Database=funkos;Username=funkos;Password=funkos` |
| Producción | `appsettings.Production.json` | Igual, sobrescribible por entorno |

En producción los secretos **no** deben ir en el repositorio. Se sobrescriben con
variables de entorno (doble guion bajo para anidar claves):

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=mi-servidor;Database=funkos;Username=...;Password=..."
```

Si la cadena no existe, `AddDatabaseConfiguration` lanza una excepción al arrancar
(*fail-fast*): es mejor fallar al inicio que descubrirlo en la primera petición.

### Serilog

- Consola: formato compacto para desarrollo.
- Fichero: `logs/funko-AAAAMMDD.log`, rotación diaria y 7 días de retención.
- `UseSerilogRequestLogging()` registra método, ruta, estado y duración.

## 🧪 Testing

| Tipo | Cantidad | Tecnología |
|---|---|---|
| Unitarios (servicios, validadores, middleware, repos con SQLite en memoria) | 70 | NUnit, Moq, FluentAssertions |
| Integración (repositorio y API real contra PostgreSQL) | 20 | Testcontainers, `WebApplicationFactory<Program>` |

Qué cubre cada bloque:

- `FunkoServiceTests` y `CategoriaServiceTests`: reglas y propagación de errores.
- `CreateFunkoValidatorTests` / `UpdateFunkoValidatorTests`: validaciones de DTO.
- `GlobalExceptionHandlerTests`: Problem Details con `traceId` y mapeo de
  excepciones.
- `FunkoRepositoryTests`: consultas, búsqueda y borrado lógico sobre SQLite.
- `FunkoRepositoryPostgreTests`: mapeo y migraciones contra PostgreSQL en
  contenedor.
- `FunkosEndpointsPostgreTests`: endpoints reales sobre
  `WebApplicationFactory<Program>` y PostgreSQL de Testcontainers
  (200, 201, 204, 400, 404, 409 y 422).

Los tests de integración arrancan el host real con
`WebApplicationFactory<Program>` —de ahí el `public partial class Program;` al
final de `Program.cs`— y aplican las migraciones al iniciar, igual que en
producción.

## 📖 Documentación

Guías numeradas en `doc/`, con el mismo orden en que se construyó el proyecto:

| Guía | Contenido |
|---|---|
| [`01-configuracion-proyectos-dotnet.md`](doc/01-configuracion-proyectos-dotnet.md) | Solución, CPM, SDK y estilo |
| [`02-arquitectura-pipeline-http.md`](doc/02-arquitectura-pipeline-http.md) | Capas, flujo y pipeline |
| [`03-inyeccion-dependencias.md`](doc/03-inyeccion-dependencias.md) | Registro explícito y ciclos de vida |
| [`04-controladores-rest.md`](doc/04-controladores-rest.md) | Endpoints, retorno tipado y `.http` |
| [`05-validacion-dtos-mapeo.md`](doc/05-validacion-dtos-mapeo.md) | DTOs, validación y mappers |
| [`06-patron-result-errores.md`](doc/06-patron-result-errores.md) | Result, errores y Problem Details |
| [`07-ef-core-postgresql.md`](doc/07-ef-core-postgresql.md) | Modelo, soft delete y migraciones |
| [`08-testing.md`](doc/08-testing.md) | Unit/Integration, Testcontainers y filtros |
| [`09-documentacion-swagger.md`](doc/09-documentacion-swagger.md) | Swashbuckle y comentarios XML |
| [`10-rubrica.md`](doc/10-rubrica.md) | Checklist completa de la rúbrica |

Además, la API se documenta sola en `/swagger`, generada con Swashbuckle a partir
de los comentarios XML.

## 📌 Estado de la práctica

| Fase | Estado |
|---|---|
| 1. Define tu recurso | ✅ |
| 2. Diseña la API en papel | ✅ |
| 3. Minimal API | 🟡 documentada como comparación, no se mantiene |
| 4. Controladores | ✅ |
| 5. Inyección de dependencias | ✅ |
| 6. Patrón Result | ✅ |
| 7. DTOs y validaciones | ✅ |
| 8. Configuración y logs | ✅ |
| 9. Persistencia con EF Core | ✅ |
| 10–12. Seguridad y Docker | ⏳ pendientes |

Resumen de la rúbrica: **59 ✅ / 2 🟡 / 3 ❌** en las fases 1–9 y
**73 ✅ / 4 🟡 / 7 ❌** en total. La checklist completa, con evidencias y
prioridades, está en [`doc/10-rubrica.md`](doc/10-rubrica.md).

## 🐳 Docker

La fase 12 añadirá `Dockerfile` multi-etapa y `docker-compose.yml` para levantar
API + PostgreSQL con `docker compose up`. Mientras tanto, el contenedor de
PostgreSQL de desarrollo se arranca a mano con el comando de *Inicio rápido*.

## 📄 Licencia y autor

Proyecto educativo de **Antukiller** para 2º de DAW.

El código se ha escrito para este proyecto; las ideas, estructura y convenciones
proceden de su temario y de `TiendaDawApi-NetCore`.

<a rel="license" href="https://creativecommons.org/licenses/by-nc-sa/4.0/deed.es"><img alt="Licencia de Creative Commons" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png" /></a>
