# 10. Checklist de la rúbrica — Práctica 3

> **Última revisión:** 2026-10-28 · **Checkpoint:** fase 9
> **Verificado con:** `dotnet build` (0 errores) y `dotnet test` (90/90).
>
> **Leyenda:** ✅ cumple · 🟡 parcial / no conforme · ❌ no existe · ❓ no verificable
> **Prioridad:** **P0** bloqueante · **P1** correctness · **P2** estilo · **P3** flujo de trabajo

## Índice

- [0. Entregables generales](#0-entregables-generales)
- [1. Define tu recurso](#1-define-tu-recurso)
- [2. Diseña la API en papel](#2-diseña-la-api-en-papel)
- [3. Primera API (Minimal API)](#3-primera-api-minimal-api)
- [4. La misma API con controladores](#4-la-misma-api-con-controladores)
- [5. Inyección de dependencias completa](#5-inyección-de-dependencias-completa)
- [6. Errores con patrón Result](#6-errores-con-patrón-result)
- [7. DTOs, validaciones y mapeadores](#7-dtos-validaciones-y-mapeadores)
- [8. Configuración y logs](#8-configuración-y-logs)
- [9. Persistencia real](#9-persistencia-real)
- [10–12. Seguridad, documentación y despliegue](#1012-seguridad-documentación-y-despliegue)
- [Transversales: estilo del profe](#transversales-estilo-del-profe)
- [Resumen](#resumen)
- [Plan de ataque](#plan-de-ataque)

## 0. Entregables generales

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 0.1 | Repositorio en GitHub llamado `FunkoApp` | 🟡 | Repo `Funko-Api`, proyecto `Funko-Api`, namespace `Funko_Api` | P2 |
| 0.2 | Solución en formato `.slnx` | ✅ | `Funko-Api/Funko-Api.slnx` | — |
| 0.3 | Commits incrementales, uno por fase | 🟡 | Mejorando: `chore` de limpieza ya con Conventional Commits | P3 |
| 0.4 | README con uso, tests, Docker y justificación | ✅ | `README.md` | — |
| 0.5 | `docker compose up` levanta API + BD | ❌ | Fase 12 | Pend. |
| 0.6 | Documentación en `/swagger` | ✅ | Swagger UI con Swashbuckle en `/swagger` | — |
| 0.7 | `dotnet test` en verde | ✅ | 90/90 | — |
| 0.8 | `dotnet build` sin errores | ✅ | 0 errores | — |
| 0.9 | `bin/` y `obj/` fuera del repo (`.gitignore`) | ✅ | `.gitignore` + `git rm --cached` | — |
| 0.10 | `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig` | ✅ | Configuración de build centralizada | — |
| 0.11 | Carpeta `doc/` con guías numeradas | ✅ | `doc/01–10` | — |
| 0.12 | Sin código muerto | ✅ | Eliminados `FunkoConfig`, `UpdateFrom`, `FunkoPatchRequestDto` y `UnitTest1` | — |

## 1. Define tu recurso

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 1.1 | Propiedades de `Funko` enumeradas | ✅ | `Models/Funko.cs` | — |
| 1.2 | Cada propiedad justificada por escrito | ✅ | Sección *Modelo de datos* | — |
| 1.3 | Cliente y servidor identificados | ✅ | Sección *Cliente y servidor* | — |

## 2. Diseña la API en papel

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 2.1 | Tabla de endpoints (verbo, ruta, descripción, código de éxito) | ✅ | Sección *Endpoints* | — |
| 2.2 | Listar todos | ✅ | `GET /api/funkos` → 200 | — |
| 2.3 | Consultar por ID | ✅ | `GET /api/funkos/{id}` → 200 / 404 | — |
| 2.4 | Crear | ✅ | `POST /api/funkos` → 201 | — |
| 2.5 | Actualizar completo | ✅ | `PUT /api/funkos/{id}` → 200 | — |
| 2.6 | Eliminar | ✅ | `DELETE /api/funkos/{id}` → 204 | — |
| 2.7 | **Buscar por nombre** | ✅ | `GET /api/funkos/buscar?nombre=...` | — |

## 3. Primera API (Minimal API)

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 3.1 | CRUD como Minimal API con `List<Funko>` | ❌ | No se mantiene: comparación en el README | P1 |
| 3.2 | Verificado con `.http` o Swagger | ✅ | `Funko-Api.http` con todos los endpoints + Swagger UI | — |

## 4. La misma API con controladores

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 4.1 | CRUD con `FunkosController` | ✅ | `Controllers/FunkosController.cs` | — |
| 4.2 | Comparación de enfoques en el README | ✅ | Sección *Decisiones de diseño* | — |
| 4.3 | Retorno tipado `ActionResult<T>` | ✅ | Endpoints con cuerpo devuelven `ActionResult<T>` | — |
| 4.4 | `[ProducesResponseType]` por código | ✅ | Por endpoint (200/201/204/400/404/409/422) | — |
| 4.5 | Un controlador por agregado (`ICategoriaService`) | ✅ | `CategoriasController` usa `ICategoriaService` | — |

## 5. Inyección de dependencias completa

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 5.1 | `IFunkoService` / `FunkoService` | ✅ | `Services/` | — |
| 5.2 | `IFunkoRepository` / `FunkoRepository` | ✅ | `Repositories/Funko/` | — |
| 5.3 | Repositorio en memoria `Dictionary<long, Funko>` | ❌ | Se pasó directo a EF (documentado) | P1 |
| 5.4 | Ciclos de vida correctos | ✅ | `AddScoped` explícito por capa en `Infrastructures/*Config.cs` | — |
| 5.5 | Constructor primario | ✅ | Service, repo y controller | — |
| 5.6 | **`AppDbContext` registrado en DI** | ✅ | `Infrastructures/DatabaseConfig.cs` | — |

## 6. Errores con patrón Result

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 6.1 | `Result<T>` en los servicios | ✅ | `CSharpFunctionalExtensions` | — |
| 6.2 | Error: Funko no encontrado | ✅ | `NotFound` | — |
| 6.3 | Error: **nombre duplicado** | ✅ | `ConflictError.Duplicate` → 409; comprobado con `ExistsByNameAsync` en create y update | — |
| 6.4 | Error: **stock insuficiente** | ✅ | `BusinessRuleError.StockInsuficiente` → 422 en `PATCH /api/funkos/{id}/stock` | — |
| 6.5 | Manejador global → Problem Details | ✅ | `Middleware/GlobalExceptionHandler.cs`: 500/409/408/400 con `traceId`, `instance` y `timestamp` | — |
| 6.6 | Traducción `DomainError` → HTTP centralizada | ✅ | `ToHttpResult`/`ToHttpResult<T>` únicos en `Errors/DomainErrorExtension.cs` | — |
| 6.7 | Repositorio no decide errores de negocio | ✅ | Repositorios devuelven entidad/`null`; el servicio crea `NotFound`/`ValidationError` | — |

## 7. DTOs, validaciones y mapeadores

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 7.1 | `CreateFunkoDto`, `UpdateFunkoDto`, `FunkoDto` | ✅ | `Dtos/` | — |
| 7.2 | **Data Annotations** | ✅ | Anotaciones en los DTOs + `InvalidModelStateResponseFactory` (400 en español con `traceId`); FluentValidation en el servicio | — |
| 7.3 | Validador de dominio | ✅ | `FunkoRequestValidator` (duplica reglas) | — |
| 7.4 | Mapeo con métodos de extensión | ✅ | `Mappers/FunkoMapper.cs` | — |
| 7.5 | Longitudes BD == validator | ✅ | `HasMaxLength(200)` en BD y 200 en validadores (migración `AmpliarLongitudNombreFunko`) | — |
| 7.6 | Mensajes de validación en español | ✅ | "El nombre es obligatorio.", "El stock no puede ser negativo."... | — |
| 7.7 | Sin DTOs muertos | ✅ | `FunkoPatchRequestDto` y sus 7 tests eliminados | — |
| 7.8 | `FunkoDto` no expone `IsDeleted` | ✅ | Eliminado del DTO y del mapper | — |

## 8. Configuración y logs

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 8.1 | `appsettings.json` + perfiles | ✅ | `appsettings.Development/Production.json` con `ConnectionStrings` y `Logging`; Serilog en `SerilogConfig` | — |
| 8.2 | Serilog consola + fichero | ✅ | `Infrastructures/SerilogConfig.cs`: consola + `logs/funko-AAAAMMDD.log` (7 días) | — |
| 8.3 | Log estructurado de operaciones CRUD | ✅ | `ILogger<T>` con placeholders + `UseSerilogRequestLogging` (método, ruta, estado, ms) | — |
| 8.4 | Config por `IConfiguration` en extension methods | ✅ | `DatabaseConfig` recibe `builder.Configuration`; sin configuración estática | — |
| 8.5 | Sin secretos ni paquetes en preview | ✅ | Serilog.AspNetCore 10.0.0, Sinks.File 7.0.0 y Sinks.Console 6.1.1 (estables) | — |

## 9. Persistencia real

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 9.1 | EF Core con PostgreSQL | ✅ | `DatabaseConfig.cs` + Npgsql 10.0.3 | — |
| 9.2 | Relación 1:N `Funko`–`Categoria` | ✅ | `HasOne/WithMany` + `Restrict` | — |
| 9.3 | Borrado lógico con `IsDeleted` | ✅ | `HasQueryFilter(x => !x.IsDeleted)` | — |
| 9.4 | **Migraciones** | ✅ | `Migrations/InitialCreate` | — |
| 9.5 | Migraciones aplicadas al arrancar | ✅ | `InitializeDatabaseAsync` | — |
| 9.6 | Datos semilla | ✅ | `HasData` (3 categorías, 5 funkos) | — |
| 9.7 | `AsNoTracking` + `Include` | ✅ | Repositorios | — |
| 9.8 | FK explícita (no *shadow property*) | ✅ | `Funko.CategoriaId` + `HasForeignKey(x => x.CategoriaId)` | — |
| 9.9 | `IsDeleted` correcto en update | ✅ | El repositorio consulta con el filtro global: un funko borrado devuelve `null` → 404 (test en SQLite y PostgreSQL) | — |

## 10–12. Seguridad, documentación y despliegue

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| 10.1 | JWT + BCrypt | ❌ | Pendiente (fase 10) | — |
| 10.2 | Escritura autenticada; borrado solo Admin | ❌ | Pendiente (fase 10) | — |
| 11.1 | Swagger/OpenAPI con comentarios XML | ✅ | Swashbuckle 10.2.3 + `IncludeXmlComments` | — |
| 11.2 | Tests unitarios del servicio | ✅ | 17 casos en `FunkoServiceTests` | — |
| 11.3 | Tests de integración con `WebApplicationFactory` | ✅ | `FunkosEndpointsPostgreTests`: 9 casos sobre el host real (200/201/204/400/404/409/422) | — |
| 12.1 | Dockerfile multi-etapa + compose | ❌ | Pendiente (fase 12) | — |

## Transversales: estilo del profe

| # | Requisito | Estado | Evidencia | Prio |
|---|---|---|---|---|
| T.1 | Carpetas en plural (`Controllers/`, `Dtos/`, `Mappers/`, `Validators/`) | ✅ | `Controllers/`, `Dtos/`, `Mappers/`, `Validators/`, `Infrastructures/` | — |
| T.2 | `Models/` para dominio y `Data/` para `DbContext` | ✅ | `Models/` + `Data/AppDbContext.cs` | — |
| T.3 | DI explícito por `Infrastructures/*Config.cs` | ✅ | `RepositoriesConfig`, `ServicesConfig`, `ValidatorsConfig` + `DatabaseConfig` | — |
| T.4 | XML docs en todo miembro público + `GenerateDocumentationFile` | ✅ | 0 avisos CS1591; `Funko-Api.xml` generado | — |
| T.5 | Tests en `Unit/` e `Integration/` + `TestCategories.cs` | ✅ | `Unit/` (70), `Integration/TestContainers/` (20), `TestCategories.cs` | — |
| T.6 | Integración con `[Category("Integration")]` | ✅ | Fixtures con `TestCategories.Integration` | — |
| T.7 | Nombres de test `Metodo_Escenario_Resultado` | ✅ | En español | — |
| T.8 | FluentAssertions (no `Assert.AreEqual`) | ✅ | Toda la suite | — |
| T.9 | Mensajes de error y log en español | ✅ | Dominio, validaciones y logs en español | — |
| T.10 | Conventional Commits en español | 🟡 | Últimos commits sí; los antiguos no | P3 |
| T.11 | Ramas `main` / `develop` / `feature-*` con merge explícito | 🟡 | `feature/Controller`; sin `main` ni merges | P3 |
| T.12 | Tags `vX.Y.Z` | ❌ | Ninguno | P3 |
| T.13 | Paquetes solo de tests en el proyecto de tests | ✅ | `FluentAssertions` solo en `Funko-Api.Test` | — |
| T.14 | Versiones alineadas con la guía | ✅ | `SSH.NET 2026.0.0` fijado; 0 avisos NU1903 | — |

## Resumen

| Ámbito | ✅ | 🟡 | ❌ |
|---|---|---|---|
| Fases 1–9 + entregables (64 items) | 59 | 2 | 3 |
| Total (incluye fases 10–12 y estilo, 84 items) | 73 | 4 | 7 |

> Todas las ❌ de las fases 10–12 son trabajo futuro (10.1, 10.2 y 12.1). Las
> fases 1–9 quedan cerradas; lo que resta es estilo P2/P3.

**Nota orientativa: 7/10** (P0 cerrado).

| Escenario | Nota |
|---|---|
| Ahora mismo (P0 cerrado) | 7/10 |
| P0 + P1 | 8/10 |
| P0 + P1 + P2/P3 | 9–10/10 |

## Plan de ataque

**P0 — bloqueantes (cerrado ✅)**

- [x] Arreglar `CS1061` con `ICategoriaService`
- [x] `dotnet build` en verde y `dotnet test` ejecutando (90/90)
- [x] Paquete `Npgsql.EntityFrameworkCore.PostgreSQL` en la API
- [x] `Infrastructures/DatabaseConfig.cs` + `AddDbContext` + `ConnectionStrings`
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
- [x] `public partial class Program;` + test de integración

**P2 — estilo (tres sesiones)**

- [x] Carpetas en plural + `Data/` + `Models/`
- [x] DI por extension methods (adiós Scrutor)
- [x] Español en validaciones; XML docs; `[ProducesResponseType]`; Swagger UI en `/swagger`
- [x] Tests a `Unit/` + `Integration/` con categorías
- [x] `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`
- [x] `doc/` numerada

**P3 — flujo (cierre)**

- [ ] Commits atómicos con Conventional Commits
- [ ] `main` + merge explícito desde `develop`
- [ ] Tag `v0.9.0` al cerrar la fase 9

## Cómo usar esta checklist

1. Cada vez que se cierre un punto, cambiar el estado (`🟡` → `✅`).
2. Regla de oro: **no se pasa a la fase siguiente con un P0 abierto**.
3. Antes de cada commit: `dotnet build` + `dotnet test`.
