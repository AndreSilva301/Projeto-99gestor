using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Customer>> GetById(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        {
            var clients = await _clientService.GetAllAsync();
            return Ok(clients);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Customer client)
        {
            await _clientService.AddAsync(client);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, Customer client)
        {
            if (id != client.Id)
                return BadRequest("ID mismatch");

            await _clientService.UpdateAsync(client);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            await _clientService.DeleteAsync(client);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Customer>>> Search(
            [FromQuery] string? term,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            var results = await _clientService.PaginatedSearchAsync(term, page, pageSize);
            return Ok(results);
        }
    }
}
