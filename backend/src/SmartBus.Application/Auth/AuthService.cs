using Microsoft.EntityFrameworkCore;
using SmartBus.Application.Abstractions;
using SmartBus.Domain.Entities;
using SmartBus.Domain.Enums;

namespace SmartBus.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public AuthService(IApplicationDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse?> RegistrarAsync(RegistroRequest request)
    {
        var emailJaExiste = await _db.Usuarios.AnyAsync(u => u.Email == request.Email);
        if (emailJaExiste)
        {
            return null;
        }

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = _passwordHasher.Hash(request.Senha),
            Papel = PapelUsuario.Aluno
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return GerarResposta(usuario);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var usuario = await _db.Usuarios.SingleOrDefaultAsync(u => u.Email == request.Email);

        if (usuario is null || !_passwordHasher.Verify(request.Senha, usuario.SenhaHash))
        {
            return null;
        }

        return GerarResposta(usuario);
    }

    private AuthResponse GerarResposta(Usuario usuario)
    {
        var (token, expiraEm) = _jwtService.GerarToken(usuario);
        var dto = new UsuarioDto(usuario.Id, usuario.Nome, usuario.Email, usuario.Papel.ToString());
        return new AuthResponse(token, dto, expiraEm);
    }
}
