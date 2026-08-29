using SmartBus.Domain.Enums;

namespace SmartBus.Application.Linhas;

public record LinhaDto(
    int Id,
    string Codigo,
    string Nome,
    string? Descricao,
    NivelLotacao? NivelLotacaoAtual,
    int TotalVotos,
    List<VotoNivelDto> DistribuicaoLotacao
);

public record VotoNivelDto(NivelLotacao Nivel, int Quantidade, double Percentual);
