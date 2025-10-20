using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ManiaDeLimpeza.Application.Interfaces;
public interface IForgotPasswordService
{
    Task<ApiResponse<string>> ResetAsync(ResetPasswordRequestDto dto);
    Task SendResetPasswordEmailAsync(string email);
    Task<PasswordResetToken?> VerifyResetTokenAsync(string token);
}