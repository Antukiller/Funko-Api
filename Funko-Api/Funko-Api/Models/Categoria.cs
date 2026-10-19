namespace Funko_Api.Models;

public class Categoria {
    public long Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public ICollection<Funko> Funkos { get; set; } = [];
}