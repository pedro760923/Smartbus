using Microsoft.EntityFrameworkCore;
using SmartBus.Application.Abstractions;
using SmartBus.Domain.Enums;

namespace SmartBus.Application.Linhas;

/// <summary>
/// Único lugar do sistema que sabe calcular o "nível de lotação atual"
/// de uma linha (janela deslizante sobre os reportes recentes). Antes
/// do refactor essa regra estava duplicada/implícita entre controllers;
/// centralizar aqui é o que evita a regressão mais provável do projeto
/// (dois lugares divergindo sobre o que "lotação atual" significa).
/// </summary>
public class LinhaService : ILinhaService
{
    private static readonly TimeSpan JanelaLotacaoAtual = TimeSpan.FromMinutes(30);

    private readonly IApplicationDbContext _db;

    public LinhaService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<LinhaDto>> ListarAsync(string? termo)
    {
        var query = _db.Linhas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            query = query.Where(l => l.Codigo.Contains(termo) || l.Nome.Contains(termo));
        }

        var linhas = await query.OrderBy(l => l.Codigo).ToListAsync();

        var resultado = new List<LinhaDto>(linhas.Count);
        foreach (var linha in linhas)
        {
            var resumo = await ObterResumoLotacaoAsync(linha.Id);
            resultado.Add(new LinhaDto(linha.Id, linha.Codigo, linha.Nome, linha.Descricao, resumo.Nivel, resumo.Total, resumo.Distribuicao));
        }

        return resultado;
    }

    public async Task<LinhaDto?> ObterPorIdAsync(int id)
    {
        var linha = await _db.Linhas.FindAsync(id);
        if (linha is null) return null;

        var resumo = await ObterResumoLotacaoAsync(linha.Id);
        return new LinhaDto(linha.Id, linha.Codigo, linha.Nome, linha.Descricao, resumo.Nivel, resumo.Total, resumo.Distribuicao);
    }

    private record ResumoLotacao(NivelLotacao? Nivel, int Total, List<VotoNivelDto> Distribuicao);

    /// <summary>
    /// Calcula o nível de lotação "atual" de uma linha pela MAIORIA dos
    /// votos válidos na janela deslizante — não pelo relato mais recente.
    /// Em caso de empate no número de votos, vence o grupo com o voto mais
    /// recente (assim, 30 "lotado" contra 1 "vazio" recente ainda mostra
    /// "lotado", mas 1 "lotado" contra 1 "vazio" mais recente desempata
    /// pela recência).
    ///
    /// Agrupamento feito em memória (não via GroupBy traduzido pelo EF)
    /// de propósito: a janela é sempre pequena (poucos reportes de uma
    /// linha em 30min) e evita depender de tradução de agregados do
    /// provedor MySQL/Pomelo, que não é coberta por nenhum teste do
    /// projeto contra o banco real (só InMemory/SQLite nos testes).
    /// </summary>
    private async Task<ResumoLotacao> ObterResumoLotacaoAsync(int linhaId)
    {
        var limite = DateTime.UtcNow - JanelaLotacaoAtual;

        var relatos = await _db.Reportes
            .Where(r => r.LinhaId == linhaId && r.Valido && r.CriadoEm >= limite)
            .Select(r => new { r.NivelLotacao, r.CriadoEm })
            .ToListAsync();

        var grupos = relatos
            .GroupBy(r => r.NivelLotacao)
            .Select(g => new { Nivel = g.Key, Quantidade = g.Count(), UltimoReporte = g.Max(r => r.CriadoEm) })
            .ToList();

        var total = grupos.Sum(g => g.Quantidade);

        var vencedor = grupos
            .OrderByDescending(g => g.Quantidade)
            .ThenByDescending(g => g.UltimoReporte)
            .FirstOrDefault();

        var distribuicao = grupos
            .Select(g => new VotoNivelDto(g.Nivel, g.Quantidade, total == 0 ? 0 : Math.Round(100.0 * g.Quantidade / total, 1)))
            .OrderByDescending(v => v.Quantidade)
            .ToList();

        return new ResumoLotacao(vencedor?.Nivel, total, distribuicao);
    }
}
