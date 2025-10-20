using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuoteController : AuthBaseController
    {
        // Remover IQuoteService temporariamente até implementarmos
        // private readonly IQuoteService _quoteService;

        public QuoteController()
        {
            // _quoteService = quoteService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] QuoteDto quoteDto)
        {
            // Usar a propriedade CurrentUser do AuthBaseController
            if (CurrentUser == null)
                return Unauthorized("Unable to resolve current user");

            // TODO: Implementar quando IQuoteService estiver disponível
            // var created = await _quoteService.CreateAsync(quoteDto, CurrentUser);
            // return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            
            return Ok($"Quote would be created for user: {CurrentUser.Name}");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] QuoteDto quoteDto)
        {
            if (id != quoteDto.Id) return BadRequest("ID mismatch");
            // await _quoteService.UpdateAsync(quoteDto);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // var quote = await _quoteService.GetByIdAsync(id);
            // if (quote == null) return NotFound();
            // return Ok(quote);
            return Ok($"Quote {id} for user: {CurrentUser?.Name}");
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] QuoteFilterDto filter)
        {
            // var quotes = await _quoteService.GetPagedAsync(filter);
            // return Ok(quotes);
            return Ok($"Search quotes for user: {CurrentUser?.Name}");
        }
    }
}

