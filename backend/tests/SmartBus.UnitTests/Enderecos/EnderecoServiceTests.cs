using FluentAssertions;
using SmartBus.Application.Abstractions;
using SmartBus.Application.Enderecos;
using SmartBus.Domain.Entities;
using SmartBus.UnitTests.TestUtils;
using Xunit;

namespace SmartBus.UnitTests.Enderecos;

public class EnderecoServiceTests
{
    private static Usuario NovoUsuario() => new() { Nome = "Aluno", Email = $"{Guid.NewGuid():N}@fsa.edu.br", SenhaHash = "x" };

    [Fact]
    public async Task BuscarAsync_TermoVazio_DeveRetornarHistoricoRecenteDoUsuario()
    {
        using var db = InMemoryDbFactory.Criar();
        var usuario = NovoUsuario();
        db.Usuarios.Add(usuario);
        db.HistoricosBusca.Add(new HistoricoBusca { UsuarioId = usuario.Id, Termo = "terminal", Endereco = "Terminal Santo André, SP", Latitude = 1, Longitude = 2, CriadoEm = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var service = new EnderecoService(db, new FakeGeocodingService());
        var resultado = await service.BuscarAsync(null, usuario.Id);

        resultado.Should().ContainSingle();
        resultado[0].DoHistorico.Should().BeTrue();
        resultado[0].Endereco.Should().Be("Terminal Santo André, SP");
    }

    [Fact]
    public async Task BuscarAsync_TermoMenorQueTresCaracteres_DeveRetornarListaVazia()
    {
        using var db = InMemoryDbFactory.Criar();
        var usuario = NovoUsuario();
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var geocoding = new FakeGeocodingService { Resultados = [new EnderecoGeocodificado("Av. X", 1, 2)] };
        var service = new EnderecoService(db, geocoding);

        var resultado = await service.BuscarAsync("sp", usuario.Id);

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task BuscarAsync_ComTermo_DeveCombinarHistoricoEGeocodificacaoSemDuplicar()
    {
        using var db = InMemoryDbFactory.Criar();
        var usuario = NovoUsuario();
        db.Usuarios.Add(usuario);
        db.HistoricosBusca.Add(new HistoricoBusca { UsuarioId = usuario.Id, Termo = "avenida", Endereco = "Avenida Industrial, Santo André", Latitude = 1, Longitude = 2 });
        await db.SaveChangesAsync();

        var geocoding = new FakeGeocodingService
        {
            Resultados =
            [
                new EnderecoGeocodificado("Avenida Industrial, Santo André", 1, 2), // já está no histórico -> não deve duplicar
                new EnderecoGeocodificado("Avenida Kennedy, Santo André", 3, 4)
            ]
        };
        var service = new EnderecoService(db, geocoding);

        var resultado = await service.BuscarAsync("avenida", usuario.Id);

        resultado.Should().HaveCount(2);
        resultado.Should().ContainSingle(s => s.Endereco == "Avenida Industrial, Santo André" && s.DoHistorico);
        resultado.Should().ContainSingle(s => s.Endereco == "Avenida Kennedy, Santo André" && !s.DoHistorico);
    }

    [Fact]
    public async Task BuscarAsync_FalhaNaGeocodificacao_DeveRetornarSoResultadosDoHistorico()
    {
        using var db = InMemoryDbFactory.Criar();
        var usuario = NovoUsuario();
        db.Usuarios.Add(usuario);
        db.HistoricosBusca.Add(new HistoricoBusca { UsuarioId = usuario.Id, Termo = "avenida", Endereco = "Avenida Industrial, Santo André", Latitude = 1, Longitude = 2 });
        await db.SaveChangesAsync();

        var geocoding = new FakeGeocodingService { DeveFalhar = true };
        var service = new EnderecoService(db, geocoding);

        var resultado = await service.BuscarAsync("avenida", usuario.Id);

        resultado.Should().ContainSingle(s => s.DoHistorico);
    }

    [Fact]
    public async Task RegistrarEscolhaAsync_EnderecoNovo_DeveInserir()
    {
        using var db = InMemoryDbFactory.Criar();
        var usuario = NovoUsuario();
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var service = new EnderecoService(db, new FakeGeocodingService());
        await service.RegistrarEscolhaAsync(new NovoHistoricoRequest("terminal", "Terminal Santo André, SP", 1, 2), usuario.Id);

        db.HistoricosBusca.Should().ContainSingle(h => h.Endereco == "Terminal Santo André, SP" && h.UsuarioId == usuario.Id);
    }

    [Fact]
    public async Task RegistrarEscolhaAsync_EnderecoJaExistente_DeveAtualizarEmVezDeDuplicar()
    {
        using var db = InMemoryDbFactory.Criar();
        var usuario = NovoUsuario();
        db.Usuarios.Add(usuario);
        db.HistoricosBusca.Add(new HistoricoBusca { UsuarioId = usuario.Id, Termo = "term antigo", Endereco = "Terminal Santo André, SP", Latitude = 1, Longitude = 2, CriadoEm = DateTime.UtcNow.AddDays(-1) });
        await db.SaveChangesAsync();

        var service = new EnderecoService(db, new FakeGeocodingService());
        await service.RegistrarEscolhaAsync(new NovoHistoricoRequest("term novo", "Terminal Santo André, SP", 1, 2), usuario.Id);

        db.HistoricosBusca.Should().ContainSingle(h => h.UsuarioId == usuario.Id);
        db.HistoricosBusca.Single().Termo.Should().Be("term novo");
    }
}
