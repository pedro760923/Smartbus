using Microsoft.EntityFrameworkCore;
using SmartBus.Application.Abstractions;
using SmartBus.Domain.Entities;

namespace SmartBus.Application.Reportes;

public class ReporteService : IReporteService
{
    private static readonly TimeSpan JanelaRecentes = TimeSpan.FromHours(3);
    private const int LimiteReportesRecentes = 20;

    private readonly IApplicationDbContext _db;
    private readonly IValidacaoService _validacaoService;

    public ReporteService(IApplicationDbContext db, IValidacaoService validacaoService)
    {
        _db = db;
        _validacaoService = validacaoService;
    }

    public async Task<ReporteDto?> EnviarAsync(NovoReporteRequest request, int usuarioId)
    {
        var linha = await _db.Linhas.FindAsync(request.LinhaId);
        if (linha is null)
        {
            return null;
        }

        var reporte = new Reporte
        {
            LinhaId = request.LinhaId,
            ParadaId = request.ParadaId,
            UsuarioId = usuarioId,
            NivelLotacao = request.NivelLotacao,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        reporte.Valido = await _validacaoService.ValidarAsync(reporte);

        _db.Reportes.Add(reporte);
        await _db.SaveChangesAsync();

        return new ReporteDto(
            reporte.Id,
            reporte.LinhaId,
            linha.Nome,
            reporte.ParadaId,
            reporte.NivelLotacao,
            reporte.CriadoEm,
            reporte.Valido
        );
    }

    public async Task<List<ReporteDto>> RecentesPorLinhaAsync(int linhaId)
    {
        var limite = DateTime.UtcNow - JanelaRecentes;

        return await _db.Reportes
            .Where(r => r.LinhaId == linhaId && r.Valido && r.CriadoEm >= limite)
            .OrderByDescending(r => r.CriadoEm)
            .Include(r => r.Linha)
            .Take(LimiteReportesRecentes)
            .Select(r => new ReporteDto(r.Id, r.LinhaId, r.Linha.Nome, r.ParadaId, r.NivelLotacao, r.CriadoEm, r.Valido))
            .ToListAsync();
    }
}
