using System.ComponentModel.DataAnnotations;
using SmartBus.Domain.Enums;

namespace SmartBus.Application.Reportes;

public record NovoReporteRequest(
    [Required] int LinhaId,
    int? ParadaId,
    [Required] NivelLotacao NivelLotacao,
    double? Latitude,
    double? Longitude
);

public record ReporteDto(
    int Id,
    int LinhaId,
    string LinhaNome,
    int? ParadaId,
    NivelLotacao NivelLotacao,
    DateTime CriadoEm,
    bool Valido
);
