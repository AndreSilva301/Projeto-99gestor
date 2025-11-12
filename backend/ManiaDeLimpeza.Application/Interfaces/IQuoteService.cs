using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers
{
    public interface IQuoteService
    {
        Task<Quote?> GetByIdAsync(int id);
        Task<PagedResult<Quote>> GetPagedAsync(QuoteFilterDto filter);
        Task<QuoteResponseDto> UpdateAsync(int id, UpdateQuoteDto dto);
        Task<QuoteResponseDto> CreateAsync(CreateQuoteDto dto, int userId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<QuoteResponseDto>> GetAllAsync(
        int? customerId = null,
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10);
    }
}