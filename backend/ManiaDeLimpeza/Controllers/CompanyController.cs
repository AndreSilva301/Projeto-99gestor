using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Dtos.Mappers;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CompanyController : AuthBaseController
{
    private readonly ICompanyServices _companyServices;

    public CompanyController(ICompanyServices companyServices)
    {
        _companyServices = companyServices;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(int id)
    {
        var currentUser = CurrentUser;

        if (!currentUser.IsAdminOrSysAdmin(id))
            return StatusCode(StatusCodes.Status403Forbidden, 
                ApiResponseHelper.ErrorResponse("Acesso não autorizado."));

        var company = await _companyServices.GetByIdAsync(id, currentUser);
        return Ok(company.ToDto());
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCompanyDto dto)
    {
        var currentUser = CurrentUser;

        if (!currentUser.IsAdminOrSysAdmin(id))
            return StatusCode(StatusCodes.Status403Forbidden, 
                ApiResponseHelper.ErrorResponse("Acesso não autorizado."));

        if (!dto.IsValid())
        {
            var errors = dto.Validate().ToList();
            return StatusCode(StatusCodes.Status400BadRequest,
                ApiResponseHelper.ErrorResponse(errors, "Erro de validação."));
        }

        var updatedCompany = await _companyServices.UpdateCompanyAsync(id, dto, currentUser);

        if (updatedCompany == null)
            return NotFound(ApiResponseHelper.ErrorResponse($"Empresa com ID {id} não encontrada."));

        return Ok(updatedCompany.ToDto());
    }
}
