using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DashboardController : AuthBaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("estatisticas")]
    [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _dashboardService.GetStatsAsync(CurrentUser.CompanyId);

        return Ok(new ApiResponse<DashboardStatsDto>(stats));
    }
}