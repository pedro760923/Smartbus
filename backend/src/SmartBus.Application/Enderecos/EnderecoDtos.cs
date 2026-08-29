using System.ComponentModel.DataAnnotations;

namespace SmartBus.Application.Enderecos;

public record EnderecoSugestaoDto(string Endereco, double Latitude, double Longitude, bool DoHistorico);

public record NovoHistoricoRequest(
    [Required] string Termo,
    [Required] string Endereco,
    [Required] double Latitude,
    [Required] double Longitude
);

public record HistoricoBuscaDto(int Id, string Termo, string Endereco, double Latitude, double Longitude, DateTime CriadoEm);
