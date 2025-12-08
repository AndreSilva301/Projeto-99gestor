using AutoMapper;
using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuoteController : AuthBaseController
    {
        private readonly IQuoteService _quoteService;
        private readonly IMapper _mapper;

        public QuoteController(IQuoteService quoteService, IMapper mapper)
        {
            _quoteService = quoteService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateQuoteDto dto)
        {
            var errors = dto.Validate();
            if (errors.Any())
                return BadRequest(new ApiResponse<List<string>>(errors));


            var created = await _quoteService.CreateAsync(dto, CurrentUser.Id, CurrentUser.CompanyId);

            return Created(
                string.Empty,
                new ApiResponse<QuoteResponseDto>(created)
             );
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UpdateQuoteDto dto)
        {
            var updated = await _quoteService.UpdateAsync(dto, CurrentUser.CompanyId);

            return Ok(new ApiResponse<QuoteResponseDto>(updated));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<QuoteDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var quote = await _quoteService.GetByIdAsync(id, CurrentUser.CompanyId);

            if (quote == null)
                return NotFound(new ApiResponse<string>("Quote not found"));

            var quoteDto = _mapper.Map<QuoteResponseDto>(quote);

            return Ok(
                new ApiResponse<QuoteResponseDto>(quoteDto)
            );
        }

        [HttpPost("search")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<QuoteDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<QuoteDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromBody] QuoteFilterDto filter)
        {
            var result = await _quoteService.GetPagedAsync(filter, CurrentUser.CompanyId);

            return Ok(new ApiResponse<PagedResult<QuoteResponseDto>>(result));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _quoteService.DeleteAsync(id, CurrentUser.CompanyId);

            if (!success)
                return BadRequest(new ApiResponse<string>("Could not delete quote"));

            return NoContent();
        }
    } 
}