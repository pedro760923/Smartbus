using FluentAssertions;
using SmartBus.Application.Linhas;
using SmartBus.Domain.Entities;
using SmartBus.Domain.Enums;
using SmartBus.UnitTests.TestUtils;
using Xunit;

namespace SmartBus.UnitTests.Linhas;

/// <summary>
/// Cobre especificamente a regra que motivou o refactor: o cálculo de
/// "nível de lotação atual" (janela de 30min sobre reportes válidos,
/// pegando o mais recente). Esta é a lógica que antes vivia duplicada
/// implicitamente em mais de um controller.
/// </summary>
public class LinhaServiceTests
{
    [Fact]
    public async Task ListarAsync_SemReportesRecentes_NivelAtualDeveSerNulo()
    {
        using var db = InMemoryDbFactory.Criar();
        db.Linhas.Add(new Linha { Codigo = "437", Nome = "Linha A" });
        await db.SaveChangesAsync();

        var service = new LinhaService(db);
        var resultado = await service.ListarAsync(null);

        resultado.Single().NivelLotacaoAtual.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorIdAsync_ComReportesDentroDaJanela_DeveRetornarNivelMaisRecente()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = new Linha { Codigo = "437", Nome = "Linha A" };
        var usuario = new Usuario { Nome = "Aluno", Email = "aluno@fsa.edu.br", SenhaHash = "x" };
        db.Linhas.Add(linha);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        db.Reportes.AddRange(
            new Reporte { LinhaId = linha.Id, UsuarioId = usuario.Id, NivelLotacao = NivelLotacao.Cheio, Valido = true, CriadoEm = DateTime.UtcNow.AddMinutes(-20) },
            new Reporte { LinhaId = linha.Id, UsuarioId = usuario.Id, NivelLotacao = NivelLotacao.SuperLotado, Valido = true, CriadoEm = DateTime.UtcNow.AddMinutes(-5) }
        );
        await db.SaveChangesAsync();

        var service = new LinhaService(db);
        var resultado = await service.ObterPorIdAsync(linha.Id);

        resultado!.NivelLotacaoAtual.Should().Be(NivelLotacao.SuperLotado);
    }

    [Fact]
    public async Task ObterPorIdAsync_ReporteForaDaJanelaDe30Min_DeveSerIgnorado()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = new Linha { Codigo = "437", Nome = "Linha A" };
        var usuario = new Usuario { Nome = "Aluno", Email = "aluno@fsa.edu.br", SenhaHash = "x" };
        db.Linhas.Add(linha);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        db.Reportes.Add(new Reporte
        {
            LinhaId = linha.Id,
            UsuarioId = usuario.Id,
            NivelLotacao = NivelLotacao.SuperLotado,
            Valido = true,
            CriadoEm = DateTime.UtcNow.AddMinutes(-31)
        });
        await db.SaveChangesAsync();

        var service = new LinhaService(db);
        var resultado = await service.ObterPorIdAsync(linha.Id);

        resultado!.NivelLotacaoAtual.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorIdAsync_ReporteInvalido_DeveSerIgnorado()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = new Linha { Codigo = "437", Nome = "Linha A" };
        var usuario = new Usuario { Nome = "Aluno", Email = "aluno@fsa.edu.br", SenhaHash = "x" };
        db.Linhas.Add(linha);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        db.Reportes.Add(new Reporte
        {
            LinhaId = linha.Id,
            UsuarioId = usuario.Id,
            NivelLotacao = NivelLotacao.SuperLotado,
            Valido = false,
            CriadoEm = DateTime.UtcNow.AddMinutes(-1)
        });
        await db.SaveChangesAsync();

        var service = new LinhaService(db);
        var resultado = await service.ObterPorIdAsync(linha.Id);

        resultado!.NivelLotacaoAtual.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorIdAsync_LinhaInexistente_DeveRetornarNulo()
    {
        using var db = InMemoryDbFactory.Criar();
        var service = new LinhaService(db);

        var resultado = await service.ObterPorIdAsync(999);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ListarAsync_ComTermo_DeveFiltrarPorCodigoOuNome()
    {
        using var db = InMemoryDbFactory.Criar();
        db.Linhas.AddRange(
            new Linha { Codigo = "437", Nome = "Terminal Santo André" },
            new Linha { Codigo = "512", Nome = "Vila Assunção" }
        );
        await db.SaveChangesAsync();

        var service = new LinhaService(db);
        var resultado = await service.ListarAsync("437");

        resultado.Should().ContainSingle(l => l.Codigo == "437");
    }
}
