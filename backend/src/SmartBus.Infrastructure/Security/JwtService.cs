using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartBus.Application.Abstractions;
using SmartBus.Application.Common;
using SmartBus.Domain.Entities;

namespace SmartBus.Infrastructure.Security;

public class JwtService : IJwtService
{
    private readonly JwtOptions _opcoes;

    public JwtService(IOptions<JwtOptions> opcoes)
    {
        _opcoes = opcoes.Value;
    }

    public (string token, DateTime expiraEm) GerarToken(Usuario usuario)
    {
        var expiraEm = DateTime.UtcNow.AddMinutes(_opcoes.ExpiracaoMinutos);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Papel.ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opcoes.ChaveSecreta));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opcoes.Emissor,
            audience: _opcoes.Audiencia,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}
