namespace SmartBus.Application.Paradas;

public record ParadaDto(int Id, string Nome, double Latitude, double Longitude, double? DistanciaMetros = null);
