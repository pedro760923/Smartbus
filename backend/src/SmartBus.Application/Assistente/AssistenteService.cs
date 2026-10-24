using SmartBus.Application.Abstractions;
using SmartBus.Application.Linhas;

namespace SmartBus.Application.Assistente;

public class AssistenteService : IAssistenteService
{
    private const int LimiteHistorico = 10;

    private readonly IChatIaService _chatIa;
    private readonly ILinhaService _linhaService;

    public AssistenteService(IChatIaService chatIa, ILinhaService linhaService)
    {
        _chatIa = chatIa;
        _linhaService = linhaService;
    }

    public async Task<string> ResponderAsync(string mensagem, List<MensagemHistoricoDto>? historico, int usuarioId)
    {
        var instrucaoSistema = await MontarInstrucaoSistemaAsync();

        var conversa = (historico ?? new List<MensagemHistoricoDto>())
            .TakeLast(LimiteHistorico)
            .Select(m => new MensagemConversa(m.Papel, m.Texto))
            .Append(new MensagemConversa("usuario", mensagem))
            .ToList();

        try
        {
            return await _chatIa.ConversarAsync(instrucaoSistema, conversa);
        }
        catch
        {
            // Falha do provedor de IA (rede, chave inválida, modelo fora do
            // ar) não pode virar erro 500 pro usuário — melhor uma resposta
            // clara pedindo pra tentar de novo.
            return "Desculpa, não consegui falar com o assistente agora. Tenta de novo em alguns instantes.";
        }
    }

    private async Task<string> MontarInstrucaoSistemaAsync()
    {
        var linhas = await _linhaService.ListarAsync(null);
        var dadosAoVivo = linhas.Count == 0
            ? "Nenhuma linha cadastrada no momento."
            : string.Join("\n", linhas.Select(FormatarLinha));

        return $"""
            Você é o assistente virtual do SmartBus, app de transporte da Fundação Santo André (Santo André, SP). Responda sempre em português do Brasil, de forma curta e direta. Não use markdown (sem **negrito**, sem listas com "-" ou "*", sem títulos com #) — a resposta aparece como texto simples numa bolha de chat, então escreva em frases corridas.

            Responda APENAS perguntas sobre ônibus, linhas, horários, lotação, rotas e paradas relacionadas ao SmartBus e à EMTU de Santo André. Se perguntarem algo fora desse assunto, recuse educadamente e explique que só ajuda com informações de ônibus.

            Nunca invente horários, preços ou dados que não estejam no contexto abaixo. Se não souber algo, diga que não tem essa informação e sugira consultar o site oficial da EMTU (emtu.sp.gov.br).

            === DADOS AO VIVO (lotação atual, calculada pela maioria dos reportes de usuários do SmartBus nos últimos 30 minutos) ===
            {dadosAoVivo}

            === HORÁRIOS OFICIAIS EMTU — linhas que ainda não têm reportes ao vivo no app ===
            {ConhecimentoLinhas.Linha069}

            {ConhecimentoLinhas.Linha323}

            Observação sobre as linhas 069 e 323: segundo a equipe do SmartBus, essas linhas costumam ficar mais cheias no período noturno. Mencione isso só quando perguntarem sobre lotação dessas duas linhas especificamente, deixando claro que é uma estimativa baseada em observação da equipe, não um dado medido em tempo real como o das linhas 437/512/609.
            """;
    }

    private static string FormatarLinha(LinhaDto linha)
    {
        if (linha.TotalVotos == 0)
        {
            return $"- Linha {linha.Codigo} ({linha.Nome}): sem reportes recentes de lotação.";
        }

        var distribuicao = string.Join(", ", linha.DistribuicaoLotacao.Select(v => $"{v.Quantidade}x {v.Nivel}"));
        return $"- Linha {linha.Codigo} ({linha.Nome}): lotação atual estimada = {linha.NivelLotacaoAtual} (com base em {linha.TotalVotos} reportes nos últimos 30min: {distribuicao}).";
    }
}
