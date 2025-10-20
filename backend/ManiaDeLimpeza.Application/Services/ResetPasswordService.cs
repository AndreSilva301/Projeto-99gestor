using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Infrastructure.Exceptions;
namespace ManiaDeLimpeza.Application.Services;
public class ResetPasswordService : IResetPasswordService, IScopedDependency
{

    private readonly IPasswordResetRepository _passwordResetRepository;
    private readonly IUserService _userService;

    public ResetPasswordService(
        IPasswordResetRepository passwordResetRepository,
        IUserService userService)
    {
        _passwordResetRepository = passwordResetRepository;
        _userService = userService;
    }

    public async Task<ApiResponse<string>> ResetAsync(ResetPasswordRequestDto dto)
    {
        if (!dto.IsValid())
            return ApiResponseHelper.ErrorResponse(string.Join(", ", dto.Validate()));

        var tokenData = await _passwordResetRepository.GetByTokenAsync(dto.Token);

        if (tokenData == null || tokenData.Expiration < DateTime.UtcNow)
            return ApiResponseHelper.ErrorResponse("Token inválido ou expirado");

        var user = tokenData.User;
        if (user == null)
            return ApiResponseHelper.ErrorResponse("Usuário não encontrado para o token informado.");

        try
        {
            await _userService.UpdatePasswordAsync(user, dto.NewPassword);
            return ApiResponseHelper.SuccessResponse("Senha redefinida com sucesso");
        }
        catch (BusinessException ex)
        {
            return ApiResponseHelper.ErrorResponse(ex.Message);
        }
    }
}
