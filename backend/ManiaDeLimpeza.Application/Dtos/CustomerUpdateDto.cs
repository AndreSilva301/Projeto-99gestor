using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Domain.Interfaces;

namespace ManiaDeLimpeza.Application.Dtos;
public class CustomerUpdateDto : IBasicDto
{
    public string Name { get; set; } = string.Empty;
    public AddressDto Address { get; set; } = new AddressDto();
    public PhoneDto Phone { get; set; } = new PhoneDto();
    public string Email { get; set; } = string.Empty;
    public string? Observations { get; set; }
    public List<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("O nome é obrigatório.");
        if (Name?.Length > 255)
            errors.Add("O nome deve ter no máximo 255 caracteres.");

        // Email is optional on update; validate only if provided
        if (!string.IsNullOrWhiteSpace(Email) && !Email.IsValidEmail())
            errors.Add("E-mail não é válido.");

        // Phone is optional on update; validate Mobile only if provided
        if (Phone != null && !string.IsNullOrWhiteSpace(Phone.Mobile) && !Phone.Mobile.IsValidPhone())
            errors.Add("Número de telefone inválido.");
        return errors;
    }

    public bool IsValid() => Validate().Count == 0;
}