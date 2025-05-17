using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AuthController(
            IUserService userService,
            ITokenService tokenService,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterUserDto dto)
        {
            var user = _mapper.Map<User>(dto);

            var created = await _userService.CreateUserAsync(user);

            var result = _mapper.Map<AuthResponseDto>(created);

            return CreatedAtAction(nameof(Register), result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _userService.GetByCredentialsAsync(dto.Email, dto.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");
                                    
            var result = _mapper.Map<AuthResponseDto>(user);
            result.BearerToken = _tokenService.GenerateToken(user.Id.ToString(), user.Email);

            return Ok(result);
        }
    }
}
