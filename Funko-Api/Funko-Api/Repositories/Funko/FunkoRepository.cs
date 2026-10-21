using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Funko_Api.Data;
using Funko_Api.Errors;
using Funko_Api.Infrastructure.Interface;
using Funko_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Funko_Api.Repositories;

public class FunkoRepository(AppDbContext context) : IFunkoRepository, IScopedService {
    public async Task<IReadOnlyList<Funko>> GetAllAsync() =>
        await context.Funkos
            .AsNoTracking()
            .Include(x => x.Categoria)
            .ToListAsync();

    public async Task<Result<Funko, DomainError>> GetByIdAsync(long id) {
        var funko = await context.Funkos
            .AsNoTracking()
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == id);

        return funko is not null
            ? Result.Success<Funko, DomainError>(funko)
            : Result.Failure<Funko, DomainError>(NotFound.FromId(id, "Funko"));
    }

    public async Task<Result<Funko, DomainError>> CreateAsync(Funko funko) {
        var categoria = await context.Categorias.FindAsync(funko.Categoria.Id);
        if (categoria is null)
            return Result.Failure<Funko, DomainError>(NotFound.FromId(funko.Categoria.Id, "Categoría"));

        var ahora = DateTime.UtcNow;
        var nuevoFunko = new Funko {
            Name = funko.Name,
            Price = funko.Price,
            Stock = funko.Stock,
            Categoria = categoria,
            CreatedAt = ahora,
            UpdatedAt = ahora,
            IsDeleted = false
        };

        await context.Funkos.AddAsync(nuevoFunko);
        await context.SaveChangesAsync();
        return Result.Success<Funko, DomainError>(nuevoFunko);
    }

    public async Task<Result<Funko, DomainError>> UpdateAsync(long id, Funko funko) {
        var funkoExistente = await context.Funkos
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (funkoExistente is null || funko.IsDeleted)
            return Result.Failure<Funko, DomainError>(NotFound.FromId(id, "Funko"));

        var categoria = await context.Categorias.FindAsync(funko.Categoria.Id);
        if (categoria is null)
            return Result.Failure<Funko, DomainError>(NotFound.FromId(funko.Categoria.Id, "Categoría"));

        funkoExistente.Name = funko.Name;
        funkoExistente.Categoria = categoria;
        funkoExistente.Price = funko.Price;
        funkoExistente.Stock = funko.Stock;
        funkoExistente.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Result.Success<Funko, DomainError>(funkoExistente);
    }

    public async Task<bool> DeleteAsync(long id) {
        var funko = await context.Funkos.FirstOrDefaultAsync(x => x.Id == id);
        if (funko is null)
            return false;

        funko.IsDeleted = true;
        funko.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }
}