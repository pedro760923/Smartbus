using SmartBus.Application.Previsao;

namespace SmartBus.Application.Dashboard;

public record LinhaSuperlotacaoDto(string LinhaNome, double PercentualSuperlotado);

public record PontoCalorDto(int ParadaId, string ParadaNome, double Latitude, double Longitude, double Intensidade);

public record DashboardKpisDto(
    int TotalReportesHoje,
    int TotalLinhasMonitoradas,
    List<LinhaSuperlotacaoDto> LinhasComMaiorSuperlotacao,
    List<PontoCalorDto> MapaDeCalor,
    List<FaixaEsperaDto> PrevisaoTempoEspera
);
