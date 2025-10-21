using ManiaDeLimpeza.Application.Interfaces;
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
}
