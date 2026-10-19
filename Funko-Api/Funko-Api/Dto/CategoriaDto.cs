using System.ComponentModel.DataAnnotations;

namespace Funko_Api.Dto;

public record CategoriaDto(
    string Name
);

public record CategoriaRequestDto {
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;
}