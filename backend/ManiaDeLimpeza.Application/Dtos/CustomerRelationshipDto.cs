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
            errors.Add("Description is required.");
        if (Description.Length > 500)
            errors.Add("Description maximum length is 500 characters.");
        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}