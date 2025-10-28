using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : AuthBaseController
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerRelationshipService _relationshipService;

        public CustomerController(
            ICustomerService customerService,
            ICustomerRelationshipService relationshipService)
        {
            _customerService = customerService;
            _relationshipService = relationshipService;
        }

        [HttpPost("{id:int}/relationships")]
        public async Task<ActionResult<IEnumerable<CustomerRelationshipDto>>> AddOrUpdateRelationships(
            int id,
            [FromBody] IEnumerable<CustomerRelationshipCreateOrUpdateDto> relationships)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var customer = await _customerService.GetByIdAsync(id, CurrentUserId);
                if (customer == null)
                    return NotFound();

                if (customer.CompanyId != CurrentCompanyId)
                    return Forbid();

                var result = await _relationshipService.AddOrUpdateRelationshipsAsync(id, relationships, CurrentUserId);
                return CreatedAtAction(nameof(GetRelationships), new { id }, result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}/relationships")]
        public async Task<ActionResult<IEnumerable<CustomerRelationshipDto>>> GetRelationships(int id)
        {
            var customer = await _customerService.GetByIdAsync(id, CurrentUserId);
            if (customer == null)
                return NotFound();

            if (customer.CompanyId != CurrentCompanyId)
                return Forbid();

            var result = await _relationshipService.ListRelationshipsAsync(id);
            return Ok(result.OrderByDescending(r => r.DateTime));
        }

        [HttpDelete("{id:int}/relationships")]
        public async Task<ActionResult> DeleteRelationships(int id, [FromBody] IEnumerable<int> relationshipIds)
        {
            if (relationshipIds == null || !relationshipIds.Any())
                return BadRequest("Nenhum relacionamento foi informado.");

            try
            {
                var customer = await _customerService.GetByIdAsync(id, CurrentUserId);
                if (customer == null)
                    return NotFound();

                if (customer.CompanyId != CurrentCompanyId)
                    return Forbid();

                var valid = await _relationshipService.ValidateOwnershipAsync(id, relationshipIds);
                if (!valid)
                    return BadRequest("Um ou mais relacionamentos não pertencem ao cliente informado.");

                await _relationshipService.DeleteRelationshipsAsync(id, relationshipIds);
                return NoContent();
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
