using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;

namespace ManiaDeLimpeza.Application.Services;
public class EmailSenderServices : IEmailSenderServices, IScopedDependency
{
public Task SendResetPasswordEmailAsync(string email, string token)
    {
        Console.WriteLine($"Simulating sending email to {email} with token: {token}");
        return Task.CompletedTask;
    }
}
