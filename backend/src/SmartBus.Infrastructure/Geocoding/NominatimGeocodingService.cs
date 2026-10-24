using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using SmartBus.Application.Abstractions;

namespace SmartBus.Infrastructure.Geocoding;

/// <summary>
/// Adaptador para o Nominatim (geocodificação do OpenStreetMap) — mesma
/// fonte de mapas já usada nos tiles do Leaflet no frontend. Gratuito e
/// sem API key, mas exige um User-Agent identificando a aplicação
/// (política de uso do Nominatim) e é sensível a rate limit, por isso o
/// frontend faz debounce antes de chamar essa busca.
/// </summary>
public class NominatimGeocodingService : IGeocodingService
{
    private readonly HttpClient _http;

    public NominatimGeocodingService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EnderecoGeocodificado>> BuscarEnderecosAsync(string termo, CancellationToken cancellationToken = default)
    {
        var query = Uri.EscapeDataString(termo);
        var url = $"search?q={query}&format=json&limit=5&countrycodes=br&addressdetails=0";

        var resultados = await _http.GetFromJsonAsync<List<NominatimResultado>>(url, cancellationToken)
            ?? new List<NominatimResultado>();

        return resultados
            .Where(r => r.DisplayName is not null && r.Lat is not null && r.Lon is not null)
            .Select(r => new EnderecoGeocodificado(
                r.DisplayName!,
                double.Parse(r.Lat!, CultureInfo.InvariantCulture),
                double.Parse(r.Lon!, CultureInfo.InvariantCulture)))
            .ToList();
    }

    /// <summary>
    /// O Nominatim retorna "lat"/"lon" como strings JSON, não números —
    /// desserializar direto para double lançaria exceção em toda chamada.
    /// </summary>
    private record NominatimResultado(
        [property: JsonPropertyName("display_name")] string? DisplayName,
        [property: JsonPropertyName("lat")] string? Lat,
        [property: JsonPropertyName("lon")] string? Lon
    );
}
