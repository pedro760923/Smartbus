using SmartBus.Application.Abstractions;

namespace SmartBus.IntegrationTests.Infra;

/// <summary>
/// Dublê de IChatIaService para os testes de integração não dependerem
/// de rede/Gemini nem consumirem cota gratuita da API.
/// </summary>
public class FakeChatIaService : IChatIaService
{
    public string RespostaFixa { get; set; } = "Resposta de teste.";

    public Task<string> ConversarAsync(string instrucaoSistema, List<MensagemConversa> historico, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(RespostaFixa);
    }
}
