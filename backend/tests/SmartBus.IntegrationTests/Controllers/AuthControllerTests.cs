using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SmartBus.Application.Auth;
using SmartBus.IntegrationTests.Infra;
using Xunit;

namespace SmartBus.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<SmartBusWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(SmartBusWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Registrar_ComDadosValidos_DeveRetornarTokenEUsuario()
    {
        var request = new RegistroRequest("Aluno Teste", $"aluno.{Guid.NewGuid():N}@fsa.edu.br", "senha123");

        var resposta = await _client.PostAsJsonAsync("api/auth/registro", request);

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await resposta.Content.ReadFromJsonAsync<AuthResponse>();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.Usuario.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Registrar_ComEmailJaExistente_DeveRetornarConflict()
    {
        var email = $"duplicado.{Guid.NewGuid():N}@fsa.edu.br";
        var request = new RegistroRequest("Aluno", email, "senha123");

        await _client.PostAsJsonAsync("api/auth/registro", request);
        var segundaResposta = await _client.PostAsJsonAsync("api/auth/registro", request);

        segundaResposta.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ComSenhaIncorreta_DeveRetornarUnauthorized()
    {
        var email = $"login.{Guid.NewGuid():N}@fsa.edu.br";
        await _client.PostAsJsonAsync("api/auth/registro", new RegistroRequest("Aluno", email, "senhaCorreta"));

        var resposta = await _client.PostAsJsonAsync("api/auth/login", new LoginRequest(email, "senhaErrada"));

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
    {
        var email = $"loginok.{Guid.NewGuid():N}@fsa.edu.br";
        await _client.PostAsJsonAsync("api/auth/registro", new RegistroRequest("Aluno", email, "senha123"));

        var resposta = await _client.PostAsJsonAsync("api/auth/login", new LoginRequest(email, "senha123"));

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
