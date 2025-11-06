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

        public async Task<IEnumerable<Quote>> GetAllAsync(
            int? customerId = null,
            int? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Quotes
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

            return await query
                .OrderByDescending(q => q.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public override async Task<Quote?> GetByIdAsync(int id)
        {
            return await _context.Quotes
                .Include(q => q.Customer)
                .Include(q => q.User)
                .Include(q => q.QuoteItems)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Quote> CreateAsync(Quote quote)
        {
            await _context.Quotes.AddAsync(quote);
            await _context.SaveChangesAsync();
            return quote;
        }

        public async Task<Quote> UpdateAsync(Quote quote)
        {
            _context.Quotes.Update(quote);
            await _context.SaveChangesAsync();
            return quote;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
                return false;

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountAsync(int? customerId = null, int? userId = null)
        {
            var query = _context.Quotes.AsQueryable();

            if (customerId.HasValue)
                query = query.Where(q => q.CustomerId == customerId.Value);

            if (userId.HasValue)
                query = query.Where(q => q.UserId == userId.Value);

            return await query.CountAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Quotes.AnyAsync(q => q.Id == id);
        }
    }
}