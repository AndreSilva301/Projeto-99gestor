using AutoMapper;
using ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Infrastructure.Exceptions;
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

            var quote = _mapper.Map<Quote>(dto);
            quote.CreatedAt = DateTime.UtcNow;
            quote.UserId = userId;
            quote.CompanyId = companyId;

            quote.RecalculateTotals();

            await _quoteRepository.AddAsync(quote);

            var persisted = await _quoteRepository.Query()
                .Include(q => q.Customer)
                .Include(q => q.User)
                .Include(q => q.QuoteItems)
                .FirstOrDefaultAsync(q => q.Id == quote.Id);

            return _mapper.Map<QuoteResponseDto>(persisted ?? quote);
        }

        /// <summary>
        /// Retrieves a quote by ID if it is not archived.
        /// </summary>
        public async Task<Quote?> GetByIdAsync(int id, int companyId)
        {
            var quote = await _quoteRepository.GetByIdAsync(id, companyId);

            if (quote == null)
                throw new BusinessException($"Quote with id {id} not found.");

            if (quote.Customer.CompanyId != companyId)
                throw new BusinessException("Quote does not belong to the company.");

            return quote;
        }

        public async Task<bool> DeleteAsync(int id, int companyId)
        {
            var existing = await _quoteRepository.GetByIdAsync(id, companyId);

            if (existing == null)
                throw new BusinessException($"Quote with id {id} not found.");

            if (existing.Customer.CompanyId != companyId)
                throw new BusinessException("Quote does not belong to the company.");

            await _quoteRepository.DeleteAsync(id, companyId);

            return true;
        }

        /// <summary>
        /// Updates a quote using the provided DTO.
        /// The total price is recalculated using the LineItem.Total values.
        /// Note: Total may differ from UnitPrice * Quantity for manual overrides.
        /// </summary>
        public async Task<QuoteResponseDto> UpdateAsync(int id, UpdateQuoteDto dto, int companyId)
        {
            var existing = await _quoteRepository.GetByIdAsync(id, companyId);

            if (existing == null)
                throw new BusinessException($"Quote with id {id} not found.");

            if (existing.Customer.CompanyId != companyId)
                throw new BusinessException("Quote does not belong to the company.");

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

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Id > 0)
                {
                    var existingItem = existing.QuoteItems.First(i => i.Id == itemDto.Id);
                    existingItem.Description = itemDto.Description;
                    existingItem.Quantity = itemDto.Quantity;
                    existingItem.UnitPrice = itemDto.UnitPrice;

                    if (itemDto.TotalPrice.HasValue)
                        existingItem.TotalPrice = itemDto.TotalPrice.Value;

                    existingItem.AplicarRegrasDePreco();
                }
                else
                {
                    var newItem = new QuoteItem
                    {
                        Description = itemDto.Description,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        TotalPrice = itemDto.TotalPrice ?? 0
                    };

                    newItem.AplicarRegrasDePreco();
                    existing.QuoteItems.Add(newItem);
                }
            }

            existing.RecalculateTotals();

            await _quoteRepository.UpdateAsync(existing, companyId);

            return _mapper.Map<QuoteResponseDto>(existing);
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

        /// <summary>
        /// Returns all quotes for a company without filtering.
        /// </summary>
        public async Task<IEnumerable<QuoteResponseDto>> GetAllAsync(int companyId)
        {
            var items = await _quoteRepository.Query()
                .Include(q => q.Customer)
                .Include(q => q.User)
                .Include(q => q.QuoteItems)
                .Where(q => q.CompanyId == companyId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuoteResponseDto>>(items);
        }
    }
}