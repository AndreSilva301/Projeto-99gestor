namespace ManiaDeLimpeza.Domain.ExceptionErrors;
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
