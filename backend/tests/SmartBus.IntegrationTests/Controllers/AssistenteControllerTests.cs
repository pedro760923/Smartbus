using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SmartBus.Application.Assistente;
using SmartBus.Application.Auth;
using SmartBus.IntegrationTests.Infra;
using Xunit;

namespace SmartBus.IntegrationTests.Controllers;

public class AssistenteControllerTests : IClassFixture<SmartBusWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AssistenteControllerTests(SmartBusWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Mensagem_SemToken_DeveRetornarUnauthorized()
    {
        var resposta = await _client.PostAsJsonAsync("api/assistente/mensagem", new PerguntaChatRequest("oi", null));

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Mensagem_ComTokenValido_DeveRetornarRespostaDoAssistente()
    {
        await AutenticarAsync();

        var resposta = await _client.PostAsJsonAsync("api/assistente/mensagem",
            new PerguntaChatRequest("Quando passa o próximo ônibus da linha 069?", null));

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var corpo = await resposta.Content.ReadFromJsonAsync<RespostaChatDto>();
        corpo.Should().NotBeNull();
        corpo!.Resposta.Should().NotBeNullOrWhiteSpace();
    }

    private async Task AutenticarAsync()
    {
        var email = $"assistente.{Guid.NewGuid():N}@fsa.edu.br";
        var registro = await _client.PostAsJsonAsync("api/auth/registro",
            new RegistroRequest("Aluno", email, "senha123"));
        var auth = await registro.Content.ReadFromJsonAsync<AuthResponse>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
    }
}
