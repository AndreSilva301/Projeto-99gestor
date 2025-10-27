using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class CustomerRelationshipCreateDto
{
    [Required]
    public string Description { get; set; } = string.Empty;
}
