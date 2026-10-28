# 7. EF Core y PostgreSQL

## Índice

- [7.1. Modelo de datos](#71-modelo-de-datos)
- [7.2. `AppDbContext`](#72-appdbcontext)
- [7.3. Borrado lógico](#73-borrado-lógico)
- [7.4. Migraciones y datos semilla](#74-migraciones-y-datos-semilla)
- [7.5. Consultas](#75-consultas)
- [7.6. PostgreSQL en desarrollo](#76-postgresql-en-desarrollo)

---

## 7.1. Modelo de datos

```text
Categoria 1 ──── N Funko
```

| Entidad | Propiedades |
|---|---|
| `Categoria` | `Id`, `Name`, `Funkos` |
| `Funko` | `Id`, `Name`, `Price`, `Stock`, `CategoriaId`, `Categoria`, `CreatedAt`, `UpdatedAt`, `IsDeleted` |

La FK es explícita (`CategoriaId`) y la relación usa `Restrict`: no se puede
borrar una categoría con funkos.

## 7.2. `AppDbContext`

`Data/AppDbContext.cs` configura el modelo con Fluent API:

```csharp
modelBuilder.Entity<Funko>(e => {
    e.Property(x => x.Name).HasMaxLength(200).IsRequired();
    e.Property(x => x.Price).HasPrecision(20, 2);
    e.HasOne(x => x.Categoria)
        .WithMany(c => c.Funkos)
        .HasForeignKey("CategoriaId")
        .OnDelete(DeleteBehavior.Restrict);
});
```

La cadena de conexión se lee de `ConnectionStrings:DefaultConnection` en
`Infrastructures/DatabaseConfig.cs`; si falta, la aplicación no arranca.

## 7.3. Borrado lógico

`DELETE` no elimina la fila: marca `IsDeleted = true`. El filtro global hace
que el resto de consultas no la vean:

```csharp
e.HasQueryFilter(x => !x.IsDeleted);
```

Consecuencias:

- `GET` de un funko borrado devuelve 404.
- El repositorio reactiva con `IsDeleted = false` si se vuelve a crear.
- `IsDeleted` **no** se expone en `FunkoDto`.

## 7.4. Migraciones y datos semilla

| Migración | Cambio |
|---|---|
| `InitialCreate` | Tablas, relaciones, índices y semilla |
| `AmpliarLongitudNombreFunko` | Nombre de 100 a 200 caracteres |

Semilla: 3 categorías y 5 funkos (`HasData`).

```powershell
dotnet ef migrations add NombreMigracion --project Funko-Api/Funko-Api
dotnet ef database update --project Funko-Api/Funko-Api
```

Al arrancar, `InitializeDatabaseAsync` aplica las migraciones pendientes
(`MigrateAsync`). En producción conviene controlarlas fuera del arranque.

## 7.5. Consultas

Los repositorios usan `AsNoTracking` para lecturas y `Include` cuando hace
falta la categoría:

```csharp
await context.Funkos
    .AsNoTracking()
    .Include(f => f.Categoria)
    .FirstOrDefaultAsync(f => f.Id == id);
```

Escrituras: se cargan con seguimiento, se modifican y se guarda con
`SaveChangesAsync`.

## 7.6. PostgreSQL en desarrollo

```powershell
docker start funko-postgres
```

Cadena de desarrollo:

```text
Host=localhost;Port=5432;Database=funkos;Username=funkos;Password=funkos
```
