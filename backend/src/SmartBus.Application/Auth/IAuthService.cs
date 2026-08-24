namespace SmartBus.Application.Auth;

public interface IAuthService
{
    /// <summary>Retorna null quando já existe uma conta com o e-mail informado.</summary>
    Task<AuthResponse?> RegistrarAsync(RegistroRequest request);

    /// <summary>Retorna null quando as credenciais são inválidas.</summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
