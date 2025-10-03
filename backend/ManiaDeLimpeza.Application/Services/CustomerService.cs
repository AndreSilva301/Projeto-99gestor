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
    public class CustomerService : ICustomerService, IScopedDependency
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public Task<Customer?> GetByIdAsync(int id)
        {
            return _customerRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Customer>> GetAllAsync()
        {
            return _customerRepository.GetAllAsync();
        }

        public Task AddAsync(Customer customer)
        {
            return _customerRepository.AddAsync(customer);
        }

        public Task UpdateAsync(Customer customer)
        {
            return _customerRepository.UpdateAsync(customer);
        }

        public Task DeleteAsync(Customer customer)
        {
            return _customerRepository.DeleteAsync(customer);
        }

        public Task<List<Customer>> SearchAsync(string searchTerm)
        {
            return _customerRepository.SearchAsync(searchTerm);
        }

        public Task<PagedResult<Customer>> PaginatedSearchAsync(string searchTerm, int page, int pageSize)
        {
            return _customerRepository.SearchPagedAsync(searchTerm, page, pageSize);
        }
    }
}
