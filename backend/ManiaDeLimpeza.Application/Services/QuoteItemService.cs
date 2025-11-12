using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;

namespace ManiaDeLimpeza.Application.Services;
public class QuoteItemService : IQuoteItemService, IScopedDependency
{
    private readonly IQuoteItemRepository _quoteItemRepository;
    private readonly IQuoteRepository _quoteRepository;

    public QuoteItemService(
        IQuoteItemRepository quoteItemRepository,
        IQuoteRepository quoteRepository)
    {
        _quoteItemRepository = quoteItemRepository;
        _quoteRepository = quoteRepository;
    }

    public async Task<QuoteItemResponseDto> AddItemAsync(int quoteId, CreateQuoteItemDto dto)
    {
        var quote = await _quoteRepository.GetByIdAsync(quoteId);
        if (quote == null)
            throw new KeyNotFoundException($"Quote com ID {quoteId} não encontrado.");

        var item = new QuoteItem
        {
            QuoteId = quoteId,
            Description = dto.Description,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            TotalPrice = Math.Round(dto.Quantity * dto.UnitPrice, 2),
            CustomFields = dto.CustomFields ?? new Dictionary<string, string>()
        };

        // Define a ordem do novo item (última posição)
        var existingItems = await _quoteItemRepository.GetByQuoteIdAsync(quoteId);
        item.Order = existingItems.Count() + 1;

        var created = await _quoteItemRepository.CreateAsync(item);

        // Recalcular o total do orçamento
        quote.TotalPrice = (await _quoteItemRepository.GetByQuoteIdAsync(quoteId))
            .Sum(i => i.TotalPrice);
        await _quoteRepository.UpdateAsync(quote);

        return MapToResponseDto(created);
    }

    public async Task<QuoteItemResponseDto> UpdateItemAsync(int itemId, UpdateQuoteItemDto dto)
    {
        var existing = await _quoteItemRepository.GetByIdAsync(itemId);
        if (existing == null)
            throw new KeyNotFoundException($"QuoteItem com ID {itemId} não encontrado.");

        existing.Description = dto.Description;
        existing.Quantity = dto.Quantity;
        existing.UnitPrice = dto.UnitPrice;
        existing.TotalPrice = Math.Round(dto.Quantity * dto.UnitPrice, 2);
        existing.CustomFields = dto.CustomFields ?? new Dictionary<string, string>();

        var updated = await _quoteItemRepository.UpdateAsync(existing);

        // Recalcular total do orçamento
        var quote = await _quoteRepository.GetByIdAsync(existing.QuoteId);
        if (quote != null)
        {
            var items = await _quoteItemRepository.GetByQuoteIdAsync(quote.Id);
            quote.TotalPrice = items.Sum(i => i.TotalPrice);
            await _quoteRepository.UpdateAsync(quote);
        }

        return MapToResponseDto(updated);
    }

    public async Task<bool> DeleteItemAsync(int itemId)
    {
        var existing = await _quoteItemRepository.GetByIdAsync(itemId);
        if (existing == null)
            throw new KeyNotFoundException($"QuoteItem com ID {itemId} não encontrado.");

        bool deleted = await _quoteItemRepository.DeleteAsync(itemId);

        if (deleted)
        {
            var quote = await _quoteRepository.GetByIdAsync(existing.QuoteId);
            if (quote != null)
            {
                var items = await _quoteItemRepository.GetByQuoteIdAsync(quote.Id);
                quote.TotalPrice = items.Sum(i => i.TotalPrice);
                await _quoteRepository.UpdateAsync(quote);
            }
        }

        return deleted;
    }

    public async Task<bool> ReorderItemsAsync(int quoteId, List<int> itemIdsInOrder)
    {
        return await _quoteItemRepository.ReorderAsync(quoteId, itemIdsInOrder);
    }

    private static QuoteItemResponseDto MapToResponseDto(QuoteItem item)
    {
        return new QuoteItemResponseDto
        {
            Id = item.Id,
            QuoteId = item.QuoteId,
            Description = item.Description,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice,
            Order = item.Order,
            CustomFields = item.CustomFields
        };
    }
}