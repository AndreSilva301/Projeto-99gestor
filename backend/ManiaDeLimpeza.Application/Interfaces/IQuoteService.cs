using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers
{
    public interface IQuoteService
    {
        Task<Quote?> GetByIdAsync(int id, int companyId);
        Task<PagedResult<Quote>> GetPagedAsync(QuoteFilterDto filter, int companyId);
        Task<QuoteResponseDto> UpdateAsync(int id, UpdateQuoteDto dto, int companyId);
        Task<QuoteResponseDto> CreateAsync(CreateQuoteDto dto, int userId, int companyId);
        Task<bool> DeleteAsync(int id, int companyId);
        Task<IEnumerable<QuoteResponseDto>> GetAllAsync(
        int companyId,
        int? customerId = null,
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 10);
    }
}