using AutoMapper;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ManiaDeLimpeza.Api.Controllers;
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
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(RegisterUserDto dto)
    {
        try
        {
            var user = _mapper.Map<User>(dto);
            var company = new Company
            {
                Name = dto.Company,
                CNPJ = dto.CNPJ
            };
            var createdUser = await _userService.CreateUserAsync(user);

            var result = _mapper.Map<AuthResponseDto>(createdUser);

            return ApiResponseHelper.SuccessResponse(result, "User registered successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseHelper.ErrorResponse<AuthResponseDto>(
                new List<string> { ex.Message },
                "User registration failed"
            );
        }
    }
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto dto)
    {
        User user;
        try
        {
            user = await _userService.GetByCredentialsAsync(dto.Email, dto.Password);
        }
        catch (BusinessException ex)
        {
            return Unauthorized(ApiResponseHelper.ErrorResponse<AuthResponseDto>(
                new List<string> { ex.Message }, "Unauthorized"));
        }

        if (user == null)
            return Unauthorized(ApiResponseHelper.ErrorResponse<AuthResponseDto>(
                new List<string> { "Invalid credentials" }, "Login failed"));

        var result = _mapper.Map<AuthResponseDto>(user);
        result.BearerToken = _tokenService.GenerateToken(user.Id.ToString(), user.Email);

        return Ok(ApiResponseHelper.SuccessResponse(result, "Login successful"));
    }
}