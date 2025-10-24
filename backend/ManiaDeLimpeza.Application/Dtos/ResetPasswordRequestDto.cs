using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Domain.Interfaces;

namespace ManiaDeLimpeza.Application.Dtos;
public class ResetPasswordRequestDto : IBasicDto
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string NewPasswordConfirm { get; set; } = string.Empty;

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            errors.Add("A nova senha é obrigatória.");
        }
        else if (!NewPassword.IsValidPassword())  // Fixed: negation added
        {
            errors.Add("A nova senha deve ter pelo menos 8 caracteres, contendo ao menos uma letra e um número.");
        }

        if (string.IsNullOrWhiteSpace(NewPasswordConfirm))
        {
            errors.Add("A confirmação da nova senha é obrigatória.");
        }
        else if (NewPassword != NewPasswordConfirm)
        {
            errors.Add("As senhas não coincidem.");
        }

        if (string.IsNullOrWhiteSpace(Token))
        {
            errors.Add("O token de redefinição é obrigatório.");
        }
        return errors;
    }

    public bool IsValid()
    {
        return Validate().Count == 0;
    }
}

