using AutoMapper;
using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Exceptions;
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
    [ProducesResponseType(typeof(List<UserLightDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var currentUser = GetCurrentUserOrThrow();

        if (!currentUser.IsAdminOrSysAdmin(currentUser.CompanyId))
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseHelper.ErrorResponse("Acesso não autorizado."));

        var users = await _userService.GetUsersByCompanyIdAsync(currentUser.CompanyId, includeInactive);
        var result = users.Select(u => new UserLightDto(u));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserLightDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
    {
        if (!dto.IsValid())
            return StatusCode(StatusCodes.Status400BadRequest,
                ApiResponseHelper.ErrorResponse(dto.Validate()));

        var currentUser = GetCurrentUserOrThrow();

        if (!currentUser.IsAdminOrSysAdmin(currentUser.CompanyId))            
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseHelper.ErrorResponse("Apenas administradores podem criar colaboradores."));
        
        if(dto.ProfileType != UserProfile.Employee)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseHelper.ErrorResponse("Apenas funcionários podem ser criados através desse endpoint."));

        }

        var created = await _userService.CreateEmployeeAsync(dto.Name, dto.Email, currentUser.CompanyId);

        if (created == null)
            throw new BusinessException("Erro ao criar colaborador.");

        var result = _mapper.Map<UserLightDto>(created);

        return Ok(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserLightDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var currentUser = GetCurrentUserOrThrow();

        if (currentUser.Id != id && !currentUser.IsAdminOrSysAdmin(currentUser.CompanyId))
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseHelper.ErrorResponse("Você não tem permissão para alterar os dados de outro usuário."));

        if (!dto.IsValid())
            return StatusCode(StatusCodes.Status400BadRequest,
                ApiResponseHelper.ErrorResponse(dto.Validate()));

        var user = await _userService.GetByIdAsync(id);
        if (user == null || user.CompanyId != currentUser.CompanyId)
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseHelper.ErrorResponse("Usuário não pertence à sua empresa."));

        user.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email;

        if (dto.Profile.HasValue && currentUser.IsAdminOrSysAdmin(currentUser.CompanyId))
            user.Profile = dto.Profile.Value;

        var updated = await _userService.UpdateUserAsync(user);
        var result = _mapper.Map<UserLightDto>(updated);

        return Ok(result);
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(UserLightDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(int id)
    {
        var currentUser = GetCurrentUserOrThrow();

        if (!currentUser.IsAdminOrSysAdmin(currentUser.CompanyId))
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseHelper.ErrorResponse("Apenas administradores podem desativar colaboradores."));

        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponseHelper.ErrorResponse("Usuário não encontrado."));

        if (!currentUser.IsSystemAdmin() && user.CompanyId != currentUser.CompanyId)
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponseHelper.ErrorResponse("Usuário não pertence à sua empresa."));

        var modifiedUser = await _userService.DeactivateUserAsync(id);
        var userListDto = new UserLightDto(modifiedUser);

        return Ok(userListDto);
    }

    [HttpPost("{id}/reactivate")]
    [ProducesResponseType(typeof(UserLightDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reactivate(int id)
    {
        var currentUser = GetCurrentUserOrThrow();

        if (!currentUser.IsAdminOrSysAdmin(currentUser.CompanyId))
            return StatusCode(StatusCodes.Status403Forbidden, 
                ApiResponseHelper.ErrorResponse("Apenas administradores podem reativar colaboradores."));

        var user = await _userService.GetByIdAsync(id);
       
        if (user == null)
            return NotFound(ApiResponseHelper.ErrorResponse("Usuário não encontrado."));

        if (!currentUser.IsSystemAdmin() && user.CompanyId != currentUser.CompanyId)
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponseHelper.ErrorResponse("Usuário não pertence à sua empresa."));

        var modifiedUser = await _userService.ReactivateUserAsync(id);
        var userListDto = new UserLightDto(modifiedUser);

        return Ok(userListDto);
    }
}