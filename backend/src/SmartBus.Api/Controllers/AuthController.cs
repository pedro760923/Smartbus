using Microsoft.AspNetCore.Mvc;
using SmartBus.Api.Common;
using SmartBus.Application.Auth;

namespace SmartBus.Api.Controllers;

[ApiController]
[Route(HttpExtensions.Auth.Base)]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost(HttpExtensions.Auth.Post_Registro)]
    public async Task<ActionResult<AuthResponse>> Registrar(RegistroRequest request)
    {
        var resposta = await _authService.RegistrarAsync(request);
        if (resposta is null)
        {
            return Conflict(new { mensagem = "Já existe uma conta com este e-mail." });
        }

        return Ok(resposta);
    }

    [HttpPost(HttpExtensions.Auth.Post_Login)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var resposta = await _authService.LoginAsync(request);
        if (resposta is null)
        {
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
        }

        return Ok(resposta);
    }
}
