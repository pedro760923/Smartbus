using Microsoft.EntityFrameworkCore;
using SmartBus.Domain.Entities;
using SmartBus.Domain.Enums;

namespace SmartBus.Infrastructure.Persistence;

/// <summary>
/// Popula o banco com as linhas reais de Santo André que o SmartBus
/// acompanha: 069 e 323 (EMTU, intermunicipais) e B49/B63 (SATRANS,
/// municipais). Coordenadas das paradas geocodificadas via Nominatim a
/// partir dos itinerários oficiais (emtu.sp.gov.br / satrans.santoandre.sp.gov.br).
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

        // Compartilhada pelas linhas 069 e 323.
        var terminalSantoAndreLeste = new Parada { Nome = "Terminal Metropolitano Santo André Leste", Latitude = -23.6518122, Longitude = -46.5269666 };

        // Linha 069 — Santo André Leste ↔ São Paulo (Fábrica Trol), via SBC/Rudge Ramos.
        var rudgeRamos = new Parada { Nome = "Avenida Doutor Rudge Ramos (São Bernardo do Campo)", Latitude = -23.6458542, Longitude = -46.5810871 };
        var ruaTocantinia = new Parada { Nome = "Rua Tocantínia (São Paulo / Fábrica Trol)", Latitude = -23.6421405, Longitude = -46.5857516 };

        // Linha 323 — Santo André Leste ↔ São Paulo (Jardim Clímax), via SBC/Taboão.
        var avTaboao = new Parada { Nome = "Avenida do Taboão (São Bernardo do Campo)", Latitude = -23.6504139, Longitude = -46.5894286 };
        var jardimClimax = new Parada { Nome = "Jardim Clímax (São Paulo)", Latitude = -23.6402140, Longitude = -46.6087329 };

        // Compartilhada pelas linhas B49 e B63 (corredor Av. Prestes Maia / Fundação Santo André).
        var fundacaoSantoAndre = new Parada { Nome = "Fundação Santo André (Av. Prestes Maia)", Latitude = -23.6485993, Longitude = -46.5463999 };

        // Linha B49 — Vila Palmares ↔ Centro (SATRANS).
        var vilaPalmares = new Parada { Nome = "Vila Palmares (Rua Hermínia Lopes Lobo)", Latitude = -23.6485712, Longitude = -46.5575320 };
        var rodoviariaSantoAndre = new Parada { Nome = "Rodoviária de Santo André", Latitude = -23.6389687, Longitude = -46.5380016 };
        var centroSantoAndre = new Parada { Nome = "Centro (Rua Luís Pinto Fláquer)", Latitude = -23.6573008, Longitude = -46.5249055 };

        // Linha B63 — Jardim Alvorada ↔ Vila Palmares (SATRANS).
        var jardimAlvorada = new Parada { Nome = "Jardim Alvorada (Rua Prof. Antônio Seixas Leite Ribeiro)", Latitude = -23.6978960, Longitude = -46.5248877 };
        var avIndustrial = new Parada { Nome = "Avenida Industrial", Latitude = -23.6437291, Longitude = -46.5364057 };
        var pracaAurea = new Parada { Nome = "Praça Áurea (Vila Palmares)", Latitude = -23.6519933, Longitude = -46.5601983 };

        db.Paradas.AddRange(
            terminalSantoAndreLeste, rudgeRamos, ruaTocantinia, avTaboao, jardimClimax,
            fundacaoSantoAndre, vilaPalmares, rodoviariaSantoAndre, centroSantoAndre,
            jardimAlvorada, avIndustrial, pracaAurea
        );

        var linha069 = new Linha { Codigo = "069", Nome = "Santo André Leste / São Paulo (Fábrica Trol)", Descricao = "Via São Bernardo do Campo (Rudge Ramos) — EMTU, operadora NextMobilidade" };
        var linha323 = new Linha { Codigo = "323", Nome = "Santo André Leste / São Paulo (Jardim Clímax)", Descricao = "Via São Bernardo do Campo (Taboão) — EMTU, operadora NextMobilidade" };
        var linhaB49 = new Linha { Codigo = "B49", Nome = "Vila Palmares / Centro", Descricao = "Linha municipal SATRANS Santo André" };
        var linhaB63 = new Linha { Codigo = "B63", Nome = "Jardim Alvorada / Vila Palmares", Descricao = "Linha municipal SATRANS Santo André" };

        db.Linhas.AddRange(linha069, linha323, linhaB49, linhaB63);
        db.SaveChanges();

        db.RotaParadas.AddRange(
            new RotaParada { LinhaId = linha069.Id, ParadaId = terminalSantoAndreLeste.Id, Ordem = 1 },
            new RotaParada { LinhaId = linha069.Id, ParadaId = rudgeRamos.Id, Ordem = 2 },
            new RotaParada { LinhaId = linha069.Id, ParadaId = ruaTocantinia.Id, Ordem = 3 },

            new RotaParada { LinhaId = linha323.Id, ParadaId = terminalSantoAndreLeste.Id, Ordem = 1 },
            new RotaParada { LinhaId = linha323.Id, ParadaId = avTaboao.Id, Ordem = 2 },
            new RotaParada { LinhaId = linha323.Id, ParadaId = jardimClimax.Id, Ordem = 3 },

            new RotaParada { LinhaId = linhaB49.Id, ParadaId = vilaPalmares.Id, Ordem = 1 },
            new RotaParada { LinhaId = linhaB49.Id, ParadaId = fundacaoSantoAndre.Id, Ordem = 2 },
            new RotaParada { LinhaId = linhaB49.Id, ParadaId = rodoviariaSantoAndre.Id, Ordem = 3 },
            new RotaParada { LinhaId = linhaB49.Id, ParadaId = centroSantoAndre.Id, Ordem = 4 },

            new RotaParada { LinhaId = linhaB63.Id, ParadaId = jardimAlvorada.Id, Ordem = 1 },
            new RotaParada { LinhaId = linhaB63.Id, ParadaId = avIndustrial.Id, Ordem = 2 },
            new RotaParada { LinhaId = linhaB63.Id, ParadaId = fundacaoSantoAndre.Id, Ordem = 3 },
            new RotaParada { LinhaId = linhaB63.Id, ParadaId = pracaAurea.Id, Ordem = 4 }
        );

        // Checado separadamente da guarda de Linhas no topo: o admin pode já
        // existir mesmo que Linhas esteja vazia (ex.: alguém limpou só as
        // linhas/paradas pra reseedar com dados novos, sem recriar o banco
        // inteiro) — inserir de novo aqui quebraria por e-mail duplicado.
        if (!db.Usuarios.Any(u => u.Email == "admin@fsa.edu.br"))
        {
            var admin = new Usuario
            {
                Nome = "Administrador SmartBus",
                Email = "admin@fsa.edu.br",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Papel = PapelUsuario.Admin
            };
            db.Usuarios.Add(admin);
        }

        db.SaveChanges();
    }
}
