namespace Funko_Api.Models;

/// <summary>
/// Entidad de dominio que representa un funko del catálogo.
/// </summary>
public class Funko {
    /// <summary>Identificador del funko.</summary>
    public long Id { get; set; }

    /// <summary>Nombre del funko.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Precio de venta.</summary>
    public decimal Price { get; set; }

    /// <summary>Unidades disponibles en stock.</summary>
    public int Stock { get; set; }

    /// <summary>Categoría a la que pertenece el funko.</summary>
    public required Categoria Categoria { get; set; } = null!;

    /// <summary>Fecha de creación en UTC.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Fecha de la última modificación en UTC.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Indica si el funko está borrado lógicamente.</summary>
    public bool IsDeleted { get; set; }
}
