using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Dtos.Mappers;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers.Base;
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
    public async Task<IActionResult> GetById(int id)
    {
        var currentUser = CurrentUser;

        if (!currentUser.IsAdminOrSysAdmin(id))
            return Forbid("Acesso não autorizado.");

        var company = await _companyServices.GetByIdAsync(id, currentUser);
        return Ok(company.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCompanyDto dto)
    {
        var currentUser = CurrentUser;

        if (!currentUser.IsAdminOrSysAdmin(id))
            return Forbid("Acesso não autorizado.");

        if (!dto.IsValid())
            return BadRequest(new
            {
                Message = "Erro de validação.",
                Errors = dto.Validate().ToArray()
            });

        var updatedCompany = await _companyServices.UpdateCompanyAsync(id, dto, currentUser);

        if (updatedCompany == null)
            return NotFound($"Empresa com ID {id} não encontrada.");

        return Ok(updatedCompany.ToDto());
    }
}
