using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBus.Api.Common;
using SmartBus.Application.Reportes;

namespace SmartBus.Api.Controllers;

[ApiController]
[Route(HttpExtensions.Reportes.Base)]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reporteService;

    public ReportesController(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    [HttpPost(HttpExtensions.Reportes.Post_Enviar)]
    public async Task<ActionResult<ReporteDto>> Enviar(NovoReporteRequest request)
    {
        var usuarioId = User.ObterUsuarioIdAutenticado();
        var reporte = await _reporteService.EnviarAsync(request, usuarioId);

        return reporte is null
            ? NotFound(new { mensagem = "Linha não encontrada." })
            : Ok(reporte);
    }

    [HttpGet(HttpExtensions.Reportes.Get_RecentesPorLinha)]
    public async Task<ActionResult<List<ReporteDto>>> RecentesPorLinha(int linhaId)
    {
        return Ok(await _reporteService.RecentesPorLinhaAsync(linhaId));
    }
}
