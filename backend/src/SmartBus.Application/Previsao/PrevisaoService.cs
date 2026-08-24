using Microsoft.EntityFrameworkCore;
using SmartBus.Application.Abstractions;
using SmartBus.Domain.Enums;

namespace SmartBus.Application.Previsao;

/// <summary>
/// Motor de previsão "leve": em vez de um modelo de ML pesado, calcula
/// a média histórica de lotação reportada para a mesma linha, dia da
/// semana e faixa horária (blocos de 2 horas), com fallback progressivo
/// para janelas mais amplas quando há poucos dados.
/// </summary>
public class PrevisaoService : IPrevisaoService
{
    private readonly IApplicationDbContext _db;

    public PrevisaoService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PrevisaoDto?> PreverAsync(int linhaId, DateTime? momento = null)
    {
        var linha = await _db.Linhas.FindAsync(linhaId);
        if (linha is null) return null;

        var referencia = momento ?? DateTime.UtcNow;
        var diaSemana = (int)referencia.DayOfWeek;
        var faixa = ObterFaixaHoraria(referencia.Hour);

        // 1ª tentativa: mesmo dia da semana + mesma faixa horária.
        var amostras = await _db.Reportes
            .Where(r => r.LinhaId == linhaId && r.Valido
                && r.CriadoEm.DayOfWeek == referencia.DayOfWeek
                && r.CriadoEm.Hour >= faixa.horaInicio && r.CriadoEm.Hour < faixa.horaFim)
            .Select(r => (int)r.NivelLotacao)
            .ToListAsync();

        var confiabilidade = 1.0;

        // Fallback: sem amostras suficientes na faixa exata, usa todo o histórico da linha.
        if (amostras.Count < 3)
        {
            amostras = await _db.Reportes
                .Where(r => r.LinhaId == linhaId && r.Valido)
                .Select(r => (int)r.NivelLotacao)
                .ToListAsync();
            confiabilidade = 0.4;
        }

        if (amostras.Count == 0)
        {
            return new PrevisaoDto(linhaId, diaSemana, faixa.rotulo, NivelLotacao.ComLugares, 0.1, 0);
        }

        var media = amostras.Average();
        var nivelPrevisto = (NivelLotacao)Math.Clamp((int)Math.Round(media), 0, 3);

        var confiabilidadeFinal = Math.Round(confiabilidade * Math.Min(1.0, amostras.Count / 10.0), 2);

        return new PrevisaoDto(linhaId, diaSemana, faixa.rotulo, nivelPrevisto, confiabilidadeFinal, amostras.Count);
    }

    public async Task<List<FaixaEsperaDto>> PreverTempoEsperaGeralAsync()
    {
        var faixas = new (int inicio, int fim, string rotulo)[]
        {
            (6, 9, "06h-09h"),
            (9, 12, "09h-12h"),
            (12, 14, "12h-14h"),
            (14, 18, "14h-18h"),
            (18, 21, "18h-21h"),
            (21, 24, "21h-00h")
        };

        var resultado = new List<FaixaEsperaDto>();

        foreach (var faixa in faixas)
        {
            var mediaLotacao = await _db.Reportes
                .Where(r => r.Valido && r.CriadoEm.Hour >= faixa.inicio && r.CriadoEm.Hour < faixa.fim)
                .Select(r => (int?)r.NivelLotacao)
                .AverageAsync() ?? (int)NivelLotacao.ComLugares;

            var minutosEstimados = 5 + (int)Math.Round(mediaLotacao * 4);

            resultado.Add(new FaixaEsperaDto(faixa.rotulo, minutosEstimados));
        }

        return resultado;
    }

    private static (int horaInicio, int horaFim, string rotulo) ObterFaixaHoraria(int hora)
    {
        var inicio = (hora / 2) * 2;
        var fim = inicio + 2;
        return (inicio, fim, $"{inicio:00}h-{fim:00}h");
    }
}
