using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Application.Interfaces;
public interface IQuoteItemService
{
    Task<QuoteItemResponseDto> AddItemAsync(int quoteId, CreateQuoteItemDto dto);
    Task<QuoteItemResponseDto> UpdateItemAsync(int itemId, UpdateQuoteItemDto dto);
    Task<bool> DeleteItemAsync(int itemId);
    Task<bool> ReorderItemsAsync(int quoteId, List<int> itemIdsInOrder);
}
