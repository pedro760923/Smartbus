using SmartBus.Domain.Enums;

namespace SmartBus.Domain.Entities;

/// <summary>
/// Registro de lotação reportado por um usuário (crowdsourcing).
/// Alimenta tanto o histórico exibido aos alunos quanto o motor
/// de previsão (ver PrevisaoService, camada Application).
/// </summary>
public class Reporte
{
    public int Id { get; set; }

    public int LinhaId { get; set; }
    public Linha Linha { get; set; } = null!;

    public int? ParadaId { get; set; }
    public Parada? Parada { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public NivelLotacao NivelLotacao { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Marcado como falso pelo ValidacaoService quando o reporte é
    /// considerado inconsistente (ex.: muito discrepante dos demais
    /// reportes recentes para a mesma linha).
    /// </summary>
    public bool Valido { get; set; } = true;
}
