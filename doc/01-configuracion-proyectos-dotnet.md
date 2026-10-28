# 1. Configuración de proyectos .NET

## Índice

- [1.1. Solución y proyectos](#11-solución-y-proyectos)
- [1.2. Estructura de la raíz](#12-estructura-de-la-raíz)
- [1.3. Versiones centralizadas (CPM)](#13-versiones-centralizadas-cpm)
- [1.4. SDK y estilo de código](#14-sdk-y-estilo-de-código)
- [1.5. Comandos habituales](#15-comandos-habituales)

---

## 1.1. Solución y proyectos

La solución `Funko-Api/Funko-Api.slnx` agrupa dos proyectos:

| Proyecto | Tipo | Responsabilidad |
|---|---|---|
| `Funko-Api` | `Microsoft.NET.Sdk.Web` | API, controladores, servicios, repositorios y acceso a datos |
| `Funko-Api.Test` | `Microsoft.NET.Sdk` | Tests unitarios y de integración |

El proyecto de tests referencia a la API y es el único que incluye
`FluentAssertions`, `Moq`, `Testcontainers` y `Microsoft.AspNetCore.Mvc.Testing`.

## 1.2. Estructura de la raíz

```text
Funko-Api/
├── .editorconfig              # estilo común
├── .gitignore
├── Directory.Build.props      # propiedades comunes a todos los proyectos
├── Directory.Packages.props   # versiones de paquetes (CPM)
├── global.json                # versión del SDK
├── README.md
├── doc/                       # estas guías
└── Funko-Api/                 # solución
```

Los `.csproj` **no llevan `Version`**: todas las versiones viven en
`Directory.Packages.props`.

## 1.3. Versiones centralizadas (CPM)

`Directory.Packages.props` activa
`ManagePackageVersionsCentrally` y declara cada paquete una sola vez:

| Paquete | Versión |
|---|---|
| `CSharpFunctionalExtensions` | 3.7.0 |
| `FluentValidation` | 12.2.0 |
| `Microsoft.EntityFrameworkCore` (+ Design, Relational) | 10.0.13 |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.3 |
| `Serilog.AspNetCore` / `Sinks.*` | 10.0.0 / 6.1.1 / 7.0.0 |
| `Swashbuckle.AspNetCore` | 10.2.3 |
| `NUnit` / `NUnit3TestAdapter` | 4.3.2 / 5.0.0 |
| `Moq` / `FluentAssertions` | 4.20.72 / 8.11.0 |
| `Testcontainers.PostgreSql` | 4.3.0 |

Añadir un paquete:

```powershell
dotnet add Funko-Api/Funko-Api/Funko-Api.csproj package <Paquete>
```

y fijar su versión en `Directory.Packages.props`.

## 1.4. SDK y estilo de código

- `global.json` fija el SDK **10.0.101** con `rollForward: latestFeature`.
- `Directory.Build.props` aplica `LangVersion 14`, `Nullable`, `ImplicitUsings`
  y `TreatWarningsAsErrors`: **cualquier aviso rompe el build**.
- `.editorconfig` fija UTF-8, LF, 4 espacios, llaves en línea nueva y
  namespaces file-scoped.

## 1.5. Comandos habituales

```powershell
dotnet build Funko-Api/Funko-Api.slnx
dotnet run --project Funko-Api/Funko-Api
dotnet test Funko-Api/Funko-Api.slnx
dotnet test Funko-Api/Funko-Api.slnx --filter "FullyQualifiedName~Unit"
dotnet test Funko-Api/Funko-Api.slnx --filter "FullyQualifiedName~Integration"
```
