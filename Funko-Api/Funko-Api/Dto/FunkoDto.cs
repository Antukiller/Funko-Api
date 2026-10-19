namespace Funko_Api.Dto;

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

public record FunkoPatchRequestDto {
    public string? Name { get; init; }
    public decimal? Price { get; init; }
    public int? Stock { get; init; }
    public string? CategoriaName { get; init; }
}