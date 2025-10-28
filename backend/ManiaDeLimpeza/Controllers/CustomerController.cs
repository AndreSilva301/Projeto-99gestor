using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : AuthBaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id, CurrentUserId);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<CustomerListItemDto>>> Search(
            [FromQuery] string? term,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var results = await _customerService.SearchAsync(term, page, pageSize, CurrentCompanyId);
            return Ok(results);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CustomerCreateDto dto)
        {
            var created = await _customerService.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomerDto>> Update(int id, [FromBody] CustomerUpdateDto dto)
        {
            var updated = await _customerService.UpdateAsync(id, dto, CurrentUserId);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _customerService.SoftDeleteAsync(id, CurrentUserId);
            return NoContent();
        }
    }
}
