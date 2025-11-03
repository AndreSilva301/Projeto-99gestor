using ManiaDeLimpeza.Domain.Interfaces;

namespace ManiaDeLimpeza.Application.Dtos;
public class CustomerRelationshipDto : IBasicDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }

    public List<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("A descrição é obrigatória.");
        if (Description.Length > 500)
            errors.Add("A descrição deve ter no máximo 500 caracteres.");
        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}