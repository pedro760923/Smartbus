using Microsoft.EntityFrameworkCore;
using SmartBus.Infrastructure.Persistence;

namespace SmartBus.UnitTests.TestUtils;

/// <summary>
/// Cria um SmartBusDbContext (que implementa IApplicationDbContext) sobre
/// o provider InMemory do EF Core, com um nome de banco único por
/// instância — garante isolamento total entre testes, mesmo em paralelo.
/// </summary>
public static class InMemoryDbFactory
{
    public static SmartBusDbContext Criar()
    {
        var options = new DbContextOptionsBuilder<SmartBusDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SmartBusDbContext(options);
    }
}
