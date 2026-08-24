using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBus.Api.Common;
using SmartBus.Application.Rotas;

namespace SmartBus.Api.Controllers;

[ApiController]
[Route(HttpExtensions.Rotas.Base)]
[Authorize]
public class RotasController : ControllerBase
{
    private readonly IRotaService _rotaService;

    public RotasController(IRotaService rotaService)
    {
        _rotaService = rotaService;
    }

    [HttpGet(HttpExtensions.Rotas.Get_ObterPorLinha)]
    public async Task<ActionResult<RotaDto>> ObterPorLinha(int linhaId)
    {
        var rota = await _rotaService.ObterPorLinhaAsync(linhaId);
        return rota is null ? NotFound() : Ok(rota);
    }
}
