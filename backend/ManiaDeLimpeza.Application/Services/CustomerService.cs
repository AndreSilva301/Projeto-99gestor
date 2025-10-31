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

        public async Task<CustomerDto> CreateAsync(CustomerCreateDto dto, int companyId)
        {
            if (!dto.IsValid())
                throw new BusinessException(string.Join(", ", dto.Validate()));

            var entity = _mapper.Map<Customer>(dto);
            entity.CompanyId = companyId;
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

                    await _customerRepository.AddOrUpdateRelationshipsAsync(entity.Id, new List<CustomerRelationship> { relationship });
                }
            }

            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<CustomerDto> UpdateAsync(int customerId, CustomerUpdateDto dto, int companyId)
        {
            var existing = await _customerRepository.GetbyIdWithRelationshipAsync(customerId);

            if (existing.CompanyId != companyId)
                throw new BusinessException("Cliente não pertence à empresa do usuário.");

            if (!dto.IsValid())
                throw new BusinessException(string.Join(", ", dto.Validate()));

            _mapper.Map(dto, existing);
            existing.UpdatedDate = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(existing);

            return _mapper.Map<CustomerDto>(existing);
        }

        public async Task SoftDeleteAsync(int customerId, int companyId)
        {
            var existing = await _customerRepository.GetByIdAsync(customerId);

            if (existing.CompanyId != companyId)
                throw new BusinessException("Cliente não pertence à empresa do usuário.");

            await _customerRepository.SoftDeleteAsync(customerId);
        }

        public async Task<CustomerDto?> GetByIdAsync(int customerId, int companyId)
        {
            var customer = await _customerRepository.GetbyIdWithRelationshipAsync(customerId);

            if (customer.CompanyId != companyId)
                return null;

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<PagedResult<CustomerListItemDto>> SearchAsync(string? searchTerm, int page, int pageSize, int companyId, string? orderBy = "Name", string direction = "asc")
        {
            var result = await _customerRepository.GetPagedByCompanyAsync(companyId, page, pageSize, searchTerm ?? "", orderBy ?? "Name", direction.ToLower() == "desc" ? "desc" : "asc");

            return new PagedResult<CustomerListItemDto>
            {
                Items = result.Items.Select(_mapper.Map<CustomerListItemDto>).ToList(),
                TotalItems = result.TotalItems
            };
        }

        public async Task<IEnumerable<CustomerRelationshipDto>> AddOrUpdateRelationshipsAsync(int customerId, IEnumerable<CustomerRelationshipDto> dtos, int companyId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId)
         ?? throw new BusinessException("Customer não encontrado.");

            if (customer.CompanyId != companyId)
                throw new BusinessException("Customer não pertence à sua empresa.");

            var invalidDtos = dtos.Where(d => !d.IsValid()).ToList();
            if (invalidDtos.Any())
                throw new BusinessException($"Dados inválidos: {string.Join(", ", invalidDtos.SelectMany(d => d.Validate()))}");

            var relationships = new List<CustomerRelationship>();

            foreach (var dto in dtos)
            {
                var entity = _mapper.Map<CustomerRelationship>(dto);

                entity.CustomerId = customerId;

                if (dto.Id > 0)
                {
                    entity.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    entity.CreatedDate = DateTime.UtcNow;
                }

                relationships.Add(entity);
            }

            var updatedRelationships = await _customerRepository.AddOrUpdateRelationshipsAsync(customerId, relationships);

            return relationships.Select(_mapper.Map<CustomerRelationshipDto>);
        }

        public async Task<IEnumerable<CustomerRelationshipDto>> ListRelationshipsAsync(int customerId, int companyId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId)
                ?? throw new BusinessException("Customer não encontrado.");

            if (customer.CompanyId != companyId)
                throw new BusinessException("Customer não pertence à sua empresa.");

            var relationships = await _customerRepository.GetRelationshipsByCustomerAsync(customerId);
            return relationships
                .OrderByDescending(r => r.DateTime)
                .Select(_mapper.Map<CustomerRelationshipDto>);
        }

        public async Task DeleteRelationshipsAsync(int customerId, IEnumerable<int> relationshipIds, int companyId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId)
                ?? throw new BusinessException("Customer não encontrado.");

            if (customer.CompanyId != companyId)
                throw new BusinessException("Customer não pertence à sua empresa.");

            var relationships = await _customerRepository.GetRelationshipsByCustomerAsync(customerId);
            var invalidIds = relationshipIds.Except(relationships.Select(r => r.Id)).ToList();

            if (invalidIds.Any())
                throw new BusinessException($"Os relationships [{string.Join(", ", invalidIds)}] não pertencem ao customer {customerId}.");

            await _customerRepository.DeleteRelationshipsAsync(relationshipIds, customerId);
        }
    }
}
