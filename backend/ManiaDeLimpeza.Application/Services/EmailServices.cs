using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Services;
public class EmailServices : IEmailServices, IScopedDependency
{
    public Task SendForgetPasswordEmail(string email, string token)
    {
        Console.WriteLine($"[Simulação] E-mail de recuperação de senha enviado para: {email}");
        Console.WriteLine($"Link de redefinição:");
        return Task.CompletedTask;
    }
}
