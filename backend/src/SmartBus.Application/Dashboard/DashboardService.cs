using Microsoft.EntityFrameworkCore;
using SmartBus.Application.Abstractions;
using SmartBus.Application.Previsao;
using SmartBus.Domain.Enums;

namespace SmartBus.Application.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IApplicationDbContext _db;
    private readonly IPrevisaoService _previsaoService;

    public DashboardService(IApplicationDbContext db, IPrevisaoService previsaoService)
    {
        _db = db;
        _previsaoService = previsaoService;
    }

    public async Task<DashboardKpisDto> ObterKpisAsync()
    {
        var hoje = DateTime.UtcNow.Date;

        var totalReportesHoje = await _db.Reportes.CountAsync(r => r.CriadoEm >= hoje);
        var totalLinhasMonitoradas = await _db.Linhas.CountAsync();

        var linhasComMaiorSuperlotacao = await _db.Reportes
            .Where(r => r.Valido)
            .GroupBy(r => new { r.LinhaId, r.Linha.Nome })
            .Select(g => new LinhaSuperlotacaoDto(
                g.Key.Nome,
                Math.Round(
                    100.0 * g.Count(r => r.NivelLotacao == NivelLotacao.SuperLotado) / g.Count(),
                    1)
            ))
            .OrderByDescending(x => x.PercentualSuperlotado)
            .Take(5)
            .ToListAsync();

        var mapaDeCalor = await _db.Reportes
            .Where(r => r.Valido && r.ParadaId != null)
            .GroupBy(r => new { r.ParadaId, r.Parada!.Nome, r.Parada.Latitude, r.Parada.Longitude })
            .Select(g => new PontoCalorDto(
                g.Key.ParadaId!.Value,
                g.Key.Nome,
                g.Key.Latitude,
                g.Key.Longitude,
                Math.Round(g.Average(r => (int)r.NivelLotacao) / 3.0, 2)
            ))
            .ToListAsync();

        var previsaoTempoEspera = await _previsaoService.PreverTempoEsperaGeralAsync();

        return new DashboardKpisDto(
            totalReportesHoje,
            totalLinhasMonitoradas,
            linhasComMaiorSuperlotacao,
            mapaDeCalor,
            previsaoTempoEspera
        );
    }
}
