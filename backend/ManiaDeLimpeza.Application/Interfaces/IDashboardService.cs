using ManiaDeLimpeza.Application.Dtos;

namespace ManiaDeLimpeza.Application.Interfaces;
public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync(int companyId);
}
