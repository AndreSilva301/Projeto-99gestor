using AutoMapper;
using ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Application.Services
{
    public class QuoteService : IQuoteService, IScopedDependency
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public QuoteService(
            IQuoteRepository quoteRepository,
            ICustomerRepository customerRepository,
            IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new quote from the provided DTO.
        /// The total price is calculated from the sum of each line item's Total property.
        /// Note: Total may differ from UnitPrice * Quantity for manual overrides.
        /// </summary>
        public async Task<QuoteResponseDto> CreateAsync(CreateQuoteDto dto, int userId, int companyId)
        {
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer == null || customer.CompanyId != companyId)
                throw new BusinessException("Cliente não pertence à sua empresa.");

            if (dto.Items == null || dto.Items.Count == 0)
                throw new BusinessException("O orçamento deve conter pelo menos um item.");

            //foreach (var item in dto.Items)
            //{
            //    if (string.IsNullOrWhiteSpace(item.Description))
            //        throw new BusinessException("A descrição do item não pode estar vazia.");

            //  if (item.Quantity <= 0)
            //        throw new BusinessException("A quantidade deve ser maior que zero.");

            //    if (item.UnitPrice <= 0)
            //        throw new BusinessException("O preço unitário deve ser maior que zero.");
            //}

            var quote = _mapper.Map<Quote>(dto);
            quote.CreatedAt = DateTime.UtcNow;
            quote.UserId = userId;
            quote.CompanyId = companyId;

            quote.RecalculateTotals();
            quote.EnsureQuoteItemsOrder();

            var addedQuote = await _quoteRepository.AddAsync(quote);

            return _mapper.Map<QuoteResponseDto>(addedQuote);
        }

        /// <summary>
        /// Retrieves a quote by ID if it is not archived.
        /// </summary>
        public async Task<Quote?> GetByIdAsync(int id, int companyId)
        {
            var quote = await _quoteRepository.GetByIdAsync(id);

            // Ensure the quote belongs to the specified company
            if (!quote?.IsForCompany(companyId) ?? false)
            {
                return null;
            }

            return quote;
        }

        public async Task<bool> DeleteAsync(int id, int companyId)
        {
            var quote = await _quoteRepository.DeleteAsync(id, companyId);

            return quote != null;
        }

        /// <summary>
        /// Updates a quote using the provided DTO.
        /// The total price is recalculated using the LineItem.Total values.
        /// Note: Total may differ from UnitPrice * Quantity for manual overrides.
        /// </summary>
        public async Task<QuoteResponseDto> UpdateAsync(UpdateQuoteDto dto, int companyId)
        {
            var id = dto.Id;

            var existing = await _quoteRepository.GetByIdAsync(id);

            if (dto.Items == null || dto.Items.Count == 0)
                throw new BusinessException("The quote must contain at least one item.");

            if (existing == null)
                throw new KeyNotFoundException($"Quote with id {id} not found.");

            if (existing.CompanyId != companyId)
                throw new KeyNotFoundException("Quote does not belong to the company.");

            existing.PaymentMethod = dto.PaymentMethod;
            existing.PaymentConditions = dto.PaymentConditions;
            existing.CashDiscount = dto.CashDiscount;
            existing.UpdatedAt = DateTime.UtcNow;

            var updatedItemIds = dto.Items
                .Where(x => x.Id > 0)
                .Select(x => x.Id)
                .ToList();

            var itemsToRemove = existing.QuoteItems
                .Where(x => !updatedItemIds.Contains(x.Id))
                .ToList();

            foreach (var toRemove in itemsToRemove)
                existing.QuoteItems.Remove(toRemove);

            dto.EnsureQuoteItemsOrder();

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Id > 0)
                {
                    var existingItem = existing.QuoteItems.First(i => i.Id == itemDto.Id);
                    existingItem.Description = itemDto.Description;
                    existingItem.Quantity = itemDto.Quantity;
                    existingItem.UnitPrice = itemDto.UnitPrice;
                    existingItem.Order = itemDto.Order;

                    if (itemDto.TotalPrice.HasValue)
                        existingItem.TotalPrice = existingItem.GetCalculatedTotalPrice();
                }
                else
                {
                    var newItem = new QuoteItem
                    {
                        Description = itemDto.Description,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        Order = itemDto.Order,
                        TotalPrice = itemDto.TotalPrice ?? 0
                    };

                    newItem.TotalPrice = newItem.GetCalculatedTotalPrice();
                    existing.QuoteItems.Add(newItem);
                }
            }

            existing.RecalculateTotals();

            var updated = await _quoteRepository.UpdateAsync(existing);

            return _mapper.Map<QuoteResponseDto>(updated);
        }

        /// <summary>
        /// Returns paginated and filtered list of quotes.
        /// </summary>
        public async Task<PagedResult<QuoteResponseDto>> GetPagedAsync(QuoteFilterDto filter, int companyId)
        {
            var result = await _quoteRepository.GetPagedAsync(
                searchTerm: filter.SearchTerm,
                createdAtStart: filter.CreatedAtStart,
                createdAtEnd: filter.CreatedAtEnd,
                sortBy: filter.SortBy,
                sortDescending: filter.SortDescending,
                page: filter.Page,
                pageSize: filter.PageSize,
                companyId: companyId
            );

            return new PagedResult<QuoteResponseDto>(
                result.TotalItems,
                result.Items.Select(q => _mapper.Map<QuoteResponseDto>(q)),
                result.Page,
                result.PageSize
            );
        }
    }
}