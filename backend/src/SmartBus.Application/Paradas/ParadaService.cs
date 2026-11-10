using Microsoft.EntityFrameworkCore;
using SmartBus.Application.Abstractions;
using SmartBus.Domain.Common;

namespace SmartBus.Application.Paradas;

public class ParadaService : IParadaService
{
    public const double RaioPadraoMetros = 1000;

    private readonly IApplicationDbContext _db;

    public ParadaService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ParadaDto>> ListarAsync()
    {
        var paradas = await _db.Paradas.OrderBy(p => p.Nome).ToListAsync();
        return paradas.Select(p => new ParadaDto(p.Id, p.Nome, p.Latitude, p.Longitude)).ToList();
    }

    public async Task<List<ParadaDto>> ProximasAsync(double latitude, double longitude, double raioMetros)
    {
        var paradas = await _db.Paradas.ToListAsync();

        var linhasPorParada = (await _db.RotaParadas
                .Select(rp => new { rp.ParadaId, rp.Linha.Codigo })
                .ToListAsync())
            .GroupBy(x => x.ParadaId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Codigo).Distinct().OrderBy(c => c).ToList());

        return paradas
            .Select(p => new
            {
                Parada = p,
                Distancia = GeoUtils.DistanciaMetros(latitude, longitude, p.Latitude, p.Longitude)
            })
            .Where(x => x.Distancia <= raioMetros)
            .OrderBy(x => x.Distancia)
            .Select(x => new ParadaDto(x.Parada.Id, x.Parada.Nome, x.Parada.Latitude, x.Parada.Longitude, Math.Round(x.Distancia, 0), linhasPorParada.GetValueOrDefault(x.Parada.Id)))
            .ToList();
    }
}
