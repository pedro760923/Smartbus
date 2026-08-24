using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SmartBus.Application.Auth;
using SmartBus.Application.Reportes;
using SmartBus.Domain.Enums;
using SmartBus.IntegrationTests.Infra;
using Xunit;

namespace SmartBus.IntegrationTests.Controllers;

public class ReportesControllerTests : IClassFixture<SmartBusWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ReportesControllerTests(SmartBusWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Enviar_ParaLinhaExistente_DeveCriarReporteEDevolveLo()
    {
        await AutenticarAsync();
        var linhaId = await ObterPrimeiraLinhaIdAsync();

        var resposta = await _client.PostAsJsonAsync("api/reportes",
            new NovoReporteRequest(linhaId, null, NivelLotacao.Cheio, null, null));

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var reporte = await resposta.Content.ReadFromJsonAsync<ReporteDto>();
        reporte!.LinhaId.Should().Be(linhaId);
        reporte.NivelLotacao.Should().Be(NivelLotacao.Cheio);
    }

    [Fact]
    public async Task Enviar_ParaLinhaInexistente_DeveRetornarNotFound()
    {
        await AutenticarAsync();

        var resposta = await _client.PostAsJsonAsync("api/reportes",
            new NovoReporteRequest(999999, null, NivelLotacao.Cheio, null, null));

        resposta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RecentesPorLinha_AposEnviarReporte_DeveListarNoRetorno()
    {
        await AutenticarAsync();
        var linhaId = await ObterPrimeiraLinhaIdAsync();

        await _client.PostAsJsonAsync("api/reportes",
            new NovoReporteRequest(linhaId, null, NivelLotacao.SuperLotado, null, null));

        var resposta = await _client.GetAsync($"api/reportes/linha/{linhaId}/recentes");

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var reportes = await resposta.Content.ReadFromJsonAsync<List<ReporteDto>>();
        reportes.Should().NotBeEmpty();
    }

    private async Task<int> ObterPrimeiraLinhaIdAsync()
    {
        var linhas = await _client.GetFromJsonAsync<List<SmartBus.Application.Linhas.LinhaDto>>("api/linhas");
        return linhas!.First().Id;
    }

    private async Task AutenticarAsync()
    {
        var email = $"reportes.{Guid.NewGuid():N}@fsa.edu.br";
        var registro = await _client.PostAsJsonAsync("api/auth/registro",
            new RegistroRequest("Aluno", email, "senha123"));
        var auth = await registro.Content.ReadFromJsonAsync<AuthResponse>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
    }
}
