using SmartBus.Domain.Enums;

namespace SmartBus.Application.Previsao;

public record PrevisaoDto(
    int LinhaId,
    int DiaSemana,
    string FaixaHoraria,
    NivelLotacao NivelPrevisto,
    double Confiabilidade,
    int Amostras
);

public record FaixaEsperaDto(string FaixaHoraria, int MinutosEsperaEstimados);
