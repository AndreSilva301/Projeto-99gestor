using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Persistence.Repositories
{
    public class QuoteRepository : BaseRepository<Quote>, IQuoteRepository, IScopedDependency
    {
        protected readonly ApplicationDbContext _context;

        public QuoteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<Quote?> GetByIdAsync(int id)
        {
            var result = await _context.Quotes
                            .Include(x => x.LineItems)
                            .Include(x => x.Client)
                            .FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public IQueryable<Quote> Query()
        {
            return _context.Quotes;
        }
    }
}
