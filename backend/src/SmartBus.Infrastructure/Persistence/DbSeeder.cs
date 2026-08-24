using Microsoft.EntityFrameworkCore;
using SmartBus.Domain.Entities;
using SmartBus.Domain.Enums;

namespace SmartBus.Infrastructure.Persistence;

/// <summary>
/// Popula o banco com dados de exemplo (linhas e paradas próximas ao
/// campus da FSA em Santo André) para permitir testar a aplicação de
/// ponta a ponta sem depender de integração com as concessionárias.
/// </summary>
public static class DbSeeder
{
    public static void Seed(SmartBusDbContext db)
    {
        // Este projeto usa EnsureCreated() para simplificar o setup inicial.
        // Para evoluir o schema em produção, gere e aplique migrations reais:
        // `dotnet ef migrations add NomeDaMigracao` e `dotnet ef database
        // update` (troque para `db.Database.Migrate()` assim que a primeira
        // migration existir).
        db.Database.EnsureCreated();

        if (db.Linhas.Any())
        {
            return;
        }

        var paradaCampus = new Parada { Nome = "Campus FSA - Portão Principal", Latitude = -23.6639, Longitude = -46.5383 };
        var paradaCentro = new Parada { Nome = "Terminal Santo André", Latitude = -23.6547, Longitude = -46.5382 };
        var paradaVilaAssuncao = new Parada { Nome = "Vila Assunção", Latitude = -23.6688, Longitude = -46.5296 };
        var paradaCapuava = new Parada { Nome = "Jardim Capuava", Latitude = -23.6459, Longitude = -46.5175 };

        db.Paradas.AddRange(paradaCampus, paradaCentro, paradaVilaAssuncao, paradaCapuava);

        var linha1 = new Linha { Codigo = "437", Nome = "Terminal Santo André / Campus FSA" };
        var linha2 = new Linha { Codigo = "512", Nome = "Vila Assunção / Campus FSA" };
        var linha3 = new Linha { Codigo = "609", Nome = "Jardim Capuava / Terminal Santo André" };

        db.Linhas.AddRange(linha1, linha2, linha3);
        db.SaveChanges();

        db.RotaParadas.AddRange(
            new RotaParada { LinhaId = linha1.Id, ParadaId = paradaCentro.Id, Ordem = 1 },
            new RotaParada { LinhaId = linha1.Id, ParadaId = paradaCampus.Id, Ordem = 2 },
            new RotaParada { LinhaId = linha2.Id, ParadaId = paradaVilaAssuncao.Id, Ordem = 1 },
            new RotaParada { LinhaId = linha2.Id, ParadaId = paradaCampus.Id, Ordem = 2 },
            new RotaParada { LinhaId = linha3.Id, ParadaId = paradaCapuava.Id, Ordem = 1 },
            new RotaParada { LinhaId = linha3.Id, ParadaId = paradaCentro.Id, Ordem = 2 }
        );

        var admin = new Usuario
        {
            Nome = "Administrador SmartBus",
            Email = "admin@fsa.edu.br",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Papel = PapelUsuario.Admin
        };
        db.Usuarios.Add(admin);

        db.SaveChanges();
    }
}
