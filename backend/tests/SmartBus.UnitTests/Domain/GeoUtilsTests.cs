using FluentAssertions;
using SmartBus.Domain.Common;
using Xunit;

namespace SmartBus.UnitTests.Domain;

public class GeoUtilsTests
{
    [Fact]
    public void DistanciaMetros_MesmoPonto_DeveSerZero()
    {
        var distancia = GeoUtils.DistanciaMetros(-23.6639, -46.5383, -23.6639, -46.5383);

        distancia.Should().Be(0);
    }

    [Fact]
    public void DistanciaMetros_PontosConhecidos_DeveAproximarValorEsperado()
    {
        // Dois pontos em Santo André, ~1km em linha reta (valor de referência calculado manualmente).
        var distancia = GeoUtils.DistanciaMetros(-23.6639, -46.5383, -23.6547, -46.5382);

        distancia.Should().BeApproximately(1024, 150);
    }

    [Fact]
    public void DistanciaMetros_DeveSerSimetrica()
    {
        var ab = GeoUtils.DistanciaMetros(-23.6639, -46.5383, -23.6547, -46.5382);
        var ba = GeoUtils.DistanciaMetros(-23.6547, -46.5382, -23.6639, -46.5383);

        ab.Should().BeApproximately(ba, 0.0001);
    }
}
