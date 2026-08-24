using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartBus.Application.Abstractions;
using SmartBus.Application.Common;
using SmartBus.Infrastructure.Persistence;
using SmartBus.Infrastructure.Security;

namespace SmartBus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' não configurada em appsettings.json.");

        services.AddDbContext<SmartBusDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // A Application enxerga só a abstração; o DbContext concreto do
        // EF Core é resolvido aqui e injetado como IApplicationDbContext.
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<SmartBusDbContext>());

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
