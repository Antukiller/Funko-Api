namespace Funko_Api.Errors;

/// <summary>
/// Clase base de los errores de dominio. No conoce HTTP.
/// </summary>
/// <param name="Message">Mensaje legible para el cliente.</param>
public abstract record DomainError(string Message) {
    /// <summary>
    /// Representación del error con el nombre del tipo.
    /// </summary>
    /// <returns>Cadena con el tipo y el mensaje.</returns>
    public override string ToString() => $"{GetType().Name}: {Message}";
}
