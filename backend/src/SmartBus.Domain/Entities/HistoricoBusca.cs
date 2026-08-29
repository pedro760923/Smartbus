namespace SmartBus.Domain.Entities;

/// <summary>
/// Endereço que um usuário buscou e efetivamente escolheu (não cada
/// tecla digitada) — alimenta as sugestões de "buscas recentes" e o
/// pré-preenchimento de resultados de endereço antes da geocodificação
/// ao vivo responder.
/// </summary>
public class HistoricoBusca
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public string Termo { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
