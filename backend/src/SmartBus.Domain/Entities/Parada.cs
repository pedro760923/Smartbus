namespace SmartBus.Domain.Entities;

public class Parada
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public ICollection<RotaParada> RotaParadas { get; set; } = new List<RotaParada>();
    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
}
