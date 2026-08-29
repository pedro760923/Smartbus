using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBus.Api.Common;
using SmartBus.Application.Enderecos;

namespace SmartBus.Api.Controllers;

[ApiController]
[Route(HttpExtensions.Enderecos.Base)]
[Authorize]
public class EnderecosController : ControllerBase
{
    private readonly IEnderecoService _enderecoService;

    public EnderecosController(IEnderecoService enderecoService)
    {
        _enderecoService = enderecoService;
    }

    [HttpGet(HttpExtensions.Enderecos.Get_Buscar)]
    public async Task<ActionResult<List<EnderecoSugestaoDto>>> Buscar([FromQuery] string? termo)
    {
        var usuarioId = User.ObterUsuarioIdAutenticado();
        return Ok(await _enderecoService.BuscarAsync(termo, usuarioId));
    }

    [HttpPost(HttpExtensions.Enderecos.Post_RegistrarHistorico)]
    public async Task<ActionResult<HistoricoBuscaDto>> RegistrarHistorico(NovoHistoricoRequest request)
    {
        var usuarioId = User.ObterUsuarioIdAutenticado();
        return Ok(await _enderecoService.RegistrarEscolhaAsync(request, usuarioId));
    }

    [HttpGet(HttpExtensions.Enderecos.Get_HistoricoRecente)]
    public async Task<ActionResult<List<HistoricoBuscaDto>>> HistoricoRecente()
    {
        var usuarioId = User.ObterUsuarioIdAutenticado();
        return Ok(await _enderecoService.HistoricoRecenteAsync(usuarioId));
    }
}
