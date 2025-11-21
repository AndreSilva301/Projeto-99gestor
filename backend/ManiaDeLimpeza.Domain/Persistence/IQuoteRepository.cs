using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Domain.Persistence
{
    public interface IQuoteRepository : IBaseRepository<Quote>
    {
        Task<PagedResult<Quote>> GetPagedAsync(
            string? searchTerm,
            DateTime? createdAtStart,
            DateTime? createdAtEnd,
            string sortBy,
            bool sortDescending,
            int page,
            int pageSize,
            int companyId);

        Task<Quote?> GetByIdAsync(int id, int companyId);
        Task<Quote> CreateAsync(Quote quote, int companyId);
        Task<Quote> UpdateAsync(Quote quote, int companyId);
        Task<bool> DeleteAsync(int id, int companyId);
        Task<bool> ExistsAsync(int id, int companyId);
    }
}