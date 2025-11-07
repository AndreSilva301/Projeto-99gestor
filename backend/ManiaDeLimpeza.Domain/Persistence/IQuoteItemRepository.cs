using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Domain.Persistence;
public interface IQuoteItemRepository
{
    Task<IEnumerable<QuoteItem>> GetByQuoteIdAsync(int quoteId);
    Task<QuoteItem?> GetByIdAsync(int id);
    Task<QuoteItem> CreateAsync(QuoteItem item);
    Task<QuoteItem> UpdateAsync(QuoteItem item);
    Task<bool> DeleteAsync(int id);
    Task<bool> ReorderAsync(int quoteId, List<int> itemIdsInOrder);
}
