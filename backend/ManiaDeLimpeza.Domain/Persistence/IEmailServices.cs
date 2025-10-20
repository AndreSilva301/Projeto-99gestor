using System.Reflection.Metadata;

namespace ManiaDeLimpeza.Domain.Persistence;
public interface IEmailServices 
{
    Task SendForgetPasswordEmail(string email, string token);
}
