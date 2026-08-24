using Microsoft.EntityFrameworkCore;
using SmartBus.Application.Abstractions;
using SmartBus.Domain.Entities;

namespace SmartBus.Application.Reportes;

public class ValidacaoService : IValidacaoService
{
    private static readonly TimeSpan JanelaRecente = TimeSpan.FromMinutes(20);
    private const int DiferencaMaximaAceitavel = 2; // níveis de distância no enum NivelLotacao
    private const int MinimoReportesParaComparar = 3;

    private readonly IApplicationDbContext _db;

    public ValidacaoService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> ValidarAsync(Reporte reporte)
    {
        var limite = DateTime.UtcNow - JanelaRecente;

        var reportesRecentes = await _db.Reportes
            .Where(r => r.LinhaId == reporte.LinhaId && r.CriadoEm >= limite && r.Valido)
            .Select(r => r.NivelLotacao)
            .ToListAsync();

        // Sem histórico suficiente para comparar: aceita o reporte, ele
        // próprio vira parte da base de referência futura.
        if (reportesRecentes.Count < MinimoReportesParaComparar)
        {
            return true;
        }

        var mediaRecente = reportesRecentes.Average(n => (int)n);
        var diferenca = Math.Abs((int)reporte.NivelLotacao - mediaRecente);

        return diferenca <= DiferencaMaximaAceitavel;
    }
}
