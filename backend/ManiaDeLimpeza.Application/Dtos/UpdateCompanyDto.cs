using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateCompanyDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? CNPJ { get; set; }

    [Required]
    public AddressDto Address { get; set; } = new AddressDto();

    [Required]
    public PhoneDto Phone { get; set; } = new PhoneDto();

    public IEnumerable<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("O nome da empresa é obrigatório.");

        if (Address == null)
            errors.Add("O endereço é obrigatório.");
        else
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

        if (Phone == null)
        {
            errors.Add("O telefone é obrigatório.");
        }
        else
        {
            var hasMobile = !string.IsNullOrWhiteSpace(Phone.Mobile);
            var hasLandline = !string.IsNullOrWhiteSpace(Phone.Landline);

            if (!hasMobile && !hasLandline)
                errors.Add("É necessário informar pelo menos um telefone (celular ou fixo).");
        }
        return errors;
    }
    public bool IsValid() => !Validate().Any();
}
