using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Queries.Quotes
{
    public static class QuoteSorting
    {
        public static IQueryable<Quote> ApplySorting(IQueryable<Quote> query, string? sortBy, bool descending)
        {
            return (sortBy?.ToLower(), descending) switch
            {
                ("clientname", true) => query.OrderByDescending(q => q.Customer.Name),
                ("clientname", false) => query.OrderBy(q => q.Customer.Name),
                ("createdat", true) => query.OrderByDescending(q => q.CreatedAt),
                _ => query.OrderBy(q => q.CreatedAt),
            };
        }
    }
}
