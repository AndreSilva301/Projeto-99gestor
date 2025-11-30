using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Quote?> DeleteAsync(int id, int companyId)
        {
            var quote = await _context.Quotes
                .FirstOrDefaultAsync(q => q.Id == id && q.CompanyId == companyId);

            if (quote == default)
                return null;

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            return quote;
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
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(q =>
                    q.Customer.Name.ToLower().Contains(lowerSearchTerm) ||
                    q.Customer.Email.ToLower().Contains(lowerSearchTerm) ||
                    (q.Customer.Phone.Mobile != null && q.Customer.Phone.Mobile.Contains(searchTerm)) ||
                    (q.Customer.Phone.Landline != null && q.Customer.Phone.Landline.Contains(searchTerm))
                );
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