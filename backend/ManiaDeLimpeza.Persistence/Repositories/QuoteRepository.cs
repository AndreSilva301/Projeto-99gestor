using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Persistence.Repositories
{
    namespace ManiaDeLimpeza.Persistence.Repositories
    {
        public class QuoteRepository : BaseRepository<Quote>, IQuoteRepository, IScopedDependency
        {
            private readonly ApplicationDbContext _context;

            public QuoteRepository(ApplicationDbContext context)
                : base(context)
            {
                _context = context;
            }

            public async Task<Quote?> GetByIdAsync(int id, int companyId)
            {
                return await _context.Quotes
                    .Include(q => q.Customer)
                    .Include(q => q.User)
                    .Include(q => q.QuoteItems)
                    .FirstOrDefaultAsync(q => q.Id == id && q.CompanyId == companyId);
            }

            public async Task<Quote> CreateAsync(Quote quote, int companyId)
            {
                quote.CompanyId = companyId;

                await _context.Quotes.AddAsync(quote);
                await _context.SaveChangesAsync();

                return quote;
            }

            public async Task<Quote> UpdateAsync(Quote quote, int companyId)
            {
                quote.CompanyId = companyId;

                _context.Quotes.Update(quote);
                await _context.SaveChangesAsync();

                return quote;
            }

            public async Task<bool> DeleteAsync(int id, int companyId)
            {
                var quote = await _context.Quotes
                    .FirstOrDefaultAsync(q => q.Id == id && q.CompanyId == companyId);

                if (quote == null)
                    return false;

                _context.Quotes.Remove(quote);
                await _context.SaveChangesAsync();

                return true;
            }

            public async Task<bool> ExistsAsync(int id, int companyId)
            {
                return await _context.Quotes
                    .AnyAsync(q => q.Id == id && q.CompanyId == companyId);
            }

            public async Task<PagedResult<Quote>> GetPagedAsync(
                    string? searchTerm,
                    DateTime? createdAtStart,
                    DateTime? createdAtEnd,
                    string sortBy,
                    bool sortDescending,
                    int page,
                    int pageSize,
                    int companyId)
            {
                IQueryable<Quote> query = _context.Quotes
                    .Include(q => q.Customer)
                    .Include(q => q.User)
                    .Include(q => q.QuoteItems)
                    .Where(q => q.CompanyId == companyId);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(q =>
                        q.Customer.Name.Contains(searchTerm) ||
                        q.User.Name.Contains(searchTerm));
                }

                if (createdAtStart.HasValue)
                    query = query.Where(q => q.CreatedAt >= createdAtStart.Value);

                if (createdAtEnd.HasValue)
                    query = query.Where(q => q.CreatedAt <= createdAtEnd.Value);

                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortDescending
                        ? query.OrderByDescending(q => EF.Property<object>(q, sortBy))
                        : query.OrderBy(q => EF.Property<object>(q, sortBy));
                }

                var totalItems = await query.CountAsync();

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<Quote>
                {
                    TotalItems = totalItems,
                    Items = items,
                    Page = page,
                    PageSize = pageSize
                };
            }
        }
    }
}