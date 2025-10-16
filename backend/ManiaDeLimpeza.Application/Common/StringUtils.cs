using System.Text.RegularExpressions;

namespace ManiaDeLimpeza.Application.Common;
public class StringUtils
{

    private static readonly Regex EmailRegex = new Regex(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email.Trim());
    }
}