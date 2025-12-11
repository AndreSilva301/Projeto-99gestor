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
        _logger.LogInformation("[Simulação] Enviando convite de acesso...");

        return Task.CompletedTask;
    }
    public Task SendContactEmail(string to, string subject, string body)
    {
        _logger.LogInformation("[Simulação] Email de contato enviado para: {to}", to);
        _logger.LogInformation("[Simulação] Assunto: {subject}", subject);
        _logger.LogInformation("[Simulação] Corpo da mensagem:\n{body}", body);

        return Task.CompletedTask;
    }
}
