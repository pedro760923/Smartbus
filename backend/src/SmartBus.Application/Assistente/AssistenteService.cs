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

            Responda APENAS perguntas sobre ônibus, linhas, horários, lotação, rotas e paradas relacionadas ao SmartBus, à EMTU (linhas intermunicipais 069 e 323) e à SATRANS (linhas municipais de Santo André B49 e B63). Se perguntarem algo fora desse assunto, recuse educadamente e explique que só ajuda com informações de ônibus.

            Nunca invente horários, preços ou dados que não estejam no contexto abaixo. Se não souber algo, diga que não tem essa informação e sugira consultar o site oficial da EMTU (emtu.sp.gov.br) ou da SATRANS (satrans.santoandre.sp.gov.br), conforme a linha.

            === DADOS AO VIVO (lotação atual das linhas do SmartBus, calculada pela maioria dos reportes de usuários nos últimos 30 minutos) ===
            {dadosAoVivo}

            === HORÁRIOS, TARIFAS E ITINERÁRIO OFICIAIS (complementam os dados ao vivo acima — use isso pra perguntas de horário/tarifa/rota, e os dados ao vivo pra perguntas de lotação agora) ===
            {ConhecimentoLinhas.Linha069}

            {ConhecimentoLinhas.Linha323}

            {ConhecimentoLinhas.LinhaB49}

            {ConhecimentoLinhas.LinhaB63}

            Observação geral sobre lotação: quando houver dado ao vivo (seção DADOS AO VIVO) pra uma linha, use sempre esse dado real como resposta principal. Quando não houver reportes recentes pra uma linha, você pode mencionar que, segundo a equipe do SmartBus, a linha B63 costuma ser historicamente a mais cheia da rede, especialmente à noite — mas deixe claro que isso é uma estimativa da equipe, não um dado medido em tempo real.
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
