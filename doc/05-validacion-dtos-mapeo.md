# 5. Validación, DTOs y mapeo

## Índice

- [5.1. DTOs](#51-dtos)
- [5.2. Validación en dos niveles](#52-validación-en-dos-niveles)
- [5.3. Mapeo](#53-mapeo)
- [5.4. Qué se expone y qué no](#54-qué-se-expone-y-qué-no)

---

## 5.1. DTOs

| DTO | Uso | Campos |
|---|---|---|
| `CreateFunkoDto` | `POST /api/funkos` | `Name`, `Price`, `Stock`, `CategoriaName` |
| `UpdateFunkoDto` | `PUT /api/funkos/{id}` | `Name`, `Price`, `Stock`, `CategoriaName` |
| `FunkoDto` | Respuestas de funko | `Id`, `Name`, `Price`, `Stock`, `CategoriaName`, `CreatedAt`, `UpdatedAt` |
| `CategoriaDto` | Respuestas de categoría | `Id`, `Name` |

Son `record` con propiedades `init`: inmutables una vez construidos.

## 5.2. Validación en dos niveles

**Data Annotations en los DTOs** → `[ApiController]` valida antes de entrar en
la acción. Si falla, `InvalidModelStateResponseFactory` construye un
`ValidationProblemDetails` en español con un error por campo y `traceId`:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Errores de validación",
  "status": 400,
  "errors": {
    "Name": ["El nombre es obligatorio."]
  },
  "traceId": "0HN..."
}
```

**FluentValidation en el dominio** → reglas reutilizables sobre la entidad:

| Validador | Valida |
|---|---|
| `FunkoRequestValidator` | `Funko` (nombre, precio, stock, categoría) |
| `CreateFunkoValidator` | `CreateFunkoDto` |
| `UpdateFunkoValidator` | `UpdateFunkoDto` |

Los mensajes están en español y las longitudes coinciden con la base de datos
(`HasMaxLength(200)`).

## 5.3. Mapeo

`Mappers/FunkoMapper.cs` usa métodos de extensión, sin AutoMapper:

```csharp
public static FunkoDto ToDto(this Funko funko) => new(
    funko.Id,
    funko.Name,
    funko.Price,
    funko.Stock,
    funko.Categoria.Name,
    funko.CreatedAt,
    funko.UpdatedAt);

public static Funko ToModel(this CreateFunkoDto dto, Categoria categoria) => new() {
    Name = dto.Name,
    Price = dto.Price,
    Stock = dto.Stock,
    Categoria = categoria
};
```

## 5.4. Qué se expone y qué no

- `FunkoDto` **no** incluye `IsDeleted`: es un detalle de persistencia.
- Las entidades nunca salen del servicio: siempre se convierten a DTO.
- `CategoriaName` se resuelve con `Include`, así el cliente no necesita otra
  llamada.
