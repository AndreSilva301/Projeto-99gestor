using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuotesController : AuthBaseController
    {
        private readonly IQuoteService _quoteService;

        public QuotesController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<QuoteResponseDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<QuoteResponseDto>>>> GetAll(
            int? customerId = null, int pageNumber = 1, int pageSize = 10)
        {
            var userId = CurrentUser.Id;
            var result = await _quoteService.GetAllAsync(customerId, userId, null, null, pageNumber, pageSize);
            return Ok(ApiResponseHelper.SuccessResponse(result, "Orçamentos listados com sucesso"));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<QuoteResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<QuoteResponseDto>>> GetById(int id)
        {
            var quote = await _quoteService.GetByIdAsync(id);
            if (quote == null)
                return NotFound(ApiResponseHelper.ErrorResponse("Orçamento não encontrado"));

            return Ok(ApiResponseHelper.SuccessResponse(quote, "Orçamento encontrado"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<QuoteResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<QuoteResponseDto>>> Create([FromBody] CreateQuoteDto dto)
        {
            if (dto == null)
                return BadRequest(ApiResponseHelper.ErrorResponse("Dados inválidos"));

            var userId = CurrentUser.Id;
            var created = await _quoteService.CreateAsync(dto, userId);

            return Created("/api/quotes", ApiResponseHelper.SuccessResponse(created, "Orçamento criado com sucesso"));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<QuoteResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<QuoteResponseDto>>> Update(int id, [FromBody] UpdateQuoteDto dto)
        {
            try
            {
                var updated = await _quoteService.UpdateAsync(id, dto);
                return Ok(ApiResponseHelper.SuccessResponse(updated, "Orçamento atualizado com sucesso"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponseHelper.ErrorResponse("Orçamento não encontrado"));
            }
            catch (BusinessException ex)
            {
                return BadRequest(ApiResponseHelper.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _quoteService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(ApiResponseHelper.ErrorResponse("Orçamento não encontrado"));

            await _quoteService.DeleteAsync(id);
            return NoContent();
        }
    }
}
