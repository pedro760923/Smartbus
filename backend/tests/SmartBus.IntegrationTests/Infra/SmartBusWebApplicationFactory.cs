using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SmartBusDbContext>));
            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            _connection.Open();

            services.AddDbContext<SmartBusDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
    }
}
