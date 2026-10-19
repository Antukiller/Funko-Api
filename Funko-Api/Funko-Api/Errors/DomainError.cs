namespace Funko_Api.Errors;

public abstract record DomainError(string Message) {
    public override string ToString() => $"{GetType().Name}: {Message}";
}