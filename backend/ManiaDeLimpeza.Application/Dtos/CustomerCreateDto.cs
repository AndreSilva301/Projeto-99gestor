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
            errors.Add("Nome é obrigatório");
        if (Name?.Length > 255)
            errors.Add("O nome pode ter no máximo 255 caracteres.");
        if (!Email.IsValidEmail())
            errors.Add("O endereço de e-mail não é válido.");
        if (!Phone?.Mobile.IsValidPhone() ?? true)
            errors.Add("Número de telefone inválido.");
        if (Relationships != null)
        {
            foreach (var rel in Relationships)
                errors.AddRange(rel.Validate());
        }
        return errors;
    }

    public bool IsValid() => Validate().Count == 0;
}
