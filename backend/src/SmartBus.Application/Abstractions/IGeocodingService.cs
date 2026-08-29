namespace SmartBus.Application.Abstractions;

/// <summary>
/// Porta (Ports &amp; Adapters) para um provedor externo de geocodificação
/// de endereços. A implementação real (Nominatim/OSM) fica em
/// SmartBus.Infrastructure — esta interface não conhece nada sobre
/// histórico de busca, isso é responsabilidade da camada Application.
/// </summary>
public record EnderecoGeocodificado(string Endereco, double Latitude, double Longitude);

public interface IGeocodingService
{
    Task<List<EnderecoGeocodificado>> BuscarEnderecosAsync(string termo, CancellationToken cancellationToken = default);
}
