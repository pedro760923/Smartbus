namespace SmartBus.Domain.Common;

/// <summary>
/// Utilitário de geolocalização puro (sem dependências externas),
/// por isso vive no Domain: é regra de negócio, não detalhe de
/// infraestrutura.
/// </summary>
public static class GeoUtils
{
    private const double RaioTerraMetros = 6371000;

    /// <summary>
    /// Distância em metros entre dois pontos geográficos (fórmula de Haversine).
    /// Usada para localizar paradas próximas ao usuário.
    /// </summary>
    public static double DistanciaMetros(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = Radianos(lat2 - lat1);
        var dLon = Radianos(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(Radianos(lat1)) * Math.Cos(Radianos(lat2))
                * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return RaioTerraMetros * c;
    }

    private static double Radianos(double graus) => graus * Math.PI / 180.0;
}
