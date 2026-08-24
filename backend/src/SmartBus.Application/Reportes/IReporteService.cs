namespace SmartBus.Application.Reportes;

public interface IReporteService
{
    /// <summary>Retorna null quando a linha informada não existe.</summary>
    Task<ReporteDto?> EnviarAsync(NovoReporteRequest request, int usuarioId);
    Task<List<ReporteDto>> RecentesPorLinhaAsync(int linhaId);
}
