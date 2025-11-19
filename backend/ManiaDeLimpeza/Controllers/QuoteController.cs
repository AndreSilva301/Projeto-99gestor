using AutoMapper;
using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuoteController : AuthBaseController
    {
        private readonly QuoteService _quoteService;
        private readonly IMapper _mapper;

        public QuoteController(QuoteService quoteService, IMapper mapper)
        {
            _quoteService = quoteService;
            _mapper = mapper;
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] QuoteDto dto)
        {
            if (CurrentUser == null)
                return Unauthorized(new ApiResponse<string>("Unable to resolve current user"));

            var created = await _quoteService.CreateAsync(dto, CurrentUser!, CurrentUser!.CompanyId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                new ApiResponse<QuoteDto>(_mapper.Map<QuoteDto>(created))
            );
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuoteDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new ApiResponse<string>("ID mismatch"));

            var updated = await _quoteService.UpdateAsync(id, dto, CurrentUser.CompanyId);

            return Ok(new ApiResponse<QuoteDto>(updated));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var quote = await _quoteService.GetByIdAsync(id, CurrentUser.CompanyId);

            if (quote == null)
                return NotFound(new ApiResponse<string>("Quote not found"));

            return Ok(new ApiResponse<QuoteDto>(_mapper.Map<QuoteDto>(quote)));
        }

        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<QuoteDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<QuoteDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromBody] QuoteFilterDto filter)
        {
            var result = await _quoteService.GetPagedAsync(filter, CurrentUser.CompanyId);

            var quoteDtos = result.Items.Select(q => new QuoteDto
            {
                Id = q.Id,
                CustomerId = q.CustomerId,
                TotalPrice = q.TotalPrice,
                PaymentMethod = q.PaymentMethod,
                PaymentConditions = q.PaymentConditions,
                CashDiscount = q.CashDiscount,

                Items = q.QuoteItems.Select(i => new QuoteItemDto
                {
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    CustomFields = i.CustomFields
                }).ToList()
            });

            var dto = new PagedResultDto<QuoteDto>(
                totalCount: result.TotalCount,
                items: quoteDtos
            );

            return Ok(new ApiResponse<PagedResultDto<QuoteDto>>(dto));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _quoteService.DeleteAsync(id, CurrentUser.CompanyId);

            if (!deleted)
                return BadRequest(new ApiResponse<string>("Unable to delete quote"));

            return NoContent();
        }
    }
}