using System.Text.RegularExpressions;

namespace ManiaDeLimpeza.Application.Common;
public static class StringUtils
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    private static readonly Regex PasswordRegex = new Regex(
        @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
        RegexOptions.Compiled
    );

    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email.Trim());
    }

    public static bool ValidatePassword(this string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return PasswordRegex.IsMatch(password);
    }

    public static bool IsValidPhone(this string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        var pattern = @"^[\d\s\-\+\(\)]+$";
        if (!Regex.IsMatch(phone, pattern))
            return false;

        int digitCount = phone.Count(char.IsDigit);
        return digitCount >= 8 && digitCount <= 11;
    }
}