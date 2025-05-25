using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Controllers.ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuoteController : AuthControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuoteController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] QuoteDto quoteDto)
        {
            var user = GetCurrentUser();
            if (user == null)
                return Unauthorized("Unable to resolve current user");

            var created = await _quoteService.CreateAsync(quoteDto, user);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] QuoteDto quoteDto)
        {
            if (id != quoteDto.Id) return BadRequest("ID mismatch");
            await _quoteService.UpdateAsync(quoteDto);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quote = await _quoteService.GetByIdAsync(id);
            if (quote == null) return NotFound();
            return Ok(quote);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Archive(int id)
        {
            var success = await _quoteService.ArchiveAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}

