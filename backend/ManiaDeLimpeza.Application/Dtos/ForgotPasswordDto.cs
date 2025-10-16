using ManiaDeLimpeza.Application.Common;

namespace ManiaDeLimpeza.Application.Dtos;
public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (!StringUtils.IsValidEmail(Email))
            errors.Add("E-mail inválido.");

        return errors;
    }

    public bool IsValid() => Validate().Count == 0;
}