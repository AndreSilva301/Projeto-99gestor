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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var currentUser = CurrentUser;
        if (currentUser == null)
            return Unauthorized("Usuário não autenticado.");

        if (!IsSystemAdmin(currentUser))
            return Forbid("Acesso restrito a administradores do sistema.");

        var companies = await _companyServices.GetAllAsync(currentUser);
        var result = companies.Select(c => c.ToDto());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var currentUser = CurrentUser;
        if (currentUser == null)
            return Unauthorized("Usuário não autenticado.");

        if (!IsSystemAdmin(currentUser) && !IsCompanyAdmin(currentUser, id))
            return Forbid("Acesso não autorizado.");

        var company = await _companyServices.GetByIdAsync(id, currentUser);
        if (company == null)
            return NotFound($"Empresa com ID {id} não encontrada.");

        return Ok(company.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCompanyDto dto)
    {
        var currentUser = CurrentUser;
        if (currentUser == null)
            return Unauthorized("Usuário não autenticado.");

        if (!IsSystemAdmin(currentUser) && !IsCompanyAdmin(currentUser, id))
            return Forbid("Acesso não autorizado.");

        if (dto == null || dto.Address == null)
            return BadRequest("Dados inválidos. Endereço é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Address.Street) ||
            string.IsNullOrWhiteSpace(dto.Address.Number) ||
            string.IsNullOrWhiteSpace(dto.Address.Neighborhood) ||
            string.IsNullOrWhiteSpace(dto.Address.City) ||
            string.IsNullOrWhiteSpace(dto.Address.State) ||
            string.IsNullOrWhiteSpace(dto.Address.ZipCode))
        {
            return BadRequest("Campos obrigatórios do endereço não foram preenchidos.");
        }

        var company = await _companyServices.GetByIdAsync(id, currentUser);
        if (company == null)
            return NotFound($"Empresa com ID {id} não encontrada.");

        company.UpdateFromDto(dto);
        await _companyServices.UpdateCompanyAsync(company);

        return Ok(company.ToDto());
    }

    private static bool IsSystemAdmin(User user)
        => user.Profile == UserProfile.SystemAdmin;

    private static bool IsCompanyAdmin(User user, int companyId)
        => user.Profile == UserProfile.SystemAdmin && user.CompanyId == companyId;
}


