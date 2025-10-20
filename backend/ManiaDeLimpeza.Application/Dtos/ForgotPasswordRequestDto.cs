using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Domain.Interfaces;

namespace ManiaDeLimpeza.Application.Dtos;
public class ForgotPasswordRequestDto : IBasicDto
{
    public string Email { get; set; } = string.Empty;

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Email))
            errors.Add("E-mail é obrigatório.");
        else if (!Email.IsValidEmail())  // Fixed: negation added
            errors.Add("E-mail inválido.");

        return errors;
    }

    public bool IsValid()
    {
        return Validate().Count == 0;
    }

}