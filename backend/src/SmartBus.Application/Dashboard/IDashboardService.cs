namespace SmartBus.Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardKpisDto> ObterKpisAsync();
}
