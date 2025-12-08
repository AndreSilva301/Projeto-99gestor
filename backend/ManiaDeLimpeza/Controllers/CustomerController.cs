using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Response;
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

        /// <summary>
        /// Adds or updates customer relationships.
        /// </summary>
        [HttpPost("{id:int}/relationships")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerRelationshipDto>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerRelationshipDto>>>> AddOrUpdateRelationships(int id, [FromBody] IEnumerable<CustomerRelationshipDto> relationships)
        {
            var currentUser = GetCurrentUserOrThrow();

            if (relationships == null || !relationships.Any())
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponseHelper.ErrorResponse("No relationships provided."));

            var invalidDtos = relationships.Where(r => !r.IsValid()).ToList();
            if (invalidDtos.Any())
            {
                var validationErrors = invalidDtos.SelectMany(r => r.Validate()).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponseHelper.ErrorResponse(validationErrors, "Validation failed."));
            }

            var customer = await _customerService.GetByIdAsync(id, currentUser.CompanyId);
            if (customer == null)
                return StatusCode(StatusCodes.Status404NotFound, ApiResponseHelper.ErrorResponse("Customer not found."));

            if (customer.CompanyId != currentUser.CompanyId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponseHelper.ErrorResponse("Access denied."));

            var result = await _customerService.AddOrUpdateRelationshipsAsync(id, relationships, currentUser.CompanyId);
            return CreatedAtAction(nameof(GetRelationships), new { id }, ApiResponseHelper.SuccessResponse(result, "Relationships created or updated successfully."));
        }

        /// <summary>
        /// Gets relationships for a customer.
        /// </summary>
        [HttpGet("{id:int}/relationships")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerRelationshipDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerRelationshipDto>>>> GetRelationships(int id)
        {
            var currentUser = GetCurrentUserOrThrow();
            var customer = await _customerService.GetByIdAsync(id, currentUser.CompanyId);
            if (customer == null)
                return StatusCode(StatusCodes.Status404NotFound, ApiResponseHelper.ErrorResponse("Customer not found."));
            if (customer.CompanyId != currentUser.CompanyId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponseHelper.ErrorResponse("Access denied."));

            var result = await _customerService.ListRelationshipsAsync(id, currentUser.CompanyId);
            var ordered = result.OrderByDescending(r => r.DateTime);
            return Ok(ApiResponseHelper.SuccessResponse(ordered, "Relationships retrieved successfully."));
        }

        /// <summary>
        /// Deletes specific relationships for a customer.
        /// </summary>
        [HttpDelete("{id:int}/relationships")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRelationships(int id, [FromBody] IEnumerable<int> relationshipIds)
        {
            var currentUser = GetCurrentUserOrThrow();

            if (relationshipIds == null || !relationshipIds.Any())
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponseHelper.ErrorResponse("No relationship IDs were provided."));

            var customer = await _customerService.GetByIdAsync(id, currentUser.CompanyId);
            if (customer == null)
                return StatusCode(StatusCodes.Status404NotFound, ApiResponseHelper.ErrorResponse("Customer not found."));
            if (customer.CompanyId != currentUser.CompanyId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponseHelper.ErrorResponse("Access denied."));

            await _customerService.DeleteRelationshipsAsync(id, relationshipIds, currentUser.CompanyId);
            return Ok(ApiResponseHelper.SuccessResponse("Relationships deleted successfully."));
        }

        /// <summary>
        /// Retrieves a customer by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetById(int id)
        {
            var currentUser = GetCurrentUserOrThrow();
            var customer = await _customerService.GetByIdAsync(id, currentUser.CompanyId);
            if (customer == null)
                return StatusCode(StatusCodes.Status404NotFound, ApiResponseHelper.ErrorResponse("Customer not found."));
            if (customer.CompanyId != currentUser.CompanyId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponseHelper.ErrorResponse("Access denied."));

            return Ok(ApiResponseHelper.SuccessResponse(customer, "Customer retrieved successfully."));
        }

        /// <summary>
        /// Searches customers (company scoped).
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CustomerListItemDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<CustomerListItemDto>>>> Search(
            [FromQuery] string? term,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string orderby = "Name",
            [FromQuery] string direction = "asc")
        {
            var currentUser = GetCurrentUserOrThrow();
            //Enforce OrderBy
            var possibleValuesOrderBy = new List<string>() { "UpdatedDate", "CreatedDate", "Phone", "Name" };
            if(possibleValuesOrderBy.Any(x => x.ToLower().Equals(orderby.ToLower())))
            {
                orderby = "Name";
            }
            //Enforce correct direction
            direction = direction.ToLower() == "desc" ? "desc" : "asc";

            //PageSize can't be bigger than 1000
            if(pageSize > 1000)
            {
                pageSize = 1000;
            }

            var results = await _customerService.SearchAsync(term, page, pageSize, currentUser.CompanyId, orderby, direction);
            return Ok(ApiResponseHelper.SuccessResponse(results, "Customers retrieved successfully."));
        }

        /// <summary>
        /// Creates a new customer.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Create([FromBody] CustomerCreateDto dto)
        {
            var currentUser = GetCurrentUserOrThrow();
            if (!dto.IsValid())
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponseHelper.ErrorResponse(dto.Validate(), "Validation failed."));

            var created = await _customerService.CreateAsync(dto, currentUser.CompanyId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponseHelper.SuccessResponse(created, "Customer created successfully."));
        }

        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> Update(int id, [FromBody] CustomerUpdateDto dto)
        {
            var currentUser = GetCurrentUserOrThrow();
            var existing = await _customerService.GetByIdAsync(id, currentUser.CompanyId);
            if (existing == null)
                return StatusCode(StatusCodes.Status404NotFound, ApiResponseHelper.ErrorResponse("Customer not found."));
            if (existing.CompanyId != currentUser.CompanyId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponseHelper.ErrorResponse("Access denied."));

            if (!dto.IsValid())
                return StatusCode(StatusCodes.Status400BadRequest, ApiResponseHelper.ErrorResponse(dto.Validate(), "Validation failed."));

            var updated = await _customerService.UpdateAsync(id, dto, currentUser.CompanyId);
            return Ok(ApiResponseHelper.SuccessResponse(updated, "Customer updated successfully."));
        }

        /// <summary>
        /// Soft deletes a customer.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var currentUser = GetCurrentUserOrThrow();
            var existing = await _customerService.GetByIdAsync(id, currentUser.CompanyId);
            if (existing == null)
                return StatusCode(StatusCodes.Status404NotFound, ApiResponseHelper.ErrorResponse("Customer not found."));
            if (existing.CompanyId != currentUser.CompanyId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponseHelper.ErrorResponse("Access denied."));

            await _customerService.SoftDeleteAsync(id, currentUser.CompanyId);
            return Ok(ApiResponseHelper.SuccessResponse("Customer deleted successfully."));
        }
    }
}
