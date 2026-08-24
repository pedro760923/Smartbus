namespace SmartBus.Application.Previsao;

public interface IPrevisaoService
{
    Task<PrevisaoDto?> PreverAsync(int linhaId, DateTime? momento = null);
    Task<List<FaixaEsperaDto>> PreverTempoEsperaGeralAsync();
}
