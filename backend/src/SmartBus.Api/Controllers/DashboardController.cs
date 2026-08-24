using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBus.Api.Common;
using SmartBus.Application.Dashboard;
using SmartBus.Domain.Enums;

namespace SmartBus.Api.Controllers;

[ApiController]
[Route(HttpExtensions.Dashboard.Base)]
[Authorize(Roles = nameof(PapelUsuario.Admin))]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet(HttpExtensions.Dashboard.Get_Kpis)]
    public async Task<ActionResult<DashboardKpisDto>> Kpis()
    {
        return Ok(await _dashboardService.ObterKpisAsync());
    }
}
