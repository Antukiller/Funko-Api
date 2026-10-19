using System.ComponentModel.DataAnnotations;

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

public record FunkoRequestDto {
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    
    public string Name { get; init; } = string.Empty;
    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    
    public decimal Price { get; init; }
    [Required(ErrorMessage = "Stock is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0.")]
    public int Stock { get; init; }
    
    [Required(ErrorMessage = "CategoriaName is required.")]
    [MaxLength(200, ErrorMessage = "CategoriaName cannot exceed 200 characters.")]
    public string CategoriaName { get; init; } = string.Empty;
}

public record FunkoPatchRequestDto {
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string? Name { get; init; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal? Price { get; init; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to 0.")]
    public int? Stock { get; init; }
    
    [MaxLength(200, ErrorMessage = "CategoriaName cannot exceed 200 characters.")]
    public string? CategoriaName { get; init; }
}