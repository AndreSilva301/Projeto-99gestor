using AutoMapper;
using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoworkersController : AuthBaseController
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public CoworkersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var currentUser = GetCurrentUserOrThrow();

        if (!currentUser.IsAdminOrSysAdmin())
            return Forbid("Acesso negado. Apenas administradores podem listar colaboradores.");

        var users = await _userService.GetUsersByCompanyIdAsync(currentUser.CompanyId, includeInactive);
        var result = users.Select(u => new UserListDto(u));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
    {
        if (!dto.IsValid())
            return BadRequest(dto.Validate());

        var currentUser = GetCurrentUserOrThrow();

        if (!currentUser.IsAdminOrSysAdmin())
            return Forbid("Apenas administradores podem criar colaboradores.");

        var created = await _userService.CreateEmployeeAsync(dto.Name, dto.Email, currentUser.CompanyId);

        if (created == null)
            throw new BusinessException("Erro ao criar colaborador.");

        var result = _mapper.Map<UserListDto>(created);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var currentUser = GetCurrentUserOrThrow();

        if (currentUser.Id != id && !currentUser.IsAdminOrSysAdmin())
            return Forbid("Você não tem permissão para alterar os dados de outro usuário.");

        if (!dto.IsValid())
            return BadRequest(new { Errors = dto.Validate() });

        var user = await _userService.GetByIdAsync(id);
        if (user == null || user.CompanyId != currentUser.CompanyId)
            return Forbid("Usuário não pertence à sua empresa.");

        user.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email;

        if (dto.Profile.HasValue && currentUser.IsAdminOrSysAdmin())
            user.Profile = dto.Profile.Value;

        var updated = await _userService.UpdateUserAsync(user);
        var result = _mapper.Map<UserListDto>(updated);

        return Ok(result);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var currentUser = GetCurrentUserOrThrow();

        if (!currentUser.IsAdminOrSysAdmin())
            return Forbid("Apenas administradores podem desativar colaboradores.");

        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound("Usuário não encontrado.");

        if (!currentUser.IsSystemAdmin() && user.CompanyId != currentUser.CompanyId)
            return Forbid("Usuário não pertence à sua empresa.");

        await _userService.DeactivateUserAsync(id);

        return Ok("Usuário desativado com sucesso.");
    }

    [HttpPost("{id}/reactivate")]
    public async Task<IActionResult> Reactivate(int id)
    {
        var currentUser = GetCurrentUserOrThrow();

        if (!currentUser.IsAdminOrSysAdmin())
            return Forbid("Apenas administradores podem reativar colaboradores.");

        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound("Usuário não encontrado.");

        if (!currentUser.IsSystemAdmin() && user.CompanyId != currentUser.CompanyId)
            return Forbid("Usuário não pertence à sua empresa.");

        await _userService.ReactivateUserAsync(id);

        return Ok("Usuário reativado com sucesso.");
    }
}