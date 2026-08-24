using SmartBus.Domain.Entities;

namespace SmartBus.Application.Reportes;

/// <summary>
/// Decide se um novo reporte deve ser considerado válido, com base na
/// consistência em relação aos reportes recentes da mesma linha
/// (defesa simples contra ruído/abuso do crowdsourcing).
/// </summary>
public interface IValidacaoService
{
    Task<bool> ValidarAsync(Reporte reporte);
}
