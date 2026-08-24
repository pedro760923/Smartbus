namespace SmartBus.Domain.Entities;

/// <summary>
/// Tabela associativa entre Linha e Parada: cada linha possui uma
/// sequência ordenada de paradas.
/// </summary>
public class RotaParada
{
    public int Id { get; set; }

    public int LinhaId { get; set; }
    public Linha Linha { get; set; } = null!;

    public int ParadaId { get; set; }
    public Parada Parada { get; set; } = null!;

    public int Ordem { get; set; }
}
