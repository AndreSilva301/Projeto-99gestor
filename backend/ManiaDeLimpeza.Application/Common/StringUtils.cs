using System.Text.RegularExpressions;

namespace ManiaDeLimpeza.Application.Common;
public static class StringUtils
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    private static readonly Regex PasswordRegex = new Regex(
        @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z0-9@#$%^&*!+=\-._]{8,}$",
        RegexOptions.Compiled
    );

    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email.Trim());
    }

    public static bool IsValidPassword(this string password)
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

    public static bool IsValidCNPJ(this string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;
        cnpj = Regex.Replace(cnpj, @"[^\d]", "");
        if (cnpj.Length != 14)
            return false;
        int[] multipliers1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multipliers2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCnpj = cnpj.Substring(0, 12);
        int sum = 0;
        for (int i = 0; i < 12; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multipliers1[i];
        int remainder = sum % 11;
        int firstDigit = remainder < 2 ? 0 : 11 - remainder;
        tempCnpj += firstDigit;
        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multipliers2[i];
        remainder = sum % 11;
        int secondDigit = remainder < 2 ? 0 : 11 - remainder;
        return cnpj.EndsWith(firstDigit.ToString() + secondDigit.ToString());
    }
}