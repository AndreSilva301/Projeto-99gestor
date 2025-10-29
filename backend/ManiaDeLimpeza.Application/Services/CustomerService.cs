using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Infrastructure.Exceptions;

namespace ManiaDeLimpeza.Application.Services
{
    public class CustomerService : ICustomerService, IScopedDependency
    {
        private readonly ICustomerRelationshipRepository _relationshipRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public CustomerService(
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            ICustomerRelationshipRepository relationshipRepository,
            IMapper mapper)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _relationshipRepository = relationshipRepository;
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
                foreach (var relationshipDto in dto.Relationships)
                {
                    if (!relationshipDto.IsValid())
                        throw new BusinessException(string.Join(", ", relationshipDto.Validate()));

                    var relationship = _mapper.Map<CustomerRelationship>(relationshipDto);
                    relationship.CustomerId = entity.Id;
                    relationship.CreatedDate = DateTime.UtcNow;

                    await _relationshipRepository.AddAsync(relationship);
                }
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

        public async Task<IEnumerable<CustomerRelationshipDto>> AddOrUpdateRelationshipsAsync( int customerId, IEnumerable<CustomerRelationshipCreateOrUpdateDto> dtos, int currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            var customer = await _customerRepository.GetByIdAsync(customerId)
                ?? throw new BusinessException("Customer não encontrado.");

            if (customer.CompanyId != user.CompanyId)
                throw new BusinessException("Customer não pertence à sua empresa.");

            var results = new List<CustomerRelationship>();

            foreach (var dto in dtos)
            {
                if (!dto.IsValid())
                    throw new BusinessException($"Dados inválidos: {string.Join(", ", dto.Validate())}");

                CustomerRelationship entity;

                if (dto.Id > 0)
                {
                    entity = await _relationshipRepository.GetByIdAsync(dto.Id)
                        ?? throw new BusinessException($"Relationship {dto.Id} não encontrado.");

                    if (entity.CustomerId != customerId)
                        throw new BusinessException($"Relationship {dto.Id} não pertence ao Customer {customerId}.");

                    _mapper.Map(dto, entity);
                    entity.UpdatedDate = DateTime.UtcNow;

                    await _relationshipRepository.UpdateAsync(entity);
                }
                else
                {
                    entity = _mapper.Map<CustomerRelationship>(dto);
                    entity.CustomerId = customerId;
                    entity.CreatedDate = DateTime.UtcNow;

                    await _relationshipRepository.AddAsync(entity);
                }

                results.Add(entity);
            }

            return results.Select(_mapper.Map<CustomerRelationshipDto>);
        }

        public async Task<IEnumerable<CustomerRelationshipDto>> ListRelationshipsAsync(int customerId, int currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            var customer = await _customerRepository.GetByIdAsync(customerId)
                ?? throw new BusinessException("Customer não encontrado.");

            if (customer.CompanyId != user.CompanyId)
                throw new BusinessException("Customer não pertence à sua empresa.");

            var relationships = await _relationshipRepository.GetByCustomerIdAsync(customerId);
            return relationships
                .OrderByDescending(r => r.DateTime)
                .Select(_mapper.Map<CustomerRelationshipDto>);
        }

        public async Task DeleteRelationshipsAsync(int customerId, IEnumerable<int> relationshipIds, int currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            var customer = await _customerRepository.GetByIdAsync(customerId)
                ?? throw new BusinessException("Customer não encontrado.");

            if (customer.CompanyId != user.CompanyId)
                throw new BusinessException("Customer não pertence à sua empresa.");

            var relationships = await _relationshipRepository.GetByCustomerIdAsync(customerId);
            var invalidIds = relationshipIds.Except(relationships.Select(r => r.Id)).ToList();

            if (invalidIds.Any())
                throw new BusinessException($"Os relationships [{string.Join(", ", invalidIds)}] não pertencem ao customer {customerId}.");

            await _relationshipRepository.DeleteRelationshipsAsync(relationshipIds);
        }
    }
}
