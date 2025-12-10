using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ManiaDeLimpeza.Application.Services;

public class ContactService : IContactService, IScopedDependency
{
    private readonly IEmailSender _emailSender;

    public ContactService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task ProcessContactAsync(ContactRequestDto dto)
    {
        var subject = $"Novo contato de {dto.Name}";
        var body = $@"
            Nome: {dto.Name}
            Email: {dto.Email}
            Telefone: {dto.Phone}
            Interesse: {dto.Interest}

            Mensagem:
            {dto.Message}
        ";

        await _emailSender.SendEmailAsync("contato@suasite.com", subject, body);
    }
}