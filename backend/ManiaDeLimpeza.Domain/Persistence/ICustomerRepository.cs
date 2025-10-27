using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Domain.Persistence
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<List<Customer>> SearchAsync(string searchTerm);
        Task<PagedResult<Customer>> SearchPagedAsync(string searchTerm, int page, int pageSize); 
        Task<Customer?> GetbyIdWithRelationshipAsync(int id);
        Task<PagedResult<Customer>> GetPagedByCompanyAsync(int companyId, int page, int pageSize, string? searchTerm);
        Task<IEnumerable<CustomerRelationship>> GetRelationshipsByCustomerAsync(int customerId);
        Task SoftDeleteAsync(int customerId);
        Task<IEnumerable<CustomerRelationship>> AddOrUpdateRelationshipsAsync(int customerId, IEnumerable<CustomerRelationship> relationships);
        Task DeleteRelationshipsAsync(IEnumerable<int> relationshipIds, int customerId);
    }
}
