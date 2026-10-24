using SmartBus.Application.Abstractions;

namespace SmartBus.UnitTests.Enderecos;

public class FakeGeocodingService : IGeocodingService
{
    public List<EnderecoGeocodificado> Resultados { get; set; } = new();
    public bool DeveFalhar { get; set; }

    public Task<List<EnderecoGeocodificado>> BuscarEnderecosAsync(string termo, CancellationToken cancellationToken = default)
    {
        if (DeveFalhar)
        {
            throw new HttpRequestException("Falha simulada de rede.");
        }

        return Task.FromResult(Resultados);
    }
}
