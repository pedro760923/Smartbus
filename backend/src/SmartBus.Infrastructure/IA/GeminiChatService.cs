using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SmartBus.Application.Abstractions;

namespace SmartBus.Infrastructure.IA;

/// <summary>
/// Adaptador para a Interactions API do Gemini (Google AI Studio) —
/// substituiu a antiga generateContent; o Google não mantém mais essa
/// documentada como caminho principal. Endpoint: POST /v1beta/interactions,
/// autenticação via header x-goog-api-key (configurado no HttpClient
/// tipado, ver SmartBus.Infrastructure.DependencyInjection).
/// </summary>
public class GeminiChatService : IChatIaService
{
    private static readonly JsonSerializerOptions JsonOpcoes = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly HttpClient _http;
    private readonly string _modelo;

    public GeminiChatService(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _modelo = configuration["Gemini:Modelo"] ?? "gemini-3.6-flash";
    }

    public async Task<string> ConversarAsync(string instrucaoSistema, List<MensagemConversa> historico, CancellationToken cancellationToken = default)
    {
        var entrada = historico
            .Select(m => new GeminiInputStep(
                m.Papel == "assistente" ? "model_output" : "user_input",
                new List<GeminiContentPart> { new("text", m.Texto) }))
            .ToList();

        var corpo = new GeminiRequest(_modelo, instrucaoSistema, entrada);

        using var resposta = await _http.PostAsJsonAsync("interactions", corpo, JsonOpcoes, cancellationToken);
        resposta.EnsureSuccessStatusCode();

        var resultado = await resposta.Content.ReadFromJsonAsync<GeminiInteractionResponse>(JsonOpcoes, cancellationToken);

        if (resultado is null || resultado.Status != "completed")
        {
            throw new InvalidOperationException($"A interação com o Gemini não foi concluída (status: {resultado?.Status ?? "desconhecido"}).");
        }

        var texto = (resultado.Steps ?? new List<GeminiStep>())
            .Where(s => s.Type == "model_output")
            .SelectMany(s => s.Content ?? new List<GeminiContentPart>())
            .Where(c => c.Type == "text" && c.Text is not null)
            .Select(c => c.Text!);

        return string.Join("\n", texto);
    }

    private record GeminiRequest(string Model, string SystemInstruction, List<GeminiInputStep> Input);
    private record GeminiInputStep(string Type, List<GeminiContentPart> Content);
    private record GeminiContentPart(string Type, string? Text);
    private record GeminiInteractionResponse(string? Status, List<GeminiStep>? Steps);
    private record GeminiStep(string Type, List<GeminiContentPart>? Content);
}
