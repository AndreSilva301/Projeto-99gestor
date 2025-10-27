using System.Diagnostics.CodeAnalysis;

namespace ManiaDeLimpeza.Api.Auth;

[ExcludeFromCodeCoverage]
public class ResetPasswordOptions
{
    public const string SECTION = "ResetPasswordOptions";
    public int TokenExpirationInMinutes { get; set; } = 30;
}
