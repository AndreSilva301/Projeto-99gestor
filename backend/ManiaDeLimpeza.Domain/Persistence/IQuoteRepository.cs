using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Domain.Persistence;
public interface IQuoteRepository : IBaseRepository<Quote>
{
    Task<IEnumerable<Quote>> GetAllAsync(
        int? customerId = null,
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10
    );

    Task<Quote?> GetByIdAsync(int id);
    Task<Quote> CreateAsync(Quote quote);
    Task<Quote> UpdateAsync(Quote quote);
    Task<bool> DeleteAsync(int id);
    Task<int> CountAsync(int? customerId = null, int? userId = null);
    Task<bool> ExistsAsync(int id);
}