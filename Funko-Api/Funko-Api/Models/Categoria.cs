namespace Funko_Api.Models;

/// <summary>
/// Entidad de dominio que representa una categoría de funkos.
/// </summary>
public class Categoria {
    /// <summary>Identificador de la categoría.</summary>
    public long Id { get; set; }

    /// <summary>Nombre de la categoría.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Funkos que pertenecen a la categoría.</summary>
    public ICollection<Funko> Funkos { get; set; } = [];
}
