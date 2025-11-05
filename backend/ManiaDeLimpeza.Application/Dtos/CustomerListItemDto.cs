using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Domain.Interfaces;

namespace ManiaDeLimpeza.Application.Dtos;
public class CustomerListItemDto : IBasicDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PhoneDto Phone { get; set; } = new PhoneDto();
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Nome é obrigatório.");
        if (Name?.Length > 255)
            errors.Add("O nome pode ter no máximo 255 caracteres.");
        if (!Phone?.Mobile.IsValidPhone() ?? true)
            errors.Add("Número de telefone inválido.");
        return errors;
    }

    public bool IsValid() => Validate().Count == 0;
}
