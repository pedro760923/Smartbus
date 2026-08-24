using System.ComponentModel.DataAnnotations;

namespace SmartBus.Application.Auth;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Senha
);

public record RegistroRequest(
    [Required, MinLength(2)] string Nome,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Senha
);

public record UsuarioDto(int Id, string Nome, string Email, string Papel);

public record AuthResponse(string Token, UsuarioDto Usuario, DateTime ExpiraEm);
