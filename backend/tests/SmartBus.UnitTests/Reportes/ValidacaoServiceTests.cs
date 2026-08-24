using FluentAssertions;
using SmartBus.Application.Reportes;
using SmartBus.Domain.Entities;
using SmartBus.Domain.Enums;
using SmartBus.UnitTests.TestUtils;
using Xunit;

namespace SmartBus.UnitTests.Reportes;

public class ValidacaoServiceTests
{
    private static Linha NovaLinha() => new() { Codigo = "437", Nome = "Linha Teste" };
    private static Usuario NovoUsuario(int id) => new() { Id = id, Nome = "Aluno", Email = $"aluno{id}@fsa.edu.br", SenhaHash = "x" };

    [Fact]
    public async Task ValidarAsync_SemHistoricoSuficiente_DeveAceitar()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = NovaLinha();
        db.Linhas.Add(linha);
        await db.SaveChangesAsync();

        var service = new ValidacaoService(db);
        var reporte = new Reporte { LinhaId = linha.Id, UsuarioId = 1, NivelLotacao = NivelLotacao.SuperLotado };

        var valido = await service.ValidarAsync(reporte);

        // Menos de 3 reportes recentes na linha: não há base de comparação,
        // então o reporte é aceito por padrão.
        valido.Should().BeTrue();
    }

    [Fact]
    public async Task ValidarAsync_ReporteConsistenteComHistoricoRecente_DeveAceitar()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = NovaLinha();
        var usuario = NovoUsuario(1);
        db.Linhas.Add(linha);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        // 3 reportes recentes e válidos, todos "Cheio" -> média = Cheio (2).
        for (var i = 0; i < 3; i++)
        {
            db.Reportes.Add(new Reporte
            {
                LinhaId = linha.Id,
                UsuarioId = usuario.Id,
                NivelLotacao = NivelLotacao.Cheio,
                Valido = true,
                CriadoEm = DateTime.UtcNow.AddMinutes(-i)
            });
        }
        await db.SaveChangesAsync();

        var service = new ValidacaoService(db);
        var novoReporte = new Reporte { LinhaId = linha.Id, UsuarioId = usuario.Id, NivelLotacao = NivelLotacao.ComLugares };

        var valido = await service.ValidarAsync(novoReporte);

        valido.Should().BeTrue();
    }

    [Fact]
    public async Task ValidarAsync_ReporteMuitoDiscrepanteDoHistoricoRecente_DeveRejeitar()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = NovaLinha();
        var usuario = NovoUsuario(1);
        db.Linhas.Add(linha);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        // 3 reportes recentes "Vazio" (0) -> média = 0.
        for (var i = 0; i < 3; i++)
        {
            db.Reportes.Add(new Reporte
            {
                LinhaId = linha.Id,
                UsuarioId = usuario.Id,
                NivelLotacao = NivelLotacao.Vazio,
                Valido = true,
                CriadoEm = DateTime.UtcNow.AddMinutes(-i)
            });
        }
        await db.SaveChangesAsync();

        var service = new ValidacaoService(db);
        // SuperLotado (3) está a 3 níveis de distância de Vazio (0) -> acima do limite de 2.
        var novoReporte = new Reporte { LinhaId = linha.Id, UsuarioId = usuario.Id, NivelLotacao = NivelLotacao.SuperLotado };

        var valido = await service.ValidarAsync(novoReporte);

        valido.Should().BeFalse();
    }

    [Fact]
    public async Task ValidarAsync_IgnoraReportesForaDaJanelaDeTempo()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = NovaLinha();
        var usuario = NovoUsuario(1);
        db.Linhas.Add(linha);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        // 3 reportes "SuperLotado" só que fora da janela de 20 minutos.
        for (var i = 0; i < 3; i++)
        {
            db.Reportes.Add(new Reporte
            {
                LinhaId = linha.Id,
                UsuarioId = usuario.Id,
                NivelLotacao = NivelLotacao.SuperLotado,
                Valido = true,
                CriadoEm = DateTime.UtcNow.AddMinutes(-30 - i)
            });
        }
        await db.SaveChangesAsync();

        var service = new ValidacaoService(db);
        var novoReporte = new Reporte { LinhaId = linha.Id, UsuarioId = usuario.Id, NivelLotacao = NivelLotacao.Vazio };

        var valido = await service.ValidarAsync(novoReporte);

        // Sem reportes dentro da janela recente, cai no caso de "sem
        // histórico suficiente" e aceita.
        valido.Should().BeTrue();
    }
}
