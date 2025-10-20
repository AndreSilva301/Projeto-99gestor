namespace ManiaDeLimpeza.Api.Auth;

public class ResetPasswordOptions
{
    public const string SECTION = "ResetPasswordOptions";
    public int TokenExpirationInMinutes { get; set; } = 30;
}
