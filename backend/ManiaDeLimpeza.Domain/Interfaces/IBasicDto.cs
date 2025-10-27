namespace ManiaDeLimpeza.Domain.Interfaces;
public interface IBasicDto
{
    List<string> Validate();
    bool IsValid();
}
