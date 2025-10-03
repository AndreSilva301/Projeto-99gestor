using ManiaDeLimpeza.Application.Dtos;
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
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository, IScopedDependency
    {
        protected readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Customer>> SearchAsync(string searchTerm)
        {
            return await BuildScoredQuery(searchTerm)
                .ToListAsync();
        }

        public async Task<PagedResult<Customer>> SearchPagedAsync(string searchTerm, int page, int pageSize)
        {
            var scoredQuery = BuildScoredQuery(searchTerm);

            var totalItems = await scoredQuery.CountAsync();

            var items = await scoredQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = items
            };
        }

        private IQueryable<Customer> BuildScoredQuery(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _context.Customers.AsQueryable();

            var words = searchTerm
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(w => w.ToLower())
                .ToArray();

            return _context.Customers
                .Select(client => new
                {
                    Client = client,
                    MatchScore = words.Count(word =>
                        (client.Name != null && client.Name.ToLower().Contains(word)) ||
                        (client.Phone.Mobile != null && client.Phone.Mobile.ToLower().Contains(word)) ||
                        (client.Phone.Landline != null && client.Phone.Landline.ToLower().Contains(word)))
                })
                .Where(x => x.MatchScore > 0)
                .OrderByDescending(x => x.MatchScore)
                .Select(x => x.Client)
                .AsQueryable();
        }
    }
}
