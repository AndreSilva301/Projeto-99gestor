namespace ManiaDeLimpeza.Application.Dtos;
public class ResetPasswordRequestDto
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string NewPasswordConfirm { get; set; } = string.Empty;

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Token))
            errors.Add("Token é obrigatório.");

        if (string.IsNullOrWhiteSpace(NewPassword))
            errors.Add("A nova senha é obrigatória.");
        else if (NewPassword.Length < 6)
            errors.Add("A nova senha deve ter pelo menos 6 caracteres.");

        if (NewPassword != NewPasswordConfirm)
            errors.Add("As senhas não conferem.");

        return errors;
    }
    public bool IsValid() => !Validate().Any();
}

