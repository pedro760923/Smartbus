using SmartBus.Domain.Enums;

namespace SmartBus.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PapelUsuario Papel { get; set; } = PapelUsuario.Aluno;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
}
