using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ManiaDeLimpeza.Application.Services;
public class EmailSendServices : IEmailServices, IScopedDependency
{
    private readonly ILogger<EmailSendServices> _logger;

    public EmailSendServices(ILogger<EmailSendServices> logger)
    {
        _logger = logger;
    }

    public Task SendForgetPasswordEmail(string email, string token)
    {
        _logger.LogInformation("[Simulação] E-mail de recuperação de senha enviado para: {email}", email);
        _logger.LogInformation("[Simulação] Link de redefinição: {token}", token);

        return Task.CompletedTask;
    }
}
