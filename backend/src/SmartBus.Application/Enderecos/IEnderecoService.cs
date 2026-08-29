namespace SmartBus.Application.Enderecos;

public interface IEnderecoService
{
    Task<List<EnderecoSugestaoDto>> BuscarAsync(string? termo, int usuarioId);
    Task<HistoricoBuscaDto> RegistrarEscolhaAsync(NovoHistoricoRequest request, int usuarioId);
    Task<List<HistoricoBuscaDto>> HistoricoRecenteAsync(int usuarioId);
}
