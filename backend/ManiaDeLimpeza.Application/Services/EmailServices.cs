using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Services;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ManiaDeLimpeza.Application.Services;
public class EmailServices : IEmailServices, IScopedDependency
{
    private readonly ILogger<EmailServices> _logger;

    public EmailServices(ILogger<EmailServices> logger)
    {
        _logger = logger;
    }

    public Task SendForgetPasswordEmail(string email, string token)
    {
        _logger.LogInformation("[Simulação] E-mail de recuperação de senha enviado para: {email}", email);
        _logger.LogInformation("[Simulação] Link de redefinição: {token}", token);

        return Task.CompletedTask;
    }

    public Task SendingAnInvitation(string name, string email, UserProfile userProfile)
    {
        var temporaryPassword = Guid.NewGuid().ToString("N")[..8]; 

        var subject = $"Convite para acesso ao sistema Mania de Limpeza";
        var body = $@"
Olá {name},

Você foi convidado para acessar o sistema do 99Gestor.

Perfil: {userProfile}
E-mail de acesso: {email}
Senha temporária: {temporaryPassword}

Por favor, acesse o sistema e altere sua senha no primeiro login.

Atenciosamente,
Equipe 99Gestor
";

        _logger.LogInformation("[Simulação] Enviando convite de acesso...");
        _logger.LogInformation("Destinatário: {Email}", email);
        _logger.LogInformation("Assunto: {Subject}", subject);
        _logger.LogInformation("Corpo do e-mail:\n{Body}", body);

        return Task.CompletedTask;
    }
}
