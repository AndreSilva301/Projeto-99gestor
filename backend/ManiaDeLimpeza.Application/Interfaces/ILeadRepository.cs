using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Application.Interfaces;
public interface ILeadRepository
{
    Task AddAsync(Lead lead);
    Task SaveChangesAsync();
}
