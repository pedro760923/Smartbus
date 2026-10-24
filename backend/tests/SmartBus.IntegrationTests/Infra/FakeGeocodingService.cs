using SmartBus.Application.Abstractions;

namespace SmartBus.IntegrationTests.Infra;

/// <summary>
/// Dublê de IGeocodingService para os testes de integração não dependerem
/// de rede/Nominatim. Resultados configuráveis via Resultados, para os
/// poucos testes que precisam verificar o merge histórico+geocodificação.
/// </summary>
public class FakeGeocodingService : IGeocodingService
{
    public List<EnderecoGeocodificado> Resultados { get; set; } = new();

    public Task<List<EnderecoGeocodificado>> BuscarEnderecosAsync(string termo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Resultados);
    }
}
