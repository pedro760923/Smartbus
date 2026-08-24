using SmartBus.Domain.Enums;

namespace SmartBus.Application.Linhas;

public record LinhaDto(int Id, string Codigo, string Nome, string? Descricao, NivelLotacao? NivelLotacaoAtual);
