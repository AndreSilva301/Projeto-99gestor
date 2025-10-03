using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Services
{
    public class ClientService : IClientService, IScopedDependency
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public Task<Customer?> GetByIdAsync(int id)
        {
            return _clientRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Customer>> GetAllAsync()
        {
            return _clientRepository.GetAllAsync();
        }

        public Task AddAsync(Customer client)
        {
            return _clientRepository.AddAsync(client);
        }

        public Task UpdateAsync(Customer client)
        {
            return _clientRepository.UpdateAsync(client);
        }

        public Task DeleteAsync(Customer client)
        {
            return _clientRepository.DeleteAsync(client);
        }

        public Task<List<Customer>> SearchAsync(string searchTerm)
        {
            return _clientRepository.SearchAsync(searchTerm);
        }

        public Task<PagedResult<Customer>> PaginatedSearchAsync(string searchTerm, int page, int pageSize)
        {
            return _clientRepository.SearchPagedAsync(searchTerm, page, pageSize);
        }
    }
}
