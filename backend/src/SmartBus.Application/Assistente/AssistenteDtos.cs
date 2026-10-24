using System.ComponentModel.DataAnnotations;

namespace SmartBus.Application.Assistente;

public record MensagemHistoricoDto(string Papel, string Texto);

public record PerguntaChatRequest([Required] string Mensagem, List<MensagemHistoricoDto>? Historico);

public record RespostaChatDto(string Resposta);
