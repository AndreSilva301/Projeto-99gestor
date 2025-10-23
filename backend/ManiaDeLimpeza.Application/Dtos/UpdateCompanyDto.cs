using ManiaDeLimpeza.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateCompanyDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? CNPJ { get; set; }

    public AddressDto Address { get; set; } = new AddressDto();

    public PhoneDto Phone { get; set; } = new PhoneDto();

    public IEnumerable<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("O nome da empresa é obrigatório.");

        if (!string.IsNullOrWhiteSpace(CNPJ) && !StringUtils.IsValidCNPJ(CNPJ))
            errors.Add("O CNPJ informado é inválido.");

        if (Address != null)
        {
            if (string.IsNullOrWhiteSpace(Address.Street))
                errors.Add("Rua é obrigatória.");
            if (string.IsNullOrWhiteSpace(Address.Number))
                errors.Add("Número é obrigatório.");
            if (string.IsNullOrWhiteSpace(Address.Neighborhood))
                errors.Add("Bairro é obrigatório.");
            if (string.IsNullOrWhiteSpace(Address.City))
                errors.Add("Cidade é obrigatória.");
            if (string.IsNullOrWhiteSpace(Address.State))
                errors.Add("Estado é obrigatório.");
            if (string.IsNullOrWhiteSpace(Address.ZipCode))
                errors.Add("CEP é obrigatório.");
        }

        var hasMobile = !string.IsNullOrWhiteSpace(Phone.Mobile);
        var hasLandline = !string.IsNullOrWhiteSpace(Phone.Landline);

        if (hasMobile && !StringUtils.IsValidPhone(Phone.Mobile))
            errors.Add("O número de celular informado é inválido.");

        if (hasLandline && !StringUtils.IsValidPhone(Phone.Landline))
            errors.Add("O número de telefone fixo informado é inválido.");

        return errors;
    }
    public bool IsValid() => !Validate().Any();
}
