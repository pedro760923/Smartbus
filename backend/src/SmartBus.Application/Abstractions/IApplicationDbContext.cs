using Microsoft.EntityFrameworkCore;
using SmartBus.Domain.Entities;

namespace SmartBus.Application.Abstractions;

/// <summary>
/// Porta (Ports &amp; Adapters) que a camada Application usa para acessar
/// dados, sem depender da Infrastructure (EF Core concreto, SQLite etc.).
/// A implementação real fica em SmartBus.Infrastructure.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Usuario> Usuarios { get; }
    DbSet<Linha> Linhas { get; }
    DbSet<Parada> Paradas { get; }
    DbSet<RotaParada> RotaParadas { get; }
    DbSet<Reporte> Reportes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
