using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers
{
    public interface IQuoteService
    {
        Task<Quote> CreateAsync(QuoteDto quote, User user);
        Task<Quote?> GetByIdAsync(int id);
        Task<PagedResult<Quote>> GetPagedAsync(QuoteFilterDto filter);
        Task<Quote> UpdateAsync(QuoteDto quote);
    }
}