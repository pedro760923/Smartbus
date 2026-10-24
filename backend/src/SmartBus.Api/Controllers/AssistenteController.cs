using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBus.Api.Common;
using SmartBus.Application.Assistente;

namespace SmartBus.Api.Controllers;

[ApiController]
[Route(HttpExtensions.Assistente.Base)]
[Authorize]
public class AssistenteController : ControllerBase
{
    private readonly IAssistenteService _assistenteService;

    public AssistenteController(IAssistenteService assistenteService)
    {
        _assistenteService = assistenteService;
    }

    [HttpPost(HttpExtensions.Assistente.Post_Mensagem)]
    public async Task<ActionResult<RespostaChatDto>> Mensagem(PerguntaChatRequest request)
    {
        var usuarioId = User.ObterUsuarioIdAutenticado();
        var resposta = await _assistenteService.ResponderAsync(request.Mensagem, request.Historico, usuarioId);
        return Ok(new RespostaChatDto(resposta));
    }
}
