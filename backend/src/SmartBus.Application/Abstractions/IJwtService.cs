using SmartBus.Domain.Entities;

namespace SmartBus.Application.Abstractions;

public interface IJwtService
{
    (string token, DateTime expiraEm) GerarToken(Usuario usuario);
}
