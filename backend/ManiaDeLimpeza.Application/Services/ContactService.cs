using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Services;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ManiaDeLimpeza.Application.Services;

public class ContactService : IContactService, IScopedDependency
{
    private readonly IEmailServices _emailServices;

    public ContactService(IEmailServices emailServices)
    {
        _emailServices = emailServices;
    }

    public async Task ProcessContactAsync(ContactRequestDto dto)
    {
        var subject = $"Novo contato de {dto.Name}";
        var body = $@"
            Nome: {dto.Name}
            Email: {dto.Email}
            Telefone: {dto.Phone}
            Interesse: {dto.Interest.ToString()}

            Mensagem:
            {dto.Message}
        ";

        await _emailServices.SendContactEmail("contato@suasite.com", subject, body);
    }
}