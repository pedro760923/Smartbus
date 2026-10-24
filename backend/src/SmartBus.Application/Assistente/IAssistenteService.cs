namespace SmartBus.Application.Assistente;

public interface IAssistenteService
{
    Task<string> ResponderAsync(string mensagem, List<MensagemHistoricoDto>? historico, int usuarioId);
}
