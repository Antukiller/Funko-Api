using System.Collections.Generic;

namespace Funko_Api.Errors;

/// <summary>
/// Error de recurso no encontrado (HTTP 404).
/// </summary>
/// <param name="Message">Mensaje legible para el cliente.</param>
public sealed record NotFound(string Message) : DomainError(Message) {
    /// <summary>
    /// Crea un error de recurso no encontrado a partir de su identificador.
    /// </summary>
    /// <param name="id">Identificador buscado.</param>
    /// <param name="resource">Nombre del recurso.</param>
    /// <returns>Error de dominio listo para traducir a HTTP 404.</returns>
    public static NotFound FromId(long id, string resource = "Recurso") =>
        new($"{resource} con ID {id} no encontrado");
}

/// <summary>
/// Error de validación con detalle por campo (HTTP 400).
/// </summary>
/// <param name="Message">Mensaje legible para el cliente.</param>
/// <param name="Errors">Errores agrupados por campo.</param>
public sealed record ValidationError(string Message, Dictionary<string, string[]> Errors) : DomainError(Message) {
    /// <summary>
    /// Crea un error de validación sin detalle por campo.
    /// </summary>
    /// <param name="message">Mensaje legible para el cliente.</param>
    /// <returns>Error de dominio listo para traducir a HTTP 400.</returns>
    public static ValidationError Create(string message) =>
        new(message, []);

    /// <summary>
    /// Crea un error de validación con el detalle por campo.
    /// </summary>
    /// <param name="errors">Errores agrupados por campo.</param>
    /// <returns>Error de dominio listo para traducir a HTTP 400.</returns>
    public static ValidationError WithFieldErrors(Dictionary<string, string[]> errors) =>
        new("Errores de validación", errors);
}

/// <summary>
/// Error de conflicto, por ejemplo un nombre duplicado (HTTP 409).
/// </summary>
/// <param name="Message">Mensaje legible para el cliente.</param>
public sealed record ConflictError(string Message) : DomainError(Message) {
    /// <summary>
    /// Crea un error de recurso duplicado.
    /// </summary>
    /// <param name="resource">Tipo de recurso.</param>
    /// <param name="name">Nombre que ya existe.</param>
    /// <returns>Error de dominio listo para traducir a HTTP 409.</returns>
    public static ConflictError Duplicate(string resource, string name) =>
        new($"Ya existe un {resource} con el nombre '{name}'");
}

/// <summary>
/// Error de regla de negocio incumplida (HTTP 422).
/// </summary>
/// <param name="Message">Mensaje legible para el cliente.</param>
public sealed record BusinessRuleError(string Message) : DomainError(Message) {
    /// <summary>
    /// Crea un error de stock insuficiente.
    /// </summary>
    /// <param name="name">Nombre del funko.</param>
    /// <param name="disponible">Stock disponible.</param>
    /// <param name="solicitado">Unidades solicitadas.</param>
    /// <returns>Error de dominio listo para traducir a HTTP 422.</returns>
    public static BusinessRuleError StockInsuficiente(string name, int disponible, int solicitado) =>
        new($"Stock insuficiente para '{name}': disponible {disponible}, solicitado {solicitado}");
}
