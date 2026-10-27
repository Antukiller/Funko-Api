using System;

namespace Funko_Api.Dtos;

public record FunkoDto(
    long Id,
    string Name,
    decimal Price,
    int Stock,
    string CategoriaName,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsDeleted
);
