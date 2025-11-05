using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class CustomerRelationshipCreateDto : IBasicDto
{
    [Required]
    public string Description { get; set; } = string.Empty;

    public List<String> Validate()
    {
        var errors = new List<string>();
        if (String.IsNullOrWhiteSpace(Description))
            errors.Add("A descrição é obrigatória.");
        if (Description.Length > 500)
            errors.Add("A descrição deve ter no máximo 500 caracteres.");
        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}
