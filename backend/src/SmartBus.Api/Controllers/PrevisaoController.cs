using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBus.Api.Common;
using SmartBus.Application.Previsao;

namespace SmartBus.Api.Controllers;

[ApiController]
[Route(HttpExtensions.Previsao.Base)]
[Authorize]
public class PrevisaoController : ControllerBase
{
    private readonly IPrevisaoService _previsaoService;

    public PrevisaoController(IPrevisaoService previsaoService)
    {
        _previsaoService = previsaoService;
    }

    [HttpGet(HttpExtensions.Previsao.Get_ObterPorLinha)]
    public async Task<IActionResult> ObterPorLinha(int linhaId)
    {
        var previsao = await _previsaoService.PreverAsync(linhaId);
        return previsao is null ? NotFound() : Ok(previsao);
    }
}
