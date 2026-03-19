namespace URP.Domain.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string resource, object id)
        : base($"{resource} with identifier '{id}' was not found.") { }

    public NotFoundException(string message) : base(message) { }
}
