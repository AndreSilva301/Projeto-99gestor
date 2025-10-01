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

        public Task<Client?> GetByIdAsync(int id)
        {
            return _clientRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Client>> GetAllAsync()
        {
            return _clientRepository.GetAllAsync();
        }

        public Task AddAsync(Client client)
        {
            return _clientRepository.AddAsync(client);
        }

        public Task UpdateAsync(Client client)
        {
            return _clientRepository.UpdateAsync(client);
        }

        public Task DeleteAsync(Client client)
        {
            return _clientRepository.DeleteAsync(client);
        }

        public Task<List<Client>> SearchAsync(string searchTerm)
        {
            return _clientRepository.SearchAsync(searchTerm);
        }

        public Task<PagedResult<Client>> PaginatedSearchAsync(string searchTerm, int page, int pageSize)
        {
            return _clientRepository.SearchPagedAsync(searchTerm, page, pageSize);
        }
    }
}
