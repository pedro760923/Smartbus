namespace SmartBus.Application.Rotas;

public record RotaParadaDto(int Id, string Nome, double Latitude, double Longitude, int Ordem);

public record RotaDto(int LinhaId, string LinhaCodigo, string LinhaNome, List<RotaParadaDto> Paradas);
