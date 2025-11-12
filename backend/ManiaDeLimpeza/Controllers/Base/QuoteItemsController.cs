using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuoteItemsController : AuthBaseController
    {
        private readonly IQuoteItemService _quoteItemService;
        private readonly IQuoteService _quoteService;

        public QuoteItemsController(IQuoteItemService quoteItemService, IQuoteService quoteService)
        {
            _quoteItemService = quoteItemService;
            _quoteService = quoteService;
        }

        [HttpPost("quotes/{quoteId:int}/items")]
        [ProducesResponseType(typeof(ApiResponse<QuoteItemResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<QuoteItemResponseDto>>> AddItem(
            int quoteId,
            [FromBody] CreateQuoteItemDto dto)
        {
            var quote = await _quoteService.GetByIdAsync(quoteId);
            if (quote == null)
                return NotFound(ApiResponseHelper.ErrorResponse("Orçamento não encontrado."));

            if (quote.UserId != CurrentUser.Id)
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponseHelper.ErrorResponse("Você não tem permissão para modificar este orçamento."));

            var created = await _quoteItemService.AddItemAsync(quoteId, dto);
            return Created($"/api/quotes/{quoteId}/items/{created.Id}",
                ApiResponseHelper.SuccessResponse(created, "Item adicionado com sucesso"));
        }

        [HttpPut("quote-items/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<QuoteItemResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<QuoteItemResponseDto>>> UpdateItem(
            int id,
            [FromBody] UpdateQuoteItemDto dto)
        {
            var item = await _quoteItemService.UpdateItemAsync(id, dto);
            var quote = await _quoteService.GetByIdAsync(item.QuoteId);

            if (quote == null)
                return NotFound(ApiResponseHelper.ErrorResponse("Orçamento não encontrado."));

            if (quote.UserId != CurrentUser.Id)
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponseHelper.ErrorResponse("Você não tem permissão para modificar este orçamento."));

            return Ok(ApiResponseHelper.SuccessResponse(item, "Item atualizado com sucesso"));
        }

        [HttpDelete("quote-items/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _quoteItemService.UpdateItemAsync(id, new UpdateQuoteItemDto());
            if (item == null)
                return NotFound(ApiResponseHelper.ErrorResponse("Item não encontrado."));

            var quote = await _quoteService.GetByIdAsync(item.QuoteId);
            if (quote == null)
                return NotFound(ApiResponseHelper.ErrorResponse("Orçamento não encontrado."));

            if (quote.UserId != CurrentUser.Id)
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponseHelper.ErrorResponse("Você não tem permissão para modificar este orçamento."));

            var allItems = await _quoteItemService.ReorderItemsAsync(quote.Id, new List<int>());
            if (!allItems)
                return BadRequest(ApiResponseHelper.ErrorResponse("Não é possível remover o último item do orçamento."));

            await _quoteItemService.DeleteItemAsync(id);
            return NoContent();
        }

        [HttpPost("quotes/{quoteId:int}/items/reorder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReorderItems(
            int quoteId,
            [FromBody] ReorderQuoteItemsDto dto)
        {
            var quote = await _quoteService.GetByIdAsync(quoteId);
            if (quote == null)
                return NotFound(ApiResponseHelper.ErrorResponse("Orçamento não encontrado."));

            if (quote.UserId != CurrentUser.Id)
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponseHelper.ErrorResponse("Você não tem permissão para modificar este orçamento."));

            if (dto.ItemIds == null || !dto.ItemIds.Any())
                return BadRequest(ApiResponseHelper.ErrorResponse("A lista de itens é obrigatória."));

            await _quoteItemService.ReorderItemsAsync(quoteId, dto.ItemIds);
            return NoContent();
        }
    }

    public class ReorderQuoteItemsDto
    {
        public List<int> ItemIds { get; set; } = new();
    }
}
