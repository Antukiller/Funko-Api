namespace Funko_Api.Dto;

public sealed record CreateFunkoDto {
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string CategoriaName { get; init; } = string.Empty;
}
