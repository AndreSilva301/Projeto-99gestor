using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ManiaDeLimpeza.Application.Interfaces;
public interface IForgotPasswordService
{
    Task SendResetPasswordEmailAsync(string email);
    Task<PasswordResetToken?> VerifyResetTokenAsync(string token);
}