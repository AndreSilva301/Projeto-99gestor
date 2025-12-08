using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Persistence.Repositories;
public class QuoteItemRepository : IQuoteItemRepository, IScopedDependency
{
    private readonly ApplicationDbContext _context;

    public QuoteItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<QuoteItem>> GetByQuoteIdAsync(int quoteId)
    {
        bool quoteExists = await _context.Quotes.AnyAsync(q => q.Id == quoteId);
        if (!quoteExists)
            throw new KeyNotFoundException($"Quote com ID {quoteId} não foi encontrado.");

        return await _context.QuoteItems
            .Where(qi => qi.QuoteId == quoteId)
            .OrderBy(qi => qi.Order)
            .ToListAsync();
    }

    public async Task<QuoteItem?> GetByIdAsync(int id)
    {
        return await _context.QuoteItems.FindAsync(id);
    }

    public async Task<QuoteItem> CreateAsync(QuoteItem item)
    {
        bool quoteExists = await _context.Quotes.AnyAsync(q => q.Id == item.QuoteId);
        if (!quoteExists)
            throw new KeyNotFoundException($"Quote com ID {item.QuoteId} não foi encontrado.");

        _context.QuoteItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<QuoteItem> UpdateAsync(QuoteItem item)
    {
        var existing = await _context.QuoteItems.FindAsync(item.Id);
        if (existing == null)
            throw new KeyNotFoundException($"QuoteItem com ID {item.Id} não foi encontrado.");

        _context.Entry(existing).CurrentValues.SetValues(item);
        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.QuoteItems.FindAsync(id);
        if (existing == null)
            return false;

        _context.QuoteItems.Remove(existing);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ReorderAsync(int quoteId, List<int> itemIdsInOrder)
    {
        bool quoteExists = await _context.Quotes.AnyAsync(q => q.Id == quoteId);
        if (!quoteExists)
            throw new KeyNotFoundException($"Quote com ID {quoteId} não foi encontrado.");

        var items = await _context.QuoteItems
            .Where(qi => qi.QuoteId == quoteId && itemIdsInOrder.Contains(qi.Id))
            .ToListAsync();

        if (items.Count != itemIdsInOrder.Count)
            throw new InvalidOperationException("Nem todos os itens foram encontrados para reordenação.");

        for (int i = 0; i < itemIdsInOrder.Count; i++)
        {
            var item = items.First(qi => qi.Id == itemIdsInOrder[i]);
            item.Order = i + 1;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}