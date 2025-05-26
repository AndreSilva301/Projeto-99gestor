using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Persistence.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<(int TotalItems, List<T> Items)> PaginateAsync<T>(
            this IQueryable<T> query, int page, int pageSize)
        {
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ()
            {
                TotalItems = totalItems,
                Items = items
            };
        }
    }
}
