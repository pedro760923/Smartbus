using Microsoft.EntityFrameworkCore;
using SmartBus.Application.Abstractions;
using SmartBus.Domain.Entities;

namespace SmartBus.Infrastructure.Persistence;

public class SmartBusDbContext : DbContext, IApplicationDbContext
{
    public SmartBusDbContext(DbContextOptions<SmartBusDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Linha> Linhas => Set<Linha>();
    public DbSet<Parada> Paradas => Set<Parada>();
    public DbSet<RotaParada> RotaParadas => Set<RotaParada>();
    public DbSet<Reporte> Reportes => Set<Reporte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Linha>()
            .HasIndex(l => l.Codigo)
            .IsUnique();

        modelBuilder.Entity<RotaParada>()
            .HasIndex(rp => new { rp.LinhaId, rp.Ordem })
            .IsUnique();

        modelBuilder.Entity<RotaParada>()
            .HasOne(rp => rp.Linha)
            .WithMany(l => l.RotaParadas)
            .HasForeignKey(rp => rp.LinhaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RotaParada>()
            .HasOne(rp => rp.Parada)
            .WithMany(p => p.RotaParadas)
            .HasForeignKey(rp => rp.ParadaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.Linha)
            .WithMany(l => l.Reportes)
            .HasForeignKey(r => r.LinhaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.Parada)
            .WithMany(p => p.Reportes)
            .HasForeignKey(r => r.ParadaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.Usuario)
            .WithMany(u => u.Reportes)
            .HasForeignKey(r => r.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reporte>()
            .HasIndex(r => new { r.LinhaId, r.CriadoEm });
    }
}
