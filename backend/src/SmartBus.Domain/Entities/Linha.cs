namespace SmartBus.Domain.Entities;

public class Linha
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public ICollection<RotaParada> RotaParadas { get; set; } = new List<RotaParada>();
    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
}
