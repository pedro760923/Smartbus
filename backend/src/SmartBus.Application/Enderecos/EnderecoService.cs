using Microsoft.EntityFrameworkCore;
using SmartBus.Application.Abstractions;
using SmartBus.Domain.Entities;

namespace SmartBus.Application.Enderecos;

public class EnderecoService : IEnderecoService
{
    private const int LimiteHistoricoRecente = 5;
    private const int LimiteHistoricoNaBusca = 3;
    private const int LimiteGeocodificacaoNaBusca = 5;
    private const int TamanhoMinimoTermo = 3;

    private readonly IApplicationDbContext _db;
    private readonly IGeocodingService _geocoding;

    public EnderecoService(IApplicationDbContext db, IGeocodingService geocoding)
    {
        _db = db;
        _geocoding = geocoding;
    }

    public async Task<List<EnderecoSugestaoDto>> BuscarAsync(string? termo, int usuarioId)
    {
        var termoTratado = termo?.Trim() ?? string.Empty;

        if (termoTratado.Length == 0)
        {
            var recentes = await UltimasBuscasDistintasAsync(usuarioId, LimiteHistoricoRecente);
            return recentes.Select(ParaSugestaoDoHistorico).ToList();
        }

        if (termoTratado.Length < TamanhoMinimoTermo)
        {
            return new List<EnderecoSugestaoDto>();
        }

        var doHistorico = await _db.HistoricosBusca
            .Where(h => h.UsuarioId == usuarioId && (h.Endereco.Contains(termoTratado) || h.Termo.Contains(termoTratado)))
            .OrderByDescending(h => h.CriadoEm)
            .ToListAsync();

        var doHistoricoDistinto = doHistorico
            .GroupBy(h => h.Endereco, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .Take(LimiteHistoricoNaBusca)
            .ToList();

        var enderecosJaSugeridos = doHistoricoDistinto
            .Select(h => h.Endereco)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var geocodificados = await BuscarGeocodificacaoComFallbackAsync(termoTratado);

        var sugestoes = doHistoricoDistinto
            .Select(ParaSugestaoDoHistorico)
            .Concat(geocodificados
                .Where(g => !enderecosJaSugeridos.Contains(g.Endereco))
                .Take(LimiteGeocodificacaoNaBusca)
                .Select(g => new EnderecoSugestaoDto(g.Endereco, g.Latitude, g.Longitude, DoHistorico: false)))
            .ToList();

        return sugestoes;
    }

    public async Task<HistoricoBuscaDto> RegistrarEscolhaAsync(NovoHistoricoRequest request, int usuarioId)
    {
        var existente = await _db.HistoricosBusca
            .FirstOrDefaultAsync(h => h.UsuarioId == usuarioId && h.Endereco == request.Endereco);

        if (existente is not null)
        {
            existente.Termo = request.Termo;
            existente.Latitude = request.Latitude;
            existente.Longitude = request.Longitude;
            existente.CriadoEm = DateTime.UtcNow;
        }
        else
        {
            existente = new HistoricoBusca
            {
                UsuarioId = usuarioId,
                Termo = request.Termo,
                Endereco = request.Endereco,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };
            _db.HistoricosBusca.Add(existente);
        }

        await _db.SaveChangesAsync();

        return new HistoricoBuscaDto(existente.Id, existente.Termo, existente.Endereco, existente.Latitude, existente.Longitude, existente.CriadoEm);
    }

    public async Task<List<HistoricoBuscaDto>> HistoricoRecenteAsync(int usuarioId)
    {
        var recentes = await UltimasBuscasDistintasAsync(usuarioId, LimiteHistoricoRecente);
        return recentes
            .Select(h => new HistoricoBuscaDto(h.Id, h.Termo, h.Endereco, h.Latitude, h.Longitude, h.CriadoEm))
            .ToList();
    }

    private async Task<List<HistoricoBusca>> UltimasBuscasDistintasAsync(int usuarioId, int limite)
    {
        var historico = await _db.HistoricosBusca
            .Where(h => h.UsuarioId == usuarioId)
            .OrderByDescending(h => h.CriadoEm)
            .Take(50)
            .ToListAsync();

        return historico
            .GroupBy(h => h.Endereco, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .Take(limite)
            .ToList();
    }

    /// <summary>
    /// Uma falha do provedor externo de geocodificação (rede fora do ar,
    /// rate limit) não pode derrubar a busca inteira — nesse caso o
    /// usuário ainda vê as sugestões vindas do próprio histórico.
    /// </summary>
    private async Task<List<EnderecoGeocodificado>> BuscarGeocodificacaoComFallbackAsync(string termo)
    {
        try
        {
            return await _geocoding.BuscarEnderecosAsync(termo);
        }
        catch
        {
            return new List<EnderecoGeocodificado>();
        }
    }

    private static EnderecoSugestaoDto ParaSugestaoDoHistorico(HistoricoBusca h) =>
        new(h.Endereco, h.Latitude, h.Longitude, DoHistorico: true);
}
