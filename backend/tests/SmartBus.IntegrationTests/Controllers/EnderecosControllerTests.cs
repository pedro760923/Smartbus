using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SmartBus.Application.Auth;
using SmartBus.Application.Enderecos;
using SmartBus.IntegrationTests.Infra;
using Xunit;

namespace SmartBus.IntegrationTests.Controllers;

public class EnderecosControllerTests : IClassFixture<SmartBusWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EnderecosControllerTests(SmartBusWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Buscar_SemToken_DeveRetornarUnauthorized()
    {
        var resposta = await _client.GetAsync("api/enderecos?termo=Santo Andre");

        resposta.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Buscar_SemTermo_DeveRetornarListaVazia_QuandoSemHistorico()
    {
        await AutenticarAsync();

        var resposta = await _client.GetAsync("api/enderecos");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var sugestoes = await resposta.Content.ReadFromJsonAsync<List<EnderecoSugestaoDto>>();
        sugestoes.Should().NotBeNull();
        sugestoes!.Should().BeEmpty();
    }

    [Fact]
    public async Task RegistrarHistorico_DevePersistirEDevolverNoHistoricoRecente()
    {
        await AutenticarAsync();

        var novo = new NovoHistoricoRequest("Terminal", "Terminal Santo André, SP", -23.6547, -46.5382);
        var registro = await _client.PostAsJsonAsync("api/enderecos/historico", novo);
        registro.StatusCode.Should().Be(HttpStatusCode.OK);

        var resposta = await _client.GetAsync("api/enderecos/historico");
        resposta.StatusCode.Should().Be(HttpStatusCode.OK);

        var historico = await resposta.Content.ReadFromJsonAsync<List<HistoricoBuscaDto>>();
        historico.Should().NotBeNull();
        historico!.Should().ContainSingle(h => h.Endereco == "Terminal Santo André, SP");
    }

    private async Task AutenticarAsync()
    {
        var email = $"enderecos.{Guid.NewGuid():N}@fsa.edu.br";
        var registro = await _client.PostAsJsonAsync("api/auth/registro",
            new RegistroRequest("Aluno", email, "senha123"));
        var auth = await registro.Content.ReadFromJsonAsync<AuthResponse>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
    }
}
