using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Dtos.Mappers;
using ManiaDeLimpeza.Application.Interfaces;
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

    [HttpGet("{companyId}/logo")]
    public async Task<IActionResult> GetLogo(int companyId)
    {
        var currentUser = CurrentUser;

        if (!currentUser.IsAdminOrSysAdmin(companyId))
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseHelper.ErrorResponse("Acesso não autorizado."));

        var logoBase64 = await _companyServices.GetLogoAsync(companyId);

        if (logoBase64 == null)
            return NotFound(ApiResponseHelper.ErrorResponse("A empresa não possui logo."));

        return Ok(new { LogoBase64 = logoBase64 });
    }

    [HttpPut("{companyId}/logo")]
    public async Task<IActionResult> UpdateLogo(int companyId, IFormFile file)
    {
        var currentUser = CurrentUser;

        if (!currentUser.IsAdminOrSysAdmin(companyId))
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseHelper.ErrorResponse("Acesso não autorizado."));

        if (file == null || file.Length == 0)
            return BadRequest(ApiResponseHelper.ErrorResponse("Nenhum arquivo enviado."));

        if (file.Length > 2 * 1024 * 1024)
            return BadRequest(ApiResponseHelper.ErrorResponse("A imagem deve ter no máximo 2MB."));

        var validTypes = new[] { "image/png", "image/jpeg", "image/jpg" };
        if (!validTypes.Contains(file.ContentType))
            return BadRequest(ApiResponseHelper.ErrorResponse("Formato inválido. Use PNG ou JPG."));

        var result = await _companyServices.UpdateLogoAsync(companyId, file, currentUser);

        return Ok(new { LogoBase64 = result });
    }
}
