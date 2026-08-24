using FluentAssertions;
using SmartBus.Application.Previsao;
using SmartBus.Domain.Entities;
using SmartBus.Domain.Enums;
using SmartBus.UnitTests.TestUtils;
using Xunit;

namespace SmartBus.UnitTests.Previsao;

public class PrevisaoServiceTests
{
    [Fact]
    public async Task PreverAsync_LinhaInexistente_DeveRetornarNulo()
    {
        using var db = InMemoryDbFactory.Criar();
        var service = new PrevisaoService(db);

        var resultado = await service.PreverAsync(999);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task PreverAsync_SemNenhumHistorico_DeveRetornarFallbackDeBaixaConfiabilidade()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = new Linha { Codigo = "437", Nome = "Linha A" };
        db.Linhas.Add(linha);
        await db.SaveChangesAsync();

        var service = new PrevisaoService(db);
        var resultado = await service.PreverAsync(linha.Id, new DateTime(2026, 8, 17, 8, 0, 0, DateTimeKind.Utc)); // segunda-feira

        resultado!.Amostras.Should().Be(0);
        resultado.Confiabilidade.Should().Be(0.1);
        resultado.NivelPrevisto.Should().Be(NivelLotacao.ComLugares);
    }

    [Fact]
    public async Task PreverAsync_ComAmostrasSuficientesNaFaixaExata_DeveUsarConfiabilidadeAlta()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = new Linha { Codigo = "437", Nome = "Linha A" };
        var usuario = new Usuario { Nome = "Aluno", Email = "aluno@fsa.edu.br", SenhaHash = "x" };
        db.Linhas.Add(linha);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var referencia = new DateTime(2026, 8, 17, 8, 0, 0, DateTimeKind.Utc); // segunda, faixa 08h-10h

        // 5 reportes na mesma segunda-feira, mesma faixa horária, todos "Cheio".
        for (var i = 0; i < 5; i++)
        {
            db.Reportes.Add(new Reporte
            {
                LinhaId = linha.Id,
                UsuarioId = usuario.Id,
                NivelLotacao = NivelLotacao.Cheio,
                Valido = true,
                CriadoEm = referencia.AddMinutes(-i * 10)
            });
        }
        await db.SaveChangesAsync();

        var service = new PrevisaoService(db);
        var resultado = await service.PreverAsync(linha.Id, referencia);

        resultado!.Amostras.Should().Be(5);
        resultado.NivelPrevisto.Should().Be(NivelLotacao.Cheio);
        // Menos de 10 amostras: confiabilidade escalada proporcionalmente (5/10 = 0.5).
        resultado.Confiabilidade.Should().Be(0.5);
    }

    [Fact]
    public async Task PreverAsync_ComPoucasAmostrasNaFaixaExata_DeveCairNoFallbackDeHistoricoGeral()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = new Linha { Codigo = "437", Nome = "Linha A" };
        var usuario = new Usuario { Nome = "Aluno", Email = "aluno@fsa.edu.br", SenhaHash = "x" };
        db.Linhas.Add(linha);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var referencia = new DateTime(2026, 8, 17, 8, 0, 0, DateTimeKind.Utc);

        // Só 2 reportes na faixa exata (abaixo do mínimo de 3) -> deve
        // cair no fallback de "todo o histórico da linha".
        db.Reportes.AddRange(
            new Reporte { LinhaId = linha.Id, UsuarioId = usuario.Id, NivelLotacao = NivelLotacao.Cheio, Valido = true, CriadoEm = referencia },
            new Reporte { LinhaId = linha.Id, UsuarioId = usuario.Id, NivelLotacao = NivelLotacao.Cheio, Valido = true, CriadoEm = referencia.AddMinutes(-5) },
            // reporte fora da faixa horária, mas ainda entra no fallback geral
            new Reporte { LinhaId = linha.Id, UsuarioId = usuario.Id, NivelLotacao = NivelLotacao.Vazio, Valido = true, CriadoEm = referencia.AddHours(10) }
        );
        await db.SaveChangesAsync();

        var service = new PrevisaoService(db);
        var resultado = await service.PreverAsync(linha.Id, referencia);

        resultado!.Amostras.Should().Be(3);
        resultado.Confiabilidade.Should().BeLessThan(0.5); // confiabilidade reduzida (fator 0.4 do fallback)
    }
}
