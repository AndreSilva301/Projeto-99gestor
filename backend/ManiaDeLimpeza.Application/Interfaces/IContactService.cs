using ManiaDeLimpeza.Application.Dtos;

namespace ManiaDeLimpeza.Application.Interfaces;
public interface IContactService
{
    Task ProcessContactAsync(ContactRequestDto dto);
}
