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
            var nivelAtual = await ObterNivelLotacaoAtualAsync(linha.Id);
            resultado.Add(new LinhaDto(linha.Id, linha.Codigo, linha.Nome, linha.Descricao, nivelAtual));
        }

        return resultado;
    }

    public async Task<LinhaDto?> ObterPorIdAsync(int id)
    {
        var linha = await _db.Linhas.FindAsync(id);
        if (linha is null) return null;

        var nivelAtual = await ObterNivelLotacaoAtualAsync(linha.Id);
        return new LinhaDto(linha.Id, linha.Codigo, linha.Nome, linha.Descricao, nivelAtual);
    }

    private async Task<NivelLotacao?> ObterNivelLotacaoAtualAsync(int linhaId)
    {
        var limite = DateTime.UtcNow - JanelaLotacaoAtual;

        return await _db.Reportes
            .Where(r => r.LinhaId == linhaId && r.Valido && r.CriadoEm >= limite)
            .OrderByDescending(r => r.CriadoEm)
            .Select(r => (NivelLotacao?)r.NivelLotacao)
            .FirstOrDefaultAsync();
    }
}
