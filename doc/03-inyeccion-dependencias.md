# 3. Inyección de dependencias

## Índice

- [3.1. Registro explícito por capa](#31-registro-explícito-por-capa)
- [3.2. Ciclos de vida](#32-ciclos-de-vida)
- [3.3. Constructores primarios](#33-constructores-primarios)
- [3.4. Cómo añadir una dependencia](#34-cómo-añadir-una-dependencia)

---

## 3.1. Registro explícito por capa

No se usa escaneo de ensamblados (Scrutor). Cada capa expone un extension
method en `Infrastructures/`:

| Fichero | Registra |
|---|---|
| `DatabaseConfig.cs` | `AppDbContext` con Npgsql |
| `RepositoriesConfig.cs` | `IFunkoRepository`, `ICategoriaRepository` |
| `ServicesConfig.cs` | `IFunkoService`, `ICategoriaService` |
| `ValidatorsConfig.cs` | Validadores de FluentValidation |
| `SerilogConfig.cs` | Logger de consola + fichero |
| `SwaggerConfig.cs` | Swagger con comentarios XML |

```csharp
public static IServiceCollection AddServicesConfiguration(this IServiceCollection services) {
    services.AddScoped<IFunkoService, FunkoService>();
    services.AddScoped<ICategoriaService, CategoriaService>();

    return services;
}
```

En `Program.cs` se encadenan todos:

```csharp
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddRepositoriesConfiguration();
builder.Services.AddServicesConfiguration();
builder.Services.AddValidatorsConfiguration();
```

## 3.2. Ciclos de vida

| Servicio | Vida | Motivo |
|---|---|---|
| `AppDbContext` | Scoped | Una instancia por petición |
| Repositorios | Scoped | Comparten el contexto de la petición |
| Servicios | Scoped | Coordinan repositorios de la petición |
| Validadores | Scoped | Sin estado, pero coherentes con el resto |
| Logger | Singleton | Lo gestiona Serilog |

Regla: **todo lo que toca EF Core es Scoped**. Un Singleton no puede depender
de un Scoped.

## 3.3. Constructores primarios

Los controladores, servicios y repositorios usan constructor primario:

```csharp
public class FunkosController(IFunkoService service) : ControllerBase { }
public class FunkoService(IFunkoRepository repository) : IFunkoService { }
public class FunkoRepository(AppDbContext context) : IFunkoRepository { }
```

Menos ceremonia y el campo queda disponible en toda la clase.

## 3.4. Cómo añadir una dependencia

1. Crear la interfaz en `Services/` o `Repositories/`.
2. Implementarla con constructor primario.
3. Registrarla en el `*Config.cs` de su capa con la vida adecuada.
4. Consumirla por interfaz, nunca por la clase concreta.
