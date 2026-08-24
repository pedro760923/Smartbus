namespace SmartBus.Application.Paradas;

public interface IParadaService
{
    Task<List<ParadaDto>> ListarAsync();
    Task<List<ParadaDto>> ProximasAsync(double latitude, double longitude, double raioMetros);
}
