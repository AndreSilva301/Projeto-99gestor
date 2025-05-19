using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Interfaces
{
    public interface IClientService
    {
        Task<Client?> GetByIdAsync(int id);
        Task<IEnumerable<Client>> GetAllAsync();
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(Client client);
        Task<List<Client>> SearchAsync(string searchTerm);
    }
}
