using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Response;
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
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerRelationshipDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerRelationshipDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CustomerRelationshipDto>>> AddOrUpdateRelationships(
            int id,
            [FromBody] IEnumerable<CustomerRelationshipCreateOrUpdateDto> relationships)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponseHelper.ErrorResponse(errors, "Validation failed."));
            }

            var customer = await _customerService.GetByIdAsync(id, CurrentUserId);
            if (customer == null)
                return NotFound(ApiResponseHelper.ErrorResponse("Customer not found."));

            if (customer.CompanyId != CurrentCompanyId)
                return Forbid();

            var result = await _relationshipService.AddOrUpdateRelationshipsAsync(id, relationships, CurrentUserId);

            return CreatedAtAction(
                nameof(GetRelationships),
                new { id },
                ApiResponseHelper.SuccessResponse(result, "Relationships created or updated successfully."));
        }

        [HttpGet("{id}/relationships")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerRelationshipDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerRelationshipDto>>>> GetRelationships(int id)
        {
            var customer = await _customerService.GetByIdAsync(id, CurrentUserId);

            if (customer == null || customer.CompanyId != CurrentCompanyId)
                return BadRequest(ApiResponseHelper.ErrorResponse("Invalid customer or access denied."));

            var result = await _relationshipService.ListRelationshipsAsync(id);
            var ordered = result.OrderByDescending(r => r.DateTime);

            return Ok(ApiResponseHelper.SuccessResponse(ordered, "Relationships retrieved successfully."));
        }


        [HttpDelete("{id:int}/relationships")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRelationships(int id, [FromBody] IEnumerable<int> relationshipIds)
        {
            if (relationshipIds == null || !relationshipIds.Any())
                return BadRequest(ApiResponseHelper.ErrorResponse("No relationship IDs were provided."));

            var customer = await _customerService.GetByIdAsync(id, CurrentUserId);

            if (customer == null || customer.CompanyId != CurrentCompanyId)
                return BadRequest(ApiResponseHelper.ErrorResponse("Invalid customer or access denied."));

            var valid = await _relationshipService.ValidateOwnershipAsync(id, relationshipIds);
            if (!valid)
                return BadRequest(ApiResponseHelper.ErrorResponse("One or more relationships do not belong to this customer."));

            await _relationshipService.DeleteRelationshipsAsync(id, relationshipIds);
            return NoContent();
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
