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

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("{id:int}/relationships")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerRelationshipDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerRelationshipDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CustomerRelationshipDto>>> AddOrUpdateRelationships(int id, [FromBody] IEnumerable<CustomerRelationshipDto> relationships)
        {
            var invalidDtos = relationships.Where(r => !r.IsValid()).ToList();
            if (invalidDtos.Any())
            {
                var validationErrors = invalidDtos
                    .SelectMany(r => r.Validate())
                    .ToList();

                return BadRequest(ApiResponseHelper.ErrorResponse(validationErrors, "Validation failed."));
            }

            var customer = await _customerService.GetByIdAsync(id, CurrentCompanyId);
            if (customer == null)
                return NotFound(ApiResponseHelper.ErrorResponse("Customer not found."));

            if (customer.CompanyId != CurrentCompanyId)
                return Forbid();

            var result = await _customerService.AddOrUpdateRelationshipsAsync(id, relationships, CurrentCompanyId);

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

            var result = await _customerService.ListRelationshipsAsync(id, CurrentCompanyId);
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

            await _customerService.DeleteRelationshipsAsync(id, relationshipIds, CurrentCompanyId);
            return Ok(ApiResponseHelper.SuccessResponse("Relationships deleted successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id, CurrentUserId);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CustomerListItemDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<CustomerListItemDto>>> Search(
            [FromQuery] string? term,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var results = await _customerService.SearchAsync(term, page, pageSize, CurrentCompanyId);
            return Ok(ApiResponseHelper.SuccessResponse(results, "Customers retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Create([FromBody] CustomerCreateDto dto)
        {
            var created = await _customerService.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Update(int id, [FromBody] CustomerUpdateDto dto)
        {
            var updated = await _customerService.UpdateAsync(id, dto, CurrentUserId);
            return Ok(ApiResponseHelper.SuccessResponse(updated, "Customer updated successfully."));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            await _customerService.SoftDeleteAsync(id, CurrentUserId);
            return Ok(ApiResponseHelper.SuccessResponse<object>(null, "Customer deleted successfully."));
        }
    }
}
