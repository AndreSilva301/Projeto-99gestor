using ManiaDeLimpeza.Application.Common;

namespace ManiaDeLimpeza.Application.Dtos;
public class ForgotPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (!StringUtils.IsValidEmail(Email))
            errors.Add("E-mail inválido.");

        return errors;
    }

    public (bool IsValid, string ErrorMessage) IsValid()
    {
        if (string.IsNullOrWhiteSpace(Email))
            return (false, "O campo de e-mail é obrigatório.");

        if (!Email.Contains("@"))
            return (false, "O e-mail informado é inválido.");

        return (true, string.Empty);
    }
}