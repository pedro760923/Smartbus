namespace SmartBus.Application.Abstractions;

/// <summary>
/// Porta (Ports &amp; Adapters) para um provedor de LLM conversacional. A
/// implementação real (Gemini) fica em SmartBus.Infrastructure — esta
/// interface não conhece nada sobre lotação de linhas ou horários de
/// ônibus, isso é responsabilidade da camada Application (AssistenteService).
/// </summary>
public record MensagemConversa(string Papel, string Texto);

public interface IChatIaService
{
    /// <param name="instrucaoSistema">Persona/contexto fixo (quem é o assistente, o que ele sabe, regras).</param>
    /// <param name="historico">Turnos da conversa, em ordem, terminando na mensagem atual do usuário.</param>
    Task<string> ConversarAsync(string instrucaoSistema, List<MensagemConversa> historico, CancellationToken cancellationToken = default);
}
