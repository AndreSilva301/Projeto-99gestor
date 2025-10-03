using ManiaDeLimpeza.Application.Dtos;
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
        Task<Customer?> GetByIdAsync(int id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer client);
        Task UpdateAsync(Customer client);
        Task DeleteAsync(Customer client);
        Task<List<Customer>> SearchAsync(string searchTerm);
        Task<PagedResult<Customer>> PaginatedSearchAsync(string searchTerm, int page, int pageSize);
    }
}
