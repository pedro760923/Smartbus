using FluentAssertions;
using SmartBus.Application.Paradas;
using SmartBus.Domain.Entities;
using SmartBus.UnitTests.TestUtils;
using Xunit;

namespace SmartBus.UnitTests.Paradas;

public class ParadaServiceTests
{
    [Fact]
    public async Task ProximasAsync_DeveExcluirParadasForaDoRaio_EOrdenarPorDistancia()
    {
        using var db = InMemoryDbFactory.Criar();

        var perto = new Parada { Nome = "Perto", Latitude = -23.6639, Longitude = -46.5383 };
        var mediana = new Parada { Nome = "Mediana", Latitude = -23.6547, Longitude = -46.5382 }; // ~1km
        var longe = new Parada { Nome = "Longe", Latitude = -23.5000, Longitude = -46.4000 };      // dezenas de km

        db.Paradas.AddRange(perto, mediana, longe);
        await db.SaveChangesAsync();

        var service = new ParadaService(db);
        var resultado = await service.ProximasAsync(-23.6639, -46.5383, raioMetros: 2000);

        resultado.Select(p => p.Nome).Should().Equal("Perto", "Mediana");
    }

    [Fact]
    public async Task ProximasAsync_NenhumaParadaNoRaio_DeveRetornarListaVazia()
    {
        using var db = InMemoryDbFactory.Criar();
        db.Paradas.Add(new Parada { Nome = "Longe", Latitude = -23.5000, Longitude = -46.4000 });
        await db.SaveChangesAsync();

        var service = new ParadaService(db);
        var resultado = await service.ProximasAsync(-23.6639, -46.5383, raioMetros: 500);

        resultado.Should().BeEmpty();
    }
}
