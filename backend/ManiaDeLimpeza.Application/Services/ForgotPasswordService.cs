using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace ManiaDeLimpeza.Application.Services;
public class ForgotPasswordService : IForgotPasswordService, IScopedDependency
{

    private readonly IPasswordResetRepository _passwordResetRepository;
    private readonly IUserRepository _userRepository; 
      
    public ForgotPasswordService(
        IPasswordResetRepository passwordResetRepository,
        IUserRepository userRepository)
    {
        _passwordResetRepository = passwordResetRepository;
        _userRepository = userRepository;
      
    }

    public async Task SendResetPasswordEmailAsync(string email)
    {
        // Tenta encontrar o usuário (não lança exceção se não encontrar)
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return;

        // Gera sempre um token novo
        var token = GenerateSecureToken();
        var expiration = DateTime.UtcNow.AddMinutes(30);

        var resetToken = new PasswordResetToken
        {
            Token = token,
            Expiration = expiration,
            User = user
        };

        await _passwordResetRepository.AddAsync(resetToken);

        var link = $"https://seusite.com/reset-password?token={token}";
        var subject = "Recuperação de senha";
        var body = $"Olá {user.Name},\n\nUse o link abaixo para redefinir sua senha:\n{link}\n\nEste link expira em 30 minutos.";
        
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes);
    }
}
