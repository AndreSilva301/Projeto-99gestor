using AutoMapper;
using ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Queries.Quotes;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManiaDeLimpeza.Persistence.Extensions;

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
            var quote = _mapper.Map<Quote>(quoteDto);
            quote.CreatedAt = DateTime.UtcNow;
            quote.UserId = user.Id;
            quote.TotalPrice = quote.QuoteItems.Sum(li => li.TotalValue);
            if (quote.CashDiscount.HasValue)
                quote.TotalPrice -= quote.CashDiscount.Value;

            var client = await _customerRepository.GetByIdAsync(quote.CostumerId);
            if (client == null)
            {
                throw new ArgumentException("Client not found");
            }

            await _quoteRepository.AddAsync(quote);
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

        /// <summary>
        /// Updates a quote using the provided DTO.
        /// The total price is recalculated using the LineItem.Total values.
        /// Note: Total may differ from UnitPrice * Quantity for manual overrides.
        /// </summary>
        public async Task<Quote> UpdateAsync(QuoteDto quoteDto)
        {

            var existing = await _quoteRepository.GetByIdAsync(quoteDto.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Quote with ID {quoteDto.Id} not found.");

            // Apply updates from DTO into the existing entity
            _mapper.Map(quoteDto, existing);

            // Recalculate total
            existing.TotalPrice = existing.QuoteItems.Sum(li => li.TotalValue);
            if (existing.CashDiscount.HasValue)
                existing.TotalPrice -= existing.CashDiscount.Value;

            await _quoteRepository.UpdateAsync(existing);
            return existing;
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
    }
}
