using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Persistence
{
    public interface IClientRepository : IBaseRepository<Client>
    {
        Task<List<Client>> SearchAsync(string searchTerm);
        Task<PagedResult<Client>> SearchPagedAsync(string searchTerm, int page, int pageSize);
    }
}
