using Funko_Api.Data;
using Funko_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Funko_Api.Repositories;

/// <summary>
/// Repositorio de funkos sobre Entity Framework Core.
/// </summary>
/// <param name="context">Contexto de base de datos.</param>
public class FunkoRepository(AppDbContext context) : IFunkoRepository {
    /// <inheritdoc/>
    public async Task<IReadOnlyList<Funko>> GetAllAsync() =>
        await context.Funkos
            .AsNoTracking()
            .Include(x => x.Categoria)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Funko>> SearchByNameAsync(string nombre) {
        var patron = nombre.Trim().ToLower();
        return await context.Funkos
            .AsNoTracking()
            .Include(x => x.Categoria)
            .Where(x => x.Name.ToLower().Contains(patron))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null) {
        var buscado = name.Trim().ToLower();
        return await context.Funkos
            .AnyAsync(funko => funko.Name.ToLower() == buscado
                               && (excludeId == null || funko.Id != excludeId));
    }

    /// <inheritdoc/>
    public async Task<Funko?> GetByIdAsync(long id) =>
        await context.Funkos
            .AsNoTracking()
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == id);

    /// <inheritdoc/>
    public async Task<Funko> CreateAsync(Funko funko) {
        var ahora = DateTime.UtcNow;
        funko.CreatedAt = ahora;
        funko.UpdatedAt = ahora;
        funko.IsDeleted = false;

        context.Funkos.Add(funko);
        await context.SaveChangesAsync();
        return funko;
    }

    /// <inheritdoc/>
    public async Task<Funko?> UpdateAsync(long id, Funko funko) {
        var funkoExistente = await context.Funkos
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (funkoExistente is null)
            return null;

        funkoExistente.Name = funko.Name;
        funkoExistente.Price = funko.Price;
        funkoExistente.Stock = funko.Stock;
        funkoExistente.Categoria = funko.Categoria;
        funkoExistente.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return funkoExistente;
    }

    /// <inheritdoc/>
    public async Task<Funko?> UpdateStockAsync(long id, int stock) {
        var funko = await context.Funkos
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (funko is null)
            return null;

        funko.Stock = stock;
        funko.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return funko;
    }

    /// <inheritdoc/>
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
