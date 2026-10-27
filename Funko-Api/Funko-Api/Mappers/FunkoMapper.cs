using Funko_Api.Dtos;
using Funko_Api.Models;

namespace Funko_Api.Mappers;

public static class FunkoMapper {
    public static FunkoDto ToDto(this Funko funko) => new(
        funko.Id,
        funko.Name,
        funko.Price,
        funko.Stock,
        funko.Categoria.Name,
        funko.CreatedAt,
        funko.UpdatedAt,
        funko.IsDeleted);

    public static Funko ToModel(this CreateFunkoDto dto, Categoria categoria) => new() {
        Name = dto.Name,
        Price = dto.Price,
        Stock = dto.Stock,
        Categoria = categoria
    };

    public static Funko ToModel(this UpdateFunkoDto dto, Categoria categoria) => new() {
        Name = dto.Name,
        Price = dto.Price,
        Stock = dto.Stock,
        Categoria = categoria
    };
}
