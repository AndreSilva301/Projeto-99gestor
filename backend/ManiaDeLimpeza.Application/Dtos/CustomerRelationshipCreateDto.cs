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
            errors.Add("Description is required.");
        if (Description.Length > 500)
            errors.Add("Description maximum length is 500 characters.");
        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}
