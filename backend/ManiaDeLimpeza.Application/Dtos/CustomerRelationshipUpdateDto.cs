using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class CustomerRelationshipUpdateDto : IBasicDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public List<string> Validate ()
    {
        var errors = new List<string>();
        if (Id < 0)
            errors.Add("O ID é solicitado novamente;");
        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("É necessária uma descrição;");
        if (Description.Length > 500)
            errors.Add("A descrição deve ter no máximo 500 caracteres;");
        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}