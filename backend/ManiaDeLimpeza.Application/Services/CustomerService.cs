using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Infrastructure.Exceptions;
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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CustomerService(
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<CustomerDto> CreateAsync(CustomerCreateDto dto, int currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (!dto.IsValid())
                throw new BusinessException(string.Join(", ", dto.Validate()));

            var entity = _mapper.Map<Customer>(dto);
            entity.CompanyId = user.CompanyId;
            entity.CreatedDate = DateTime.UtcNow;

            await _customerRepository.AddAsync(entity); 

            if (dto.Relationships?.Any() == true)
            {
                var relationships = _mapper.Map<List<CustomerRelationship>>(dto.Relationships);
                await _customerRepository.AddOrUpdateRelationshipsAsync(entity.Id, relationships);
            }

            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<CustomerDto> UpdateAsync(int customerId, CustomerUpdateDto dto, int currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            var existing = await _customerRepository.GetbyIdWithRelationshipAsync(customerId);

            if (existing.CompanyId != user.CompanyId)
                throw new BusinessException("Cliente não pertence à empresa do usuário.");

            if (!dto.IsValid())
                throw new BusinessException(string.Join(", ", dto.Validate()));

            _mapper.Map(dto, existing);
            existing.UpdatedDate = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(existing);

            return _mapper.Map<CustomerDto>(existing);
        }

        public async Task SoftDeleteAsync(int customerId, int currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            var existing = await _customerRepository.GetByIdAsync(customerId);

            if (existing.CompanyId != user.CompanyId)
                throw new BusinessException("Cliente não pertence à empresa do usuário.");

            await _customerRepository.SoftDeleteAsync(customerId);
        }

        public async Task<CustomerDto?> GetByIdAsync(int customerId, int currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            var customer = await _customerRepository.GetbyIdWithRelationshipAsync(customerId);

            if (customer.CompanyId != user.CompanyId)
                return null;

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<PagedResult<CustomerListItemDto>> SearchAsync(string? searchTerm, int page, int pageSize, int companyId)
        {
            var result = await _customerRepository.GetPagedByCompanyAsync(companyId, page, pageSize, searchTerm ?? "");

            return new PagedResult<CustomerListItemDto>
            {
                Items = result.Items.Select(_mapper.Map<CustomerListItemDto>).ToList(),
                TotalItems = result.TotalItems
            };
        }
    }
}
