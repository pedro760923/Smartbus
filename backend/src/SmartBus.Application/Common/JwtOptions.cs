namespace SmartBus.Application.Common;

public class JwtOptions
{
    public string ChaveSecreta { get; set; } = string.Empty;
    public string Emissor { get; set; } = "SmartBusApi";
    public string Audiencia { get; set; } = "SmartBusClientes";
    public int ExpiracaoMinutos { get; set; } = 480;
}
