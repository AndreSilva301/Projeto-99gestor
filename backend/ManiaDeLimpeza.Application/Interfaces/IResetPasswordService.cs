using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;

namespace ManiaDeLimpeza.Application.Interfaces;
public interface IResetPasswordService
{
    Task<ApiResponse<string>> ResetAsync(ResetPasswordRequestDto dto);
}
