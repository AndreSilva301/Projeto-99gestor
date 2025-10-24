using ManiaDeLimpeza.Domain.Entities;
using System.Reflection.Metadata;

namespace ManiaDeLimpeza.Domain.Services;
public interface IEmailServices 
{
    Task SendForgetPasswordEmail(string email, string token);
    Task SendingAnInvitation(string name, string email, UserProfile userProfile);
}
