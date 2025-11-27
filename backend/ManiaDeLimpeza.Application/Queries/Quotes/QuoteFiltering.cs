using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Queries.Quotes
{
    public static class QuoteFiltering
    {
        public static IQueryable<Quote> ApplyFilters(IQueryable<Quote> query, QuoteFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(q =>
                    EF.Functions.Collate(q.Customer.Name, "Latin1_General_CI_AI")
                        .Contains(filter.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(q => q.Customer.Phone.Mobile.Contains(filter.SearchTerm) ||
                                         q.Customer.Phone.Landline.Contains(filter.SearchTerm));
            }

            if (filter.CreatedAtStart.HasValue)
                query = query.Where(q => q.CreatedAt >= filter.CreatedAtStart.Value);

            if (filter.CreatedAtEnd.HasValue)
                query = query.Where(q => q.CreatedAt <= filter.CreatedAtEnd.Value);

            return query;
        }
    }
}
