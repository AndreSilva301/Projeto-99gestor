using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Application.Services;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using ManiaDeLimpeza.Api.Auth;



namespace ManiaDeLimpeza.Application.Services;
public class ForgotPasswordService : IForgotPasswordService, IScopedDependency
{
    private readonly IPasswordResetRepository _passwordResetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailServices _emailSenderService;
    private readonly int _expirationInMinutes;
   

    public ForgotPasswordService(
        IPasswordResetRepository passwordResetRepository,
        IUserRepository userRepository,
        IEmailServices emailSenderService,
        IOptions<ResetPasswordOptions> resetPasswordOptions)
    {
        _passwordResetRepository = passwordResetRepository;
        _userRepository = userRepository;
        _emailSenderService = emailSenderService;
        _expirationInMinutes = resetPasswordOptions.Value.TokenExpirationInMinutes;
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
}
