using ManiaDeLimpeza.Infrastructure.DependencyInjection;

namespace ManiaDeLimpeza.Application.Services;
public class EmailSendServices : IScopedDependency
{
    public Task SendForgetPasswordEmail(string email, string token)
    {
        Console.WriteLine($"[Simulação] E-mail de recuperação de senha enviado para: {email}");
        Console.WriteLine($"Link de redefinição: {token}");
        return Task.CompletedTask;
    }
}
