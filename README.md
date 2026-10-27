# Funko-Api 🧸

[![.NET](https://img.shields.io/badge/.NET-10-blue)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10-blue)](https://dotnet.microsoft.com/en-us/apps/aspnet)
[![C#](https://img.shields.io/badge/C%23-14-blue)](https://docs.microsoft.com/dotnet/csharp/)
[![EF Core](https://img.shields.io/badge/EF%20Core-10-blue)](https://docs.microsoft.com/ef/core/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-blue)](https://www.postgresql.org/)
[![Tests](https://img.shields.io/badge/tests-81%20passing-brightgreen)](#-testing)

**API RESTful para gestionar un catálogo de Funkos**, desarrollada como práctica 3
de la asignatura *Desarrollo Web en Entornos Servidor* (DAW).

Permite consultar, buscar, crear, actualizar y borrar funkos (borrado lógico) y
consultar sus categorías, con persistencia real en PostgreSQL mediante Entity
Framework Core.

---

## 📋 Descripción

El recurso principal es el **Funko**, un muñeco de colección que pertenece a una
**Categoría**. La API expone un CRUD completo más una búsqueda por nombre, aplica
validaciones con FluentValidation, devuelve errores de negocio siguiendo el
**patrón Result** y traduce los errores a HTTP con Problem Details.

- 🧸 **CRUD de Funkos**: listar, consultar, crear, actualizar, parchear el precio y borrar.
- 🔎 **Búsqueda por nombre**: `GET /api/funkos/buscar?nombre=...` (insensible a mayúsculas).
- 🗂️ **Categorías**: consulta de todas y por id, en su propio controlador y servicio.
- 🗑️ **Borrado lógico**: `IsDeleted` + `HasQueryFilter`, los borrados no aparecen.
- ✅ **Validación**: FluentValidation en DTOs y en el modelo de dominio.
- ⚠️ **Errores de dominio**: `NotFound` y `ValidationError` traducidos a Problem Details.
- 🐘 **Persistencia**: EF Core 10 + PostgreSQL 17 con migraciones aplicadas al arrancar.
- 🧪 **Testing**: 81 tests (unitarios con NUnit + Moq + FluentAssertions, integración con Testcontainers).

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
| NUnit + Moq + FluentAssertions | 4.x / 4.x / 8.11 | Testing |
| Testcontainers | 4.3 | Tests de integración con PostgreSQL real |

## 🧩 Modelo de datos

### `Funko` — cada propiedad justificada

| Propiedad | Tipo | Justificación |
|---|---|---|
| `Id` | `long` | Identificador único. `bigint` con `IDENTITY` en PostgreSQL; `long` evita quedarse corto y es coherente con las FKs. |
| `Name` | `string` | Nombre comercial del funko. Obligatorio: sin nombre el recurso no tiene sentido. En BD y en el validador está limitado a 200 caracteres. |
| `Price` | `decimal` | Precio en euros. `decimal` y no `double` para no arrastrar errores de redondeo binario; en BD `numeric(20,2)`. |
| `Stock` | `int` | Unidades disponibles. `int` es más que suficiente para una tienda de coleccionables y no admite negativos (lo valida FluentValidation). |
| `Categoria` | `Categoria` | Relación **1:N** obligatoria: todo funko pertenece a una categoría. Se configura con `HasOne/WithMany` y `DeleteBehavior.Restrict` para no borrar categorías en uso. |
| `CreatedAt` | `DateTime` | Fecha de alta (auditoría). Se guarda en **UTC** (`DateTime.UtcNow`); Npgsql lo mapea a `timestamptz`. |
| `UpdatedAt` | `DateTime` | Fecha de la última modificación. También UTC, para poder ordenar y auditar. |
| `IsDeleted` | `bool` | Borrado lógico. `HasQueryFilter(x => !x.IsDeleted)` hace que EF excluya los borrados sin que el resto del código lo recuerde. Valor por defecto `false`. |

### `Categoria`

| Propiedad | Tipo | Justificación |
|---|---|---|
| `Id` | `long` | Identificador único de la categoría. |
| `Name` | `string` | Nombre visible (Anime, Videojuegos, Cómics...). Obligatorio, máximo 100 caracteres. |
| `Funkos` | `ICollection<Funko>` | Navegación inversa de la relación 1:N. Permite consultar los funkos de una categoría desde el modelo. |

**Relación:** una categoría tiene muchos funkos; un funko tiene exactamente una
categoría. La FK se llama `CategoriaId` y la elimina en cascada **no** está
permitida (`Restrict`): primero se reasignan o borran los funkos.

## 🌐 Cliente y servidor

- **Servidor** (este repositorio): API REST en ASP.NET Core, **sin estado**. Cada
  petición es independiente, no hay sesión ni cookies; toda la información viaja
  en la URL, la cabecera y el cuerpo JSON.
- **Clientes**: cualquier cliente HTTP. Durante el desarrollo se usa
  `Invoke-RestMethod` (PowerShell), Bruno/Postman o el propio navegador para los
  `GET`. En el futuro podría consumirla una SPA o una app Blazor.
- **Contrato**: JSON en peticiones y respuestas; errores en formato
  [Problem Details (RFC 7807)](https://datatracker.ietf.org/doc/html/rfc7807).

## 🔌 Endpoints

URL base en desarrollo: `http://localhost:5052`

| Verbo | Ruta | Descripción | Éxito |
|---|---|---|---|
| `GET` | `/api/funkos` | Lista todos los funkos activos | `200` |
| `GET` | `/api/funkos/{id}` | Obtiene un funko por id | `200` |
| `GET` | `/api/funkos/buscar?nombre={texto}` | Busca por nombre (contiene, sin distinguir mayúsculas) | `200` |
| `POST` | `/api/funkos` | Crea un funko | `201` + `Location` |
| `PUT` | `/api/funkos/{id}` | Actualiza un funko completo | `200` |
| `PATCH` | `/api/funkos/{id}/precio` | Actualiza solo el precio | `200` |
| `PATCH` | `/api/funkos/{id}/stock` | Ajusta el stock: positivo repone, negativo descuenta | `200` |
| `DELETE` | `/api/funkos/{id}` | Borrado lógico | `204` |
| `GET` | `/api/categorias` | Lista todas las categorías | `200` |
| `GET` | `/api/categorias/{id}` | Obtiene una categoría por id | `200` |

Errores habituales: `400` validación o parámetros incorrectos, `404` recurso no
encontrado, `500` error no controlado.

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

## 🚀 Puesta en marcha

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
API queda escuchando en `http://localhost:5052`. Las tablas se crean solas, no
hay que ejecutar SQL a mano.

### 3. Documentación de la API

En desarrollo está disponible el documento OpenAPI en
`http://localhost:5052/openapi/v1.json`. La interfaz Swagger UI en `/swagger`
está pendiente (P1).

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

## 🗃️ Migraciones

```powershell
# Crear una migración tras cambiar el modelo
dotnet ef migrations add NombreDescritivo --project Funko-Api/Funko-Api/Funko-Api.csproj

# Aplicarlas sin arrancar la API
dotnet ef database update --project Funko-Api/Funko-Api/Funko-Api.csproj
```

Detalle importante: los datos semilla se insertan con `HasData` e ids explícitos.
Como las columnas son `IDENTITY BY DEFAULT`, PostgreSQL **no avanza la secuencia**,
así que la migración inicial incluye un `setval` para reposicionar `funko` y
`categoria`. Sin eso, el primer `POST` chocaría contra la clave primaria.

## 🧪 Testing

```powershell
# Toda la suite (necesita Docker para los tests de integración)
dotnet test

# Solo lo que no necesita Docker
dotnet test --filter "FullyQualifiedName!~Postgre"
```

| Tipo | Cantidad | Tecnología |
|---|---|---|
| Unitarios (servicios, validadores, middleware, repos con SQLite en memoria) | 70 | NUnit, Moq, FluentAssertions |
| Integración (repositorio contra PostgreSQL real) | 11 | Testcontainers |

Cobertura actual de los 81 tests:

- `FunkoServiceTests` y `CategoriaServiceTests`: reglas y propagación de errores.
- `CreateFunkoValidatorTests` / `UpdateFunkoValidatorTests`: validaciones de DTO.
- `GlobalExceptionHandlerTests`: Problem Details con `traceId` y mapeo de excepciones.
- `FunkoRepositoryTests`: consultas, búsqueda y borrado lógico sobre SQLite.
- `FunkoRepositoryPostgreTests`: mapeo y migraciones contra PostgreSQL en contenedor.

## 🏗️ Arquitectura y estructura

Arquitectura en capas: **Controller → Service → Repository → EF Core**.

```
Funko-Api/                          # raíz del repositorio
├── .gitignore
├── README.md
└── Funko-Api/                      # solución
    ├── Funko-Api.slnx
    ├── Funko-Api/                  # API
    │   ├── Controller/             # endpoints (Funkos, Categorias)
    │   ├── Dto/                    # CreateFunkoDto, UpdateFunkoDto, FunkoDto...
    │   ├── Entities/               # AppDbContext
    │   ├── Errors/                 # DomainError, NotFound, ValidationError
    │   ├── Infrastructure/         # DI, DatabaseConfig, migraciones al arrancar
    │   ├── Mapper/                 # métodos de extensión entidad <-> DTO
    │   ├── Migrations/             # migraciones de EF Core
    │   ├── Models/                 # Funko, Categoria
    │   ├── Repositories/           # IFunkoRepository, ICategoriaRepository
    │   ├── Services/               # IFunkoService, ICategoriaService
    │   ├── Validator/              # validadores FluentValidation
    │   ├── Program.cs
    │   └── appsettings*.json
    └── Funko-Api.Test/
        ├── Repositories/
        ├── Services/
        ├── Validator/
        └── Dto/
```

Responsabilidades:

- **Controller**: traduce HTTP ↔ dominio. No contiene reglas de negocio.
- **Service**: valida, coordina repositorios y decide qué error devolver.
- **Repository**: acceso a datos con EF Core (`AsNoTracking` + `Include`).
- **Errors**: errores de dominio independientes de HTTP.
- **Infrastructure**: registro de dependencias y arranque de la base de datos.

## 🤔 Decisiones de diseño

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

### Minimal API vs. Controladores

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

Con 10 endpoints, DTOs, validadores y (pronto) autenticación por roles, los
controladores ofrecen una estructura más clara.

### Patrón Result

Los servicios devuelven `Result<T, DomainError>` de `CSharpFunctionalExtensions`.
El dominio no conoce HTTP: `NotFound` y `ValidationError` se traducen a Problem
Details en un único punto, `Errors/DomainErrorExtension.cs`. Ventaja: las reglas
de negocio se pueden testear sin levantar el servidor.

Las excepciones no controladas (fallos de BD, timeouts, bugs) no llegan al
cliente: `Middleware/GlobalExceptionHandler.cs` las captura, las registra en el
log y responde con Problem Details (`application/problem+json`) incluyendo
`traceId` para correlacionar la respuesta con la traza del servidor.

### Logging con Serilog

Serilog sustituye al logger por defecto y escribe en dos sitios: consola (formato
compacto para desarrollo) y `logs/funko-AAAAMMDD.log` (rotación diaria, 7 días).
Los niveles de `Microsoft` y EF Core se suben a `Warning` para que el log de
negocio no se ahogue entre trazas del framework. `UseSerilogRequestLogging()`
deja una línea por petición con método, ruta, código de estado y milisegundos.

### Datos semilla con `HasData`

Los seeds viajan dentro de la migración, así que cualquier entorno arranca con
las 3 categorías y los 5 funkos. El `setval` de la migración corrige la secuencia
de identidad (ver sección de migraciones).

## 🐳 Docker

Pendiente de la fase 12: `Dockerfile` multi-etapa y `docker-compose.yml` para
levantar API + PostgreSQL con `docker compose up`. Mientras tanto, el contenedor
de PostgreSQL de desarrollo se arranca a mano con el comando de la sección
*Puesta en marcha*.

## 📌 Estado del proyecto

| Fase | Estado |
|---|---|
| 1. Define tu recurso | ✅ |
| 2. Diseña la API en papel | ✅ |
| 3. Minimal API | 🟡 documentada como comparación, no se mantiene |
| 4. Controladores | ✅ |
| 5. Inyección de dependencias | ✅ |
| 6. Patrón Result | ✅ |
| 7. DTOs y validaciones | 🟡 |
| 8. Configuración y logs | ✅ |
| 9. Persistencia con EF Core | ✅ |
| 10–12. Seguridad, Swagger, Docker | ⏳ pendientes |

---

## ✅ Checklist de la rúbrica — Práctica 3

> **Última revisión:** 2026-10-27 · **Checkpoint:** fase 9
> **Verificado con:** `dotnet build` (0 errores) y `dotnet test` (81/81).
>
> **Leyenda:** ✅ cumple · 🟡 parcial / no conforme · ❌ no existe · ❓ no verificable
> **Prioridad:** **P0** bloqueante · **P1** correctness · **P2** estilo · **P3** flujo de trabajo

### 0. Entregables generales

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 0.1 | Repositorio en GitHub llamado `FunkoApp` | 🟡 | Repo `Funko-Api`, proyecto `Funko-Api`, namespace `Funko_Api` | P2 |
| 0.2 | Solución en formato `.slnx` | ✅ | `Funko-Api/Funko-Api.slnx` | — |
| 0.3 | Commits incrementales, uno por fase | 🟡 | Mejorando: `chore` de limpieza ya con Conventional Commits | P3 |
| 0.4 | README con uso, tests, Docker y justificación | ✅ | Este documento | — |
| 0.5 | `docker compose up` levanta API + BD | ❌ | Fase 12 | Pend. |
| 0.6 | Documentación en `/swagger` | ❌ | Solo OpenAPI JSON en `/openapi/v1.json` | P1 |
| 0.7 | `dotnet test` en verde | ✅ | 81/81 | — |
| 0.8 | `dotnet build` sin errores | ✅ | 0 errores | — |
| 0.9 | `bin/` y `obj/` fuera del repo (`.gitignore`) | ✅ | `.gitignore` + `git rm --cached` | — |
| 0.10 | `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig` | ❌ | No existen | P2 |
| 0.11 | Carpeta `doc/` con guías numeradas | ❌ | No existe | P2 |
| 0.12 | Sin código muerto | ✅ | Eliminados `FunkoConfig`, `UpdateFrom`, `FunkoPatchRequestDto` y `UnitTest1` | — |

### 1. Define tu recurso

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 1.1 | Propiedades de `Funko` enumeradas | ✅ | `Models/Funko.cs` | — |
| 1.2 | Cada propiedad justificada por escrito | ✅ | Sección *Modelo de datos* | — |
| 1.3 | Cliente y servidor identificados | ✅ | Sección *Cliente y servidor* | — |

### 2. Diseña la API en papel

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 2.1 | Tabla de endpoints (verbo, ruta, descripción, código de éxito) | ✅ | Sección *Endpoints* | — |
| 2.2 | Listar todos | ✅ | `GET /api/funkos` → 200 | — |
| 2.3 | Consultar por ID | ✅ | `GET /api/funkos/{id}` → 200 / 404 | — |
| 2.4 | Crear | ✅ | `POST /api/funkos` → 201 | — |
| 2.5 | Actualizar completo | ✅ | `PUT /api/funkos/{id}` → 200 | — |
| 2.6 | Eliminar | ✅ | `DELETE /api/funkos/{id}` → 204 | — |
| 2.7 | **Buscar por nombre** | ✅ | `GET /api/funkos/buscar?nombre=...` | — |

### 3. Primera API (Minimal API)

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 3.1 | CRUD como Minimal API con `List<Funko>` | ❌ | No se mantiene: comparación en el README | P1 |
| 3.2 | Verificado con `.http` o Swagger | 🟡 | `Funko-Api.http` sigue apuntando a `/todos` | P1 |

### 4. La misma API con controladores

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 4.1 | CRUD con `FunkosController` | ✅ | `Controller/FunkosController.cs` | — |
| 4.2 | Comparación de enfoques en el README | ✅ | Sección *Decisiones de diseño* | — |
| 4.3 | Retorno tipado `ActionResult<T>` | ❌ | Casi todo `IActionResult` | P1 |
| 4.4 | `[ProducesResponseType]` por código | ❌ | Ninguno | P2 |
| 4.5 | Un controlador por agregado (`ICategoriaService`) | ✅ | `CategoriasController` usa `ICategoriaService` | — |

### 5. Inyección de dependencias completa

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 5.1 | `IFunkoService` / `FunkoService` | ✅ | `Services/` | — |
| 5.2 | `IFunkoRepository` / `FunkoRepository` | ✅ | `Repositories/Funko/` | — |
| 5.3 | Repositorio en memoria `Dictionary<long, Funko>` | ❌ | Se pasó directo a EF (documentado) | P1 |
| 5.4 | Ciclos de vida correctos | 🟡 | Marker interfaces + Scrutor: no es el patrón del profe | P1 |
| 5.5 | Constructor primario | ✅ | Service, repo y controller | — |
| 5.6 | **`AppDbContext` registrado en DI** | ✅ | `Infrastructure/DatabaseConfig.cs` | — |

### 6. Errores con patrón Result

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 6.1 | `Result<T>` en los servicios | ✅ | `CSharpFunctionalExtensions` | — |
| 6.2 | Error: Funko no encontrado | ✅ | `NotFound` | — |
| 6.3 | Error: **nombre duplicado** | ✅ | `ConflictError.Duplicate` → 409; comprobado con `ExistsByNameAsync` en create y update | — |
| 6.4 | Error: **stock insuficiente** | ✅ | `BusinessRuleError.StockInsuficiente` → 422 en `PATCH /api/funkos/{id}/stock` | — |
| 6.5 | Manejador global → Problem Details | ✅ | `Middleware/GlobalExceptionHandler.cs`: 500/409/408/400 con `traceId`, `instance` y `timestamp` | — |
| 6.6 | Traducción `DomainError` → HTTP centralizada | ✅ | `ToHttpResult`/`ToHttpResult<T>` únicos en `Errors/DomainErrorExtension.cs` | — |
| 6.7 | Repositorio no decide errores de negocio | ✅ | Repositorios devuelven entidad/`null`; el servicio crea `NotFound`/`ValidationError` | — |

### 7. DTOs, validaciones y mapeadores

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 7.1 | `CreateFunkoDto`, `UpdateFunkoDto`, `FunkoDto` | ✅ | `Dto/` | — |
| 7.2 | **Data Annotations** | ✅ | Anotaciones en los DTOs + `InvalidModelStateResponseFactory` (400 en español con `traceId`); FluentValidation en el servicio | — |
| 7.3 | Validador de dominio | ✅ | `FunkoRequestValidator` (duplica reglas) | — |
| 7.4 | Mapeo con métodos de extensión | ✅ | `Mapper/FunkoMapper.cs` | — |
| 7.5 | Longitudes BD == validator | ✅ | `HasMaxLength(200)` en BD y 200 en validadores (migración `AmpliarLongitudNombreFunko`) | — |
| 7.6 | Mensajes de validación en español | ✅ | "El nombre es obligatorio.", "El stock no puede ser negativo."... | — |
| 7.7 | Sin DTOs muertos | ✅ | `FunkoPatchRequestDto` y sus 7 tests eliminados | — |
| 7.8 | `FunkoDto` no expone `IsDeleted` | ❌ | Siempre `false` por query filter | P2 |

### 8. Configuración y logs

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 8.1 | `appsettings.json` + perfiles | ✅ | `appsettings.Development/Production.json` con `ConnectionStrings` y `Logging`; Serilog en `SerilogConfig` | — |
| 8.2 | Serilog consola + fichero | ✅ | `Infrastructure/SerilogConfig.cs`: consola + `logs/funko-AAAAMMDD.log` (7 días) | — |
| 8.3 | Log estructurado de operaciones CRUD | ✅ | `ILogger<T>` con placeholders + `UseSerilogRequestLogging` (método, ruta, estado, ms) | — |
| 8.4 | Config por `IConfiguration` en extension methods | ✅ | `DatabaseConfig` recibe `builder.Configuration`; sin configuración estática | — |
| 8.5 | Sin secretos ni paquetes en preview | ✅ | Serilog.AspNetCore 10.0.0, Sinks.File 7.0.0 y Sinks.Console 6.1.1 (estables) | — |

### 9. Persistencia real

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 9.1 | EF Core con PostgreSQL | ✅ | `DatabaseConfig.cs` + Npgsql 10.0.3 | — |
| 9.2 | Relación 1:N `Funko`–`Categoria` | ✅ | `HasOne/WithMany` + `Restrict` | — |
| 9.3 | Borrado lógico con `IsDeleted` | ✅ | `HasQueryFilter(x => !x.IsDeleted)` | — |
| 9.4 | **Migraciones** | ✅ | `Migrations/InitialCreate` | — |
| 9.5 | Migraciones aplicadas al arrancar | ✅ | `InitializeDatabaseAsync` | — |
| 9.6 | Datos semilla | ✅ | `HasData` (3 categorías, 5 funkos) | — |
| 9.7 | `AsNoTracking` + `Include` | ✅ | Repositorios | — |
| 9.8 | FK explícita (no *shadow property*) | 🟡 | `e.Property<long>("CategoriaId")` | P2 |
| 9.9 | `IsDeleted` correcto en update | ✅ | El repositorio consulta con el filtro global: un funko borrado devuelve `null` → 404 (test en SQLite y PostgreSQL) | — |

### 10–12. Seguridad, documentación y despliegue

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 10.1 | JWT + BCrypt | ❌ | Pendiente (fase 10) | — |
| 10.2 | Escritura autenticada; borrado solo Admin | ❌ | Pendiente (fase 10) | — |
| 11.1 | Swagger/OpenAPI con comentarios XML | 🟡 | `AddOpenApi` sin XML docs ni UI | P1 |
| 11.2 | Tests unitarios del servicio | ✅ | 17 casos en `FunkoServiceTests` | — |
| 11.3 | Tests de integración con `WebApplicationFactory` | ❌ | Falta `public partial class Program;` | P1 |
| 12.1 | Dockerfile multi-etapa + compose | ❌ | Pendiente (fase 12) | — |

### Transversales: estilo del profe

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| T.1 | Carpetas en plural (`Controllers/`, `Dtos/`, `Mappers/`, `Validators/`) | ❌ | `Controller/`, `Dto/`, `Mapper/`, `Validator/` | P2 |
| T.2 | `Models/` para dominio y `Data/` para `DbContext` | ❌ | `Entities/` con `namespace ...Data` | P2 |
| T.3 | DI explícito por `Infrastructures/*Config.cs` | ❌ | Scrutor + marker interfaces | P1 |
| T.4 | XML docs en todo miembro público + `GenerateDocumentationFile` | ❌ | Solo métodos de controller; sin `<param>`/`<returns>` | P2 |
| T.5 | Tests en `Unit/` e `Integration/` + `TestCategories.cs` | ❌ | Carpetas por tipo técnico | P2 |
| T.6 | Integración con `[Category("Integration")]` | ❌ | Testcontainers se ejecuta siempre (exige Docker) | P2 |
| T.7 | Nombres de test `Metodo_Escenario_Resultado` | ✅ | En español | — |
| T.8 | FluentAssertions (no `Assert.AreEqual`) | ✅ | Toda la suite | — |
| T.9 | Mensajes de error y log en español | 🟡 | Errores de dominio sí; validaciones en inglés | P2 |
| T.10 | Conventional Commits en español | 🟡 | Últimos commits sí; los antiguos no | P3 |
| T.11 | Ramas `main` / `develop` / `feature-*` con merge explícito | 🟡 | `feature/Controller`; sin `main` ni merges | P3 |
| T.12 | Tags `vX.Y.Z` | ❌ | Ninguno | P3 |
| T.13 | Paquetes solo de tests en el proyecto de tests | ❌ | `FluentAssertions` está en la API | P2 |
| T.14 | Versiones alineadas con la guía | 🟡 | `Testcontainers 4.3` arrastra `SSH.NET` vulnerable (NU1903) | P2 |

### Resumen

| Ámbito | ✅ | 🟡 | ❌ |
|---|---|---|---|
| Fases 1–9 + entregables (64 items) | 46 | 8 | 10 |
| Total (incluye fases 10–12 y estilo, 84 items) | 49 | 13 | 22 |

> Todas las ❌ de las fases 10–12 son trabajo futuro. De las fases 1–9 quedan
> abiertos `public partial class Program;` + test con `WebApplicationFactory` (11.3)
> y la UI de Swagger (11.1); el resto son P2/P3 de estilo.

**Nota orientativa: 7/10** (P0 cerrado).

| Escenario | Nota |
|---|---|
| Ahora mismo (P0 cerrado) | 7/10 |
| P0 + P1 | 8/10 |
| P0 + P1 + P2/P3 | 9–10/10 |

### Plan de ataque

**P0 — bloqueantes (cerrado ✅)**

- [x] Arreglar `CS1061` con `ICategoriaService`
- [x] `dotnet build` en verde y `dotnet test` ejecutando (81/81)
- [x] Paquete `Npgsql.EntityFrameworkCore.PostgreSQL` en la API
- [x] `Infrastructure/DatabaseConfig.cs` + `AddDbContext` + `ConnectionStrings`
- [x] Migración `InitialCreate` y aplicación al arrancar
- [x] Endpoint `GET /api/funkos/buscar?nombre=...`
- [x] README: fases 1, 2 y 4
- [x] `.gitignore` + `git rm -r --cached` de `bin/`, `obj/` y `.idea/`

**P1 — correctness (dos sesiones)**

- [x] Repositorios devuelven entidad/`null`; el servicio decide el error
- [x] Una sola extensión `ToHttpResult<T>()`
- [x] `HasMaxLength` == `MaximumLength`
- [x] `ConflictError` (409) y `BusinessRuleError` (422)
- [x] `GlobalExceptionHandler` con Problem Details + `traceId`
- [x] Serilog real (`Serilog.AspNetCore`, consola + fichero)
- [x] Borrar `FunkoConfig`, `UpdateFrom`, `UnitTest1`
- [x] Borrar `FunkoPatchRequestDto` (el PATCH usa DTOs tipados)
- [x] Corregir el `IsDeleted` del update
- [ ] `public partial class Program;` + test de integración

**P2 — estilo (tres sesiones)**

- [ ] Carpetas en plural + `Data/` + `Models/`
- [ ] DI por extension methods (adiós Scrutor)
- [ ] Español en validaciones; XML docs; `[ProducesResponseType]`; Swagger UI en `/swagger`
- [ ] Tests a `Unit/` + `Integration/` con categorías
- [ ] `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`
- [ ] `doc/` numerada

**P3 — flujo (cierre)**

- [ ] Commits atómicos con Conventional Commits
- [ ] `main` + merge explícito desde `develop`
- [ ] Tag `v0.9.0` al cerrar la fase 9

### Cómo usar esta checklist

1. Cada vez que se cierre un punto, cambiar el estado (`🟡` → `✅`).
2. Regla de oro: **no se pasa a la fase siguiente con un P0 abierto**.
3. Antes de cada commit: `dotnet build` + `dotnet test --filter "FullyQualifiedName!~Postgre"`.

---

## 👤 Autor y licencia

Proyecto educativo de **Antukiller** para 2º de DAW.

Material docente basado en las guías y repositorios de
**[José Luis González Sánchez (joseluisgs)](https://github.com/joseluisgs)**
(licencia [CC BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/)).
El código se ha escrito para este proyecto; las ideas, estructura y convenciones
proceden de su temario y de `TiendaDawApi-NetCore`.
