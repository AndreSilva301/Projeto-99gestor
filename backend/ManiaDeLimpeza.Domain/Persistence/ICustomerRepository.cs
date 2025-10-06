using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Persistence
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<List<Customer>> SearchAsync(string searchTerm);
        Task<PagedResult<Customer>> SearchPagedAsync(string searchTerm, int page, int pageSize); 

    }
}
