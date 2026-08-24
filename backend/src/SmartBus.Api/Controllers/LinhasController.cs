using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBus.Api.Common;
using SmartBus.Application.Linhas;

namespace SmartBus.Api.Controllers;

[ApiController]
[Route(HttpExtensions.Linhas.Base)]
[Authorize]
public class LinhasController : ControllerBase
{
    private readonly ILinhaService _linhaService;

    public LinhasController(ILinhaService linhaService)
    {
        _linhaService = linhaService;
    }

    [HttpGet(HttpExtensions.Linhas.Get_Listar)]
    public async Task<ActionResult<List<LinhaDto>>> Listar([FromQuery] string? termo)
    {
        return Ok(await _linhaService.ListarAsync(termo));
    }

    [HttpGet(HttpExtensions.Linhas.Get_ObterPorId)]
    public async Task<ActionResult<LinhaDto>> ObterPorId(int id)
    {
        var linha = await _linhaService.ObterPorIdAsync(id);
        return linha is null ? NotFound() : Ok(linha);
    }
}
