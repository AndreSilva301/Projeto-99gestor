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
            errors.Add("Id is requerided;");
        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("Description is requerided;");
        if (Description.Length > 500)
            errors.Add("Description maximum length is 500 characters;");
        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}