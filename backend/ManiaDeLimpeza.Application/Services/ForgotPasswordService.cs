using ManiaDeLimpeza.Api.Auth;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Infrastructure.Exceptions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;



namespace ManiaDeLimpeza.Application.Services;
public class ForgotPasswordService : IForgotPasswordService, IScopedDependency
{
    private readonly IPasswordResetRepository _passwordResetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IEmailServices _emailSenderService;
    private readonly int _expirationInMinutes;


    public ForgotPasswordService(
        IPasswordResetRepository passwordResetRepository,
        IUserRepository userRepository,
        IUserService userService,
        IEmailServices emailSenderService,
        IOptions<ResetPasswordOptions> resetPasswordOptions)
    {
        _passwordResetRepository = passwordResetRepository;
        _userRepository = userRepository;
        _emailSenderService = emailSenderService;
        _expirationInMinutes = resetPasswordOptions.Value.TokenExpirationInMinutes;
        _userService = userService;
    }

    public async Task SendResetPasswordEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return;

        var token = GenerateSecureToken();
        var expiration = DateTime.UtcNow.AddMinutes(_expirationInMinutes);

        var resetToken = new PasswordResetToken
        {
            Token = token,
            Expiration = expiration,
            UserId = user.Id,
            User = user
        };

        await _passwordResetRepository.AddAsync(resetToken);

        await _emailSenderService.SendForgetPasswordEmail(user.Email, token);
    }
    private static string GenerateSecureToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes);
    }
    public async Task<PasswordResetToken?> VerifyResetTokenAsync(string token)
    {
        var tokenEntity = await _passwordResetRepository.GetByTokenAsync(token);

        if (tokenEntity == null || tokenEntity.Expiration < DateTime.UtcNow)
            return null;

        return tokenEntity;
    }

    public async Task<ApiResponse<string>> ResetAsync(ResetPasswordRequestDto dto)
    {
        if (!dto.IsValid())
            return ApiResponseHelper.ErrorResponse(string.Join(", ", dto.Validate()));

        var tokenData = await _passwordResetRepository.GetByTokenAsync(dto.Token);

        if (tokenData == null || tokenData.Expiration < DateTime.UtcNow)
            return ApiResponseHelper.ErrorResponse("Token inválido ou expirado");

        var user = tokenData.User;
        if (user == null)
            return ApiResponseHelper.ErrorResponse("Usuário não encontrado para o token informado.");

        try
        {
            await _userService.UpdatePasswordAsync(user, dto.NewPassword);
            return ApiResponseHelper.SuccessResponse("Senha redefinida com sucesso");
        }
        catch (BusinessException ex)
        {
            return ApiResponseHelper.ErrorResponse(ex.Message);
        }
    }
}
