using AutoMapper;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.Exceptions;
using ManiaDeLimpeza.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ManiaDeLimpeza.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICompanyServices _companyServices;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _dbContext;
    private readonly IForgotPasswordService _forgotPasswordService;
    private readonly IPasswordResetRepository _passwordResetRepository;


    public AuthController(
        IUserService userService,
        ICompanyServices companyServices,
        ITokenService tokenService,
        IMapper mapper,
        ApplicationDbContext dbContext,
        IForgotPasswordService forgotPasswordService,
        IPasswordResetRepository passwordResetRepository)
    {
        _userService = userService;
        _mapper = mapper;
        _tokenService = tokenService;
        _companyServices = companyServices;
        _dbContext = dbContext;
        _forgotPasswordService = forgotPasswordService;
        this._passwordResetRepository = passwordResetRepository;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(RegisterUserDto dto)
    {
        IDbContextTransaction transaction = null;

        try
        {
            var errors = dto.Validate();

            if (errors.Count > 0)
            {
                return BadRequest(ApiResponseHelper.ErrorResponse<AuthResponseDto>(
                    errors,
                    "User registration failed"
                ));
            }

            transaction = await _dbContext.Database.BeginTransactionAsync();

            var company = new Company { Name = dto.CompanyName };
            company = await _companyServices.CreateCompanyAsync(company);

            var user = _mapper.Map<User>(dto);
            user.CompanyId = company.Id;

            var createdUser = await _userService.CreateUserAsync(user, dto.Password);

            await transaction.CommitAsync();

            var result = _mapper.Map<AuthResponseDto>(createdUser);
            result.BearerToken = _tokenService.GenerateToken(createdUser.Id.ToString(), createdUser.Email);
            var response = ApiResponseHelper.SuccessResponse(result, "User registered successfully");

            return Created("/User", response);
        }
        catch (Exception ex)
        {
            await transaction?.RollbackAsync();

            return BadRequest(ApiResponseHelper.ErrorResponse<AuthResponseDto>(
                new List<string> { ex.Message },
                "User registration failed"
            ));
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
                new List<string> { "Invalid email or password" }, "Unauthorized"));

        var result = _mapper.Map<AuthResponseDto>(user);
        result.BearerToken = _tokenService.GenerateToken(user.Id.ToString(), user.Email);

        return Ok(ApiResponseHelper.SuccessResponse(result, "Login successful"));
    }


    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
    {
        var (isValid, errorMessage) = dto.IsValid(); 

        if (!isValid)
            return BadRequest(ApiResponseHelper.ErrorResponse(errorMessage));

        try
        {
            await _forgotPasswordService.SendResetPasswordEmailAsync(dto.Email);
        }
        catch (Exception ex) // A resposta para o usuário nesse método sempre deve ser Ok, por questão de segurança.
        {
            Console.WriteLine($"Erro ao processar ForgotPassword: {ex.Message}");
        }

        return Ok(ApiResponseHelper.SuccessResponse("E-mail de recuperação enviado com sucesso"));
    }


    [HttpPost("verify-reset-token")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> VerifyResetToken([FromBody] string token)
    {
        var tokenData = await _forgotPasswordService.VerifyResetTokenAsync(token);

        if (tokenData == null)
            return BadRequest(ApiResponseHelper.ErrorResponse("Token inválido ou expirado"));

        return Ok(ApiResponseHelper.SuccessResponse(new
        {
            email = tokenData.User.Email,
            expiresAt = tokenData.Expiration
        }, "Token válido"));
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<string>>> ResetPassword([FromBody] ResetPasswordRequestDto dto,
                                                                       [FromServices] IResetPasswordService resetPasswordService)
    {
        var response = await resetPasswordService.ResetAsync(dto);
        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}