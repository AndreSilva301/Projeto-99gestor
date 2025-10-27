using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Domain.Interfaces;

namespace ManiaDeLimpeza.Application.Dtos;
public class CustomerCreateDto : IBasicDto
{
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public AddressDto Address { get; set; } = new AddressDto();
    public PhoneDto Phone { get; set; } = new PhoneDto();
    public string Email { get; set; } = string.Empty;
    public string? Observations { get; set; }
    public List<CustomerRelationshipCreateDto>? Relationships { get; set; }
    public List<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name is required.");
        if (Name?.Length > 255)
            errors.Add("Name maximum length is 255 characters.");
        if (!Email.IsValidEmail())
            errors.Add("Email is not valid.");
        if (!Phone?.Mobile.IsValidPhone() ?? true)
            errors.Add("Phone number is not valid.");
        if (Relationships != null)
        {
            foreach (var rel in Relationships)
                errors.AddRange(rel.Validate());
        }
        return errors;
    }

    public bool IsValid() => Validate().Count == 0;
}
