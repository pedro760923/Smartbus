using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SmartBus.Application.Auth;
using SmartBus.IntegrationTests.Infra;
using Xunit;

namespace SmartBus.IntegrationTests.Controllers;

public class LinhasControllerTests : IClassFixture<SmartBusWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LinhasControllerTests(SmartBusWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Listar_SemToken_DeveRetornarUnauthorized()
    {
        var resposta = await _client.GetAsync("api/linhas");

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Listar_ComTokenValido_DeveRetornarLinhasDoSeed()
    {
        await AutenticarAsync();

        var resposta = await _client.GetAsync("api/linhas");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var linhas = await resposta.Content.ReadFromJsonAsync<List<SmartBus.Application.Linhas.LinhaDto>>();
        linhas.Should().NotBeNull();
        linhas!.Should().Contain(l => l.Codigo == "437"); // linha do DbSeeder
    }

    [Fact]
    public async Task ObterPorId_IdInexistente_DeveRetornarNotFound()
    {
        await AutenticarAsync();

        var resposta = await _client.GetAsync("api/linhas/999999");

        resposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task AutenticarAsync()
    {
        var email = $"linhas.{Guid.NewGuid():N}@fsa.edu.br";
        var registro = await _client.PostAsJsonAsync("api/auth/registro",
            new RegistroRequest("Aluno", email, "senha123"));
        var auth = await registro.Content.ReadFromJsonAsync<AuthResponse>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
    }
}
