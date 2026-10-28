namespace Funko_Api.Test;

/// <summary>
/// Categorías de tests para filtrar en <c>dotnet test</c>.
/// </summary>
public static class TestCategories {
    /// <summary>Tests rápidos que no necesitan Docker.</summary>
    public const string Unit = "Unit";

    /// <summary>Tests que arrancan contenedores Docker.</summary>
    public const string Integration = "Integration";
}
