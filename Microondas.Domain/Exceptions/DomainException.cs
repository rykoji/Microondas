namespace Microondas.Domain.Exceptions;

public class DomainException : Exception
{

    public DomainException() : base("Ocorreu um erro de domínio.")
    {
    }

    public DomainException(string? message) : base(message)
    {
    }

    public DomainException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}