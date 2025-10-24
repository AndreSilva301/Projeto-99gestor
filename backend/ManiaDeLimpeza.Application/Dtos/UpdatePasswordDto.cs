using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Domain.Interfaces;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdatePasswordDto : IBasicDto
{
    public string Email { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Email))
            errors.Add("E-mail é obrigatório.");
        else if (!Email.IsValidEmail())
            errors.Add("E-mail inválido.");

        if (string.IsNullOrWhiteSpace(CurrentPassword))
            errors.Add("Senha atual é obrigatória.");

        if (string.IsNullOrWhiteSpace(NewPassword))
            errors.Add("Nova senha é obrigatória.");
        else if (NewPassword.Length < 8)
            errors.Add("A nova senha deve ter pelo menos 8 caracteres.");

        if (NewPassword != ConfirmNewPassword)
            errors.Add("A confirmação da nova senha não confere.");

        return errors;
    }

    public bool IsValid()
    {
        return Validate().Count == 0;
    }
}

