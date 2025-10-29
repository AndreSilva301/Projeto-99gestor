using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto> CreateAsync(CustomerCreateDto dto, int currentUserId);
        Task<CustomerDto> UpdateAsync(int customerId, CustomerUpdateDto dto, int currentUserId);
        Task SoftDeleteAsync(int customerId, int currentUserId);
        Task<CustomerDto?> GetByIdAsync(int customerId, int currentUserId);
        Task<PagedResult<CustomerListItemDto>> SearchAsync(string? searchTerm, int page, int pageSize, int companyId);
        Task<IEnumerable<CustomerRelationshipDto>> AddOrUpdateRelationshipsAsync(int customerId, IEnumerable<CustomerRelationshipCreateOrUpdateDto> dtos, int currentUserId);
        Task<IEnumerable<CustomerRelationshipDto>> ListRelationshipsAsync(int customerId,int currentUserId);
        Task DeleteRelationshipsAsync( int customerId, IEnumerable<int> relationshipIds, int currentUserId);
    }
}
