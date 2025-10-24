using AutoMapper;
using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
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

            var result = users.Select(u => new UserListDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Profile = u.Profile
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            var currentUser = GetCurrentUserOrThrow();

            if (!currentUser.IsAdminOrSysAdmin())
                return Forbid("Apenas administradores podem criar colaboradores.");

            if (dto.ProfileType == UserProfile.SystemAdmin)
                return BadRequest("Não é permitido criar usuários com perfil SystemAdmin.");

            var created = await _userService.CreateEmployeeAsync(dto.Name, dto.Email, currentUser.CompanyId);

            if (created == null)
                throw new BusinessException("Erro ao criar colaborador.");

            return Ok(_mapper.Map<UserListDto>(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var currentUser = GetCurrentUserOrThrow();

            if (currentUser.Id != id && !currentUser.IsAdminOrSysAdmin())
                return Forbid("Você não tem permissão para alterar os dados de outro usuário.");

            var user = await _userService.GetByIdAsync(id);
            if (user == null || user.CompanyId != currentUser.CompanyId)
                return Forbid("Usuário não pertence à sua empresa.");

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (currentUser.Id != id && !currentUser.IsAdminOrSysAdmin())
                    return Forbid("Você não tem permissão para alterar o e-mail de outro usuário.");

                user.Email = dto.Email;
            }

            user.Name = dto.Name;

            var updated = await _userService.UpdateUserAsync(user);

            return Ok(_mapper.Map<UserListDto>(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var currentUser = GetCurrentUserOrThrow();

            if (!currentUser.IsAdminOrSysAdmin())
                return Forbid("Apenas administradores podem desativar colaboradores.");

            var user = await _userService.GetByIdAsync(id);
            if (user == null || user.CompanyId != currentUser.CompanyId)
                return Forbid("Usuário não pertence à sua empresa.");

            var deactivated = await _userService.DeactivateUserAsync(id);

            return Ok(_mapper.Map<UserListDto>(deactivated));
        }

        [HttpPost("{id}/reactivate")]
        public async Task<IActionResult> Reactivate(int id)
        {
            var currentUser = GetCurrentUserOrThrow();

            if (!currentUser.IsAdminOrSysAdmin())
                return Forbid("Apenas administradores podem reativar colaboradores.");

            var user = await _userService.GetByIdAsync(id);
            if (user == null || user.CompanyId != currentUser.CompanyId)
                return Forbid("Usuário não pertence à sua empresa.");

            var reactivated = await _userService.ReactivateUserAsync(id);

            return Ok(_mapper.Map<UserListDto>(reactivated));
        }
    }
}