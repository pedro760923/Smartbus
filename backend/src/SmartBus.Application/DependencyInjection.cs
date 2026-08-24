using Microsoft.Extensions.DependencyInjection;
using SmartBus.Application.Auth;
using SmartBus.Application.Dashboard;
using SmartBus.Application.Linhas;
using SmartBus.Application.Paradas;
using SmartBus.Application.Previsao;
using SmartBus.Application.Reportes;
using SmartBus.Application.Rotas;

namespace SmartBus.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra todos os casos de uso da camada Application. A API só
    /// precisa chamar builder.Services.AddApplication() — ela não conhece
    /// (nem precisa conhecer) a lista de serviços internos.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILinhaService, LinhaService>();
        services.AddScoped<IParadaService, ParadaService>();
        services.AddScoped<IRotaService, RotaService>();
        services.AddScoped<IValidacaoService, ValidacaoService>();
        services.AddScoped<IReporteService, ReporteService>();
        services.AddScoped<IPrevisaoService, PrevisaoService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
