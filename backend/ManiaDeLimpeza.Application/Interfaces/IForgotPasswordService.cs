namespace ManiaDeLimpeza.Application.Interfaces;
public interface IForgotPasswordService
{
    Task SendResetPasswordEmailAsync(string email);
}