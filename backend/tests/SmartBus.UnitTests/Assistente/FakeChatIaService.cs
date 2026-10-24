using SmartBus.Application.Abstractions;

namespace SmartBus.UnitTests.Assistente;

public class FakeChatIaService : IChatIaService
{
    public string? InstrucaoRecebida { get; private set; }
    public List<MensagemConversa>? HistoricoRecebido { get; private set; }
    public bool DeveFalhar { get; set; }
    public string RespostaFixa { get; set; } = "Resposta de teste.";

    public Task<string> ConversarAsync(string instrucaoSistema, List<MensagemConversa> historico, CancellationToken cancellationToken = default)
    {
        if (DeveFalhar)
        {
            throw new HttpRequestException("Falha simulada de rede.");
        }

        InstrucaoRecebida = instrucaoSistema;
        HistoricoRecebido = historico;
        return Task.FromResult(RespostaFixa);
    }
}
