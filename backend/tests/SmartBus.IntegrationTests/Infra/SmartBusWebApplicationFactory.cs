using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartBus.Application.Abstractions;
using SmartBus.Infrastructure.Persistence;

namespace SmartBus.IntegrationTests.Infra;

/// <summary>
/// Sobe a API inteira (pipeline HTTP real: roteamento, autenticação JWT,
/// autorização, model binding) trocando o SQLite em arquivo por um SQLite
/// "in-memory" isolado por instância de factory, para não colidir entre
/// classes de teste nem sujar o smartbus.db real do dev.
/// </summary>
public class SmartBusWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Desde o EF Core 8, chamar AddDbContext<T> mais de uma vez para o
            // mesmo T não substitui a configuração anterior — as duas ações de
            // configuração (MySQL da Program.cs + SQLite daqui) são combinadas,
            // e o EF reclama de dois provedores registrados. Por isso é preciso
            // remover também o IDbContextOptionsConfiguration<T>, não só o
            // DbContextOptions<T>.
            services.RemoveAll<DbContextOptions<SmartBusDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<SmartBusDbContext>>();

            _connection.Open();

            services.AddDbContext<SmartBusDbContext>(options =>
                options.UseSqlite(_connection));

            // Evita chamadas reais ao Nominatim durante os testes de
            // integração/CI — RemoveAll é necessário aqui pelo mesmo
            // motivo do DbContext acima (AddHttpClient também registra
            // via IHttpClientFactory, não é só um AddScoped simples).
            services.RemoveAll<IGeocodingService>();
            services.AddSingleton<IGeocodingService, FakeGeocodingService>();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
    }
}
