namespace SmartBus.Application.Rotas;

public interface IRotaService
{
    Task<RotaDto?> ObterPorLinhaAsync(int linhaId);
}
