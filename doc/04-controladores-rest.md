# 4. Controladores REST

## Índice

- [4.1. Endpoints](#41-endpoints)
- [4.2. Retorno tipado](#42-retorno-tipado)
- [4.3. Documentación de respuestas](#43-documentación-de-respuestas)
- [4.4. Fichero `.http`](#44-fichero-http)

---

## 4.1. Endpoints

### Funkos

| Verbo | Ruta | Éxito | Errores |
|---|---|---|---|
| GET | `/api/funkos` | 200 | — |
| GET | `/api/funkos/buscar?nombre=` | 200 | 400 |
| GET | `/api/funkos/{id}` | 200 | 404 |
| POST | `/api/funkos` | 201 | 400, 404, 409 |
| PUT | `/api/funkos/{id}` | 200 | 400, 404, 409 |
| PATCH | `/api/funkos/{id}/precio` | 200 | 400, 404 |
| PATCH | `/api/funkos/{id}/stock` | 200 | 400, 404, 422 |
| DELETE | `/api/funkos/{id}` | 204 | 404 |

### Categorías

| Verbo | Ruta | Éxito | Errores |
|---|---|---|---|
| GET | `/api/categorias` | 200 | — |
| GET | `/api/categorias/{id}` | 200 | 404 |

`POST` devuelve `201 Created` con la cabecera `Location` gracias a
`CreatedAtAction(nameof(GetById), ...)`.

## 4.2. Retorno tipado

Los endpoints con cuerpo devuelven `ActionResult<T>`:

```csharp
public async Task<ActionResult<FunkoDto>> GetById(long id) =>
    (await service.GetByIdAsync(id)).ToHttpResult();
```

Ventajas:

- Swagger deduce el esquema de `T`.
- El compilador impide devolver un tipo incorrecto.
- `ToHttpResult<T>` convierte el error de dominio en su respuesta HTTP.

Los endpoints sin cuerpo (DELETE) usan `ActionResult` y `NoContent()`.

## 4.3. Documentación de respuestas

Cada acción declara sus códigos:

```csharp
[HttpPatch("{id:long}/stock")]
[ProducesResponseType(typeof(FunkoDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
public async Task<ActionResult<FunkoDto>> PatchStock(long id, [FromBody] int cantidad) =>
    (await service.AdjustStockAsync(id, cantidad)).ToHttpResult();
```

No se pone `[Produces("application/json")]` a nivel de clase: forzaría también
el content type de los errores a `application/json` y rompería
`application/problem+json`.

## 4.4. Fichero `.http`

`Funko-Api/Funko-Api/Funko-Api.http` contiene una petición por endpoint con
variables de host, para probar la API desde Rider o VS Code:

```http
@Funko_Api_HostAddress = http://localhost:5052

### Listar todos los funkos
GET {{Funko_Api_HostAddress}}/api/funkos
Accept: application/json
```

Los `PATCH` de precio y stock envían un número JSON como cuerpo
(`19.99` y `-2` respectivamente).
