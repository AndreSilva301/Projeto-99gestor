using AutoMapper;
using ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
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
        public async Task<Quote> CreateAsync(QuoteDto quoteDto, User user)
        {
            var client = await _customerRepository.GetByIdAsync(quoteDto.CustomerId);
            if (client == null)
                throw new ArgumentException("Client not found");

            var quote = _mapper.Map<Quote>(quoteDto);
            quote.CreatedAt = DateTime.UtcNow;
            quote.UserId = user.Id;
            quote.TotalPrice = quote.QuoteItems.Sum(li => li.TotalPrice);
            quote.QuoteItems ??= new List<QuoteItem>();

            if (quote.CashDiscount.HasValue)
                quote.TotalPrice -= quote.CashDiscount.Value;

            await _quoteRepository.CreateAsync(quote);
            return quote;
        }

        /// <summary>
        /// Retrieves a quote by ID if it is not archived.
        /// </summary>
        public async Task<Quote?> GetByIdAsync(int id)
        {
            var quote = await _quoteRepository.GetByIdAsync(id);

            return quote;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _quoteRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Quote with id {id} not found.");

            await _quoteRepository.DeleteAsync(existing);
            return true;
        }

        /// <summary>
        /// Updates a quote using the provided DTO.
        /// The total price is recalculated using the LineItem.Total values.
        /// Note: Total may differ from UnitPrice * Quantity for manual overrides.
        /// </summary>
        public async Task<QuoteResponseDto> UpdateAsync(int id, UpdateQuoteDto dto)
        {
            var existing = await _quoteRepository.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException($"Quote with id {id} not found.");

            existing.PaymentMethod = dto.PaymentMethod;
            existing.PaymentConditions = dto.PaymentConditions;
            existing.CashDiscount = dto.CashDiscount;
            existing.UpdatedAt = DateTime.UtcNow;

            var updatedItemIds = dto.Items.Where(i => i.Id > 0).Select(i => i.Id).ToList();
            var itemsToRemove = existing.QuoteItems.Where(i => !updatedItemIds.Contains(i.Id)).ToList();

            foreach (var item in itemsToRemove)
                existing.QuoteItems.Remove(item);

            int order = 1;
            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Id > 0)
                {
                    var existingItem = existing.QuoteItems.First(i => i.Id == itemDto.Id);
                    existingItem.Description = itemDto.Description;
                    existingItem.Quantity = itemDto.Quantity;
                    existingItem.UnitPrice = itemDto.UnitPrice;
                    existingItem.TotalPrice = Math.Round(itemDto.Quantity * itemDto.UnitPrice, 2);
                    existingItem.Order = order++;
                }
                else
                {
                    var newItem = new QuoteItem
                    {
                        Description = itemDto.Description,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        TotalPrice = Math.Round(itemDto.Quantity * itemDto.UnitPrice, 2),
                        Order = order++
                    };
                    existing.QuoteItems.Add(newItem);
                }
            }

            existing.TotalPrice = Math.Round(existing.QuoteItems.Sum(i => i.TotalPrice), 2);

            decimal finalPrice = existing.TotalPrice;
            if (!string.IsNullOrWhiteSpace(existing.PaymentConditions) &&
                (existing.PaymentConditions.Contains("à vista", StringComparison.OrdinalIgnoreCase) ||
                 existing.PaymentConditions.Contains("àvista", StringComparison.OrdinalIgnoreCase)))
            {
                if (existing.CashDiscount.HasValue)
                    finalPrice -= existing.CashDiscount.Value;
            }

            existing.TotalPrice = Math.Round(Math.Max(0m, finalPrice), 2);

            await _quoteRepository.UpdateAsync(existing);
            var result = _mapper.Map<QuoteResponseDto>(existing);
            return result;
        }

        /// <summary>
        /// Archives a quote by setting IsArchived to true.
        /// </summary>

        public async Task<PagedResult<Quote>> GetPagedAsync(QuoteFilterDto filter)
        {
            throw new NotImplementedException();
            //var query = _quoteRepository.Query()
            //    .Include(q => q.Customer)
            //    .Where(q => !q.IsArchived);

            //query = QuoteFiltering.ApplyFilters(query, filter);
            //query = QuoteSorting.ApplySorting(query, filter.SortBy, filter.SortDescending);

            //var (totalItems, items) = await query.PaginateAsync(filter.Page, filter.PageSize);

            //return new PagedResult<Quote>()
            //{
            //    TotalItems = totalItems,
            //    Items = items,
            //    PageSize = filter.PageSize,
            //    Page = filter.Page,
            //};
        }

        public async Task<QuoteResponseDto> CreateAsync(CreateQuoteDto dto, int userId)
        {
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer == null)
                throw new ArgumentException($"Customer with id {dto.CustomerId} not found.");

            var quote = _mapper.Map<Quote>(dto);
            quote.CreatedAt = DateTime.UtcNow;
            quote.UserId = userId;

            quote.QuoteItems ??= new List<QuoteItem>();

            int order = 1;
            foreach (var item in quote.QuoteItems)
            {
                item.TotalPrice = Math.Round(item.Quantity * item.UnitPrice, 2);
                item.Order = order++;
            }

            quote.TotalPrice = Math.Round(quote.QuoteItems.Sum(i => i.TotalPrice), 2);

            decimal finalPrice = quote.TotalPrice;
            if (!string.IsNullOrWhiteSpace(quote.PaymentConditions) &&
                (quote.PaymentConditions.Contains("à vista", StringComparison.OrdinalIgnoreCase) ||
                 quote.PaymentConditions.Contains("àvista", StringComparison.OrdinalIgnoreCase)))
            {
                if (quote.CashDiscount.HasValue)
                    finalPrice -= quote.CashDiscount.Value;
            }

            quote.TotalPrice = Math.Round(Math.Max(0m, finalPrice), 2);

            await _quoteRepository.AddAsync(quote);

            Quote? persisted = null;
            if (_quoteRepository is IQuoteRepository repoWithQuery)
            {
                var queryable = repoWithQuery.Query();
                if (queryable != null)
                {
                    persisted = await queryable
                        .Include(q => q.Customer)
                        .Include(q => q.User)
                        .Include(q => q.QuoteItems)
                        .FirstOrDefaultAsync(q => q.Id == quote.Id);
                }
            }

            var response = _mapper.Map<QuoteResponseDto>(persisted ?? quote);
            return response;
        }

        public async Task<IEnumerable<QuoteResponseDto>> GetAllAsync(int? customerId = null, int? userId = null, DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _quoteRepository.Query()
                .Include(q => q.Customer)
                .Include(q => q.User)
                .Include(q => q.QuoteItems)
                .AsQueryable();

            if (customerId.HasValue)
                query = query.Where(q => q.CustomerId == customerId.Value);

            if (userId.HasValue)
                query = query.Where(q => q.UserId == userId.Value);

            if (startDate.HasValue)
                query = query.Where(q => q.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(q => q.CreatedAt <= endDate.Value);

            query = query.OrderByDescending(q => q.CreatedAt);

            var skip = (pageNumber - 1) * pageSize;
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            foreach (var q in items)
            {
                if (q.QuoteItems != null && q.QuoteItems.Any())
                {
                    foreach (var item in q.QuoteItems)
                    {
                        item.TotalPrice = Math.Round(item.Quantity * item.UnitPrice, 2);
                    }
                }

                q.TotalPrice = Math.Round(q.QuoteItems?.Sum(i => i.TotalPrice) ?? 0m, 2);

                decimal final = q.TotalPrice;
                if (!string.IsNullOrWhiteSpace(q.PaymentConditions) &&
                    (q.PaymentConditions.IndexOf("à vista", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     q.PaymentConditions.IndexOf("àvista", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    if (q.CashDiscount.HasValue)
                        final -= q.CashDiscount.Value;
                }
                q.TotalPrice = Math.Round(Math.Max(0m, final), 2);
            }

            var dtos = _mapper.Map<IEnumerable<QuoteResponseDto>>(items);
            return dtos;
        }

    }
}
