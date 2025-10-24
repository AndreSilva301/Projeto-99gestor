namespace ManiaDeLimpeza.Application.Dtos;
public interface IBasicDto
{
    List<string> Validate();
    bool IsValid();
}
