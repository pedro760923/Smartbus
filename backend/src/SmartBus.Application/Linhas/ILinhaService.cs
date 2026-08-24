namespace SmartBus.Application.Linhas;

public interface ILinhaService
{
    Task<List<LinhaDto>> ListarAsync(string? termo);
    Task<LinhaDto?> ObterPorIdAsync(int id);
}
