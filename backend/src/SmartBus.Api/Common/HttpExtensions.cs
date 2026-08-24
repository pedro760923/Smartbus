using System.Security.Claims;

namespace SmartBus.Api.Common;

/// <summary>
/// Ponto único de verdade para as rotas HTTP da API.
///
/// Cada endpoint é uma constante nomeada "{Verbo}_{Ação}" (ex.:
/// Get_Listar, Post_Enviar) — o método HTTP fica explícito no próprio
/// nome, então quem lê o Controller sabe o verbo sem precisar abrir o
/// atributo [Http*] ao lado. Isso evita rota "mágica" duplicada como
/// string solta em vários lugares (um erro de digitação num controller
/// não quebra silenciosamente um teste de integração que também colou
/// a string à mão).
///
/// Obs.: atributos como [HttpGet(...)] exigem constantes em tempo de
/// compilação, então aqui usamos const strings, não métodos de extensão
/// de fato — os métodos de extensão reais (que dependem de valor em
/// tempo de execução, como o usuário autenticado) ficam no fim do
/// arquivo.
/// </summary>
public static class HttpExtensions
{
    public static class Auth
    {
        public const string Base = "api/auth";
        public const string Post_Registro = "registro";
        public const string Post_Login = "login";
    }

    public static class Linhas
    {
        public const string Base = "api/linhas";
        public const string Get_Listar = "";
        public const string Get_ObterPorId = "{id:int}";
    }

    public static class Paradas
    {
        public const string Base = "api/paradas";
        public const string Get_Listar = "";
        public const string Get_Proximas = "proximas";
    }

    public static class Rotas
    {
        public const string Base = "api/rotas";
        public const string Get_ObterPorLinha = "{linhaId:int}";
    }

    public static class Reportes
    {
        public const string Base = "api/reportes";
        public const string Post_Enviar = "";
        public const string Get_RecentesPorLinha = "linha/{linhaId:int}/recentes";
    }

    public static class Previsao
    {
        public const string Base = "api/previsao";
        public const string Get_ObterPorLinha = "linha/{linhaId:int}";
    }

    public static class Dashboard
    {
        public const string Base = "api/dashboard";
        public const string Get_Kpis = "kpis";
    }

    /// <summary>
    /// Extrai o id do usuário autenticado a partir do token JWT (claim
    /// "sub"/NameIdentifier). Centralizado aqui porque antes do refactor
    /// esse parsing estava duplicado/ad-hoc dentro do ReportesController.
    /// </summary>
    public static int ObterUsuarioIdAutenticado(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub");

        if (sub is null || !int.TryParse(sub, out var usuarioId))
        {
            throw new InvalidOperationException("Não foi possível extrair o id do usuário autenticado do token.");
        }

        return usuarioId;
    }
}
