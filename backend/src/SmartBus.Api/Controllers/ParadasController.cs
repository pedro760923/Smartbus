using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBus.Api.Common;
using SmartBus.Application.Paradas;

namespace SmartBus.Api.Controllers;

[ApiController]
[Route(HttpExtensions.Paradas.Base)]
[Authorize]
public class ParadasController : ControllerBase
{
    private readonly IParadaService _paradaService;

    public ParadasController(IParadaService paradaService)
    {
        _paradaService = paradaService;
    }

    [HttpGet(HttpExtensions.Paradas.Get_Listar)]
    public async Task<ActionResult<List<ParadaDto>>> Listar()
    {
        return Ok(await _paradaService.ListarAsync());
    }

    [HttpGet(HttpExtensions.Paradas.Get_Proximas)]
    public async Task<ActionResult<List<ParadaDto>>> Proximas(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double raioMetros = ParadaService.RaioPadraoMetros)
    {
        return Ok(await _paradaService.ProximasAsync(latitude, longitude, raioMetros));
    }
}
