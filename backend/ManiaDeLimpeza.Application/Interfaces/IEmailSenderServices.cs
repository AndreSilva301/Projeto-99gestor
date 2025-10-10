namespace ManiaDeLimpeza.Application.Interfaces;
public interface IEmailSenderServices
{
    Task SendResetPasswordEmailAsync(string email, string token);
}
