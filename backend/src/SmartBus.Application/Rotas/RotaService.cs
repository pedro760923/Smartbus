using Microsoft.EntityFrameworkCore;
using SmartBus.Application.Abstractions;

namespace SmartBus.Application.Rotas;

public class RotaService : IRotaService
{
    private readonly IApplicationDbContext _db;

    public RotaService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<RotaDto?> ObterPorLinhaAsync(int linhaId)
    {
        var linha = await _db.Linhas.FindAsync(linhaId);
        if (linha is null) return null;

        var paradas = await _db.RotaParadas
            .Where(rp => rp.LinhaId == linhaId)
            .OrderBy(rp => rp.Ordem)
            .Select(rp => new RotaParadaDto(rp.Parada.Id, rp.Parada.Nome, rp.Parada.Latitude, rp.Parada.Longitude, rp.Ordem))
            .ToListAsync();

        return new RotaDto(linha.Id, linha.Codigo, linha.Nome, paradas);
    }
}
