using FluentAssertions;
using SmartBus.Application.Assistente;
using SmartBus.Application.Linhas;
using SmartBus.Domain.Entities;
using SmartBus.Domain.Enums;
using SmartBus.UnitTests.TestUtils;
using Xunit;

namespace SmartBus.UnitTests.Assistente;

public class AssistenteServiceTests
{
    [Fact]
    public async Task ResponderAsync_DeveIncluirLotacaoAoVivoNaInstrucaoDoSistema()
    {
        using var db = InMemoryDbFactory.Criar();
        var linha = new Linha { Codigo = "437", Nome = "Terminal Santo André" };
        var usuario = new Usuario { Nome = "Aluno", Email = "aluno@fsa.edu.br", SenhaHash = "x" };
        db.Linhas.Add(linha);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        db.Reportes.Add(new Reporte { LinhaId = linha.Id, UsuarioId = usuario.Id, NivelLotacao = NivelLotacao.Cheio, Valido = true, CriadoEm = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var chatIa = new FakeChatIaService();
        var service = new AssistenteService(chatIa, new LinhaService(db));

        await service.ResponderAsync("Como está a linha 437?", null, usuario.Id);

        chatIa.InstrucaoRecebida.Should().Contain("437").And.Contain("Cheio");
    }

    [Fact]
    public async Task ResponderAsync_DeveIncluirMensagemAtualComoUltimoTurnoDaConversa()
    {
        using var db = InMemoryDbFactory.Criar();
        var usuario = new Usuario { Nome = "Aluno", Email = "aluno@fsa.edu.br", SenhaHash = "x" };
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var chatIa = new FakeChatIaService();
        var service = new AssistenteService(chatIa, new LinhaService(db));

        var historico = new List<MensagemHistoricoDto> { new("usuario", "oi"), new("assistente", "olá, como posso ajudar?") };
        await service.ResponderAsync("quando passa a linha 069?", historico, usuario.Id);

        chatIa.HistoricoRecebido.Should().HaveCount(3);
        chatIa.HistoricoRecebido!.Last().Papel.Should().Be("usuario");
        chatIa.HistoricoRecebido!.Last().Texto.Should().Be("quando passa a linha 069?");
    }

    [Fact]
    public async Task ResponderAsync_FalhaNoProvedorDeIa_DeveRetornarMensagemAmigavelSemLancarExcecao()
    {
        using var db = InMemoryDbFactory.Criar();
        var usuario = new Usuario { Nome = "Aluno", Email = "aluno@fsa.edu.br", SenhaHash = "x" };
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var chatIa = new FakeChatIaService { DeveFalhar = true };
        var service = new AssistenteService(chatIa, new LinhaService(db));

        var resposta = await service.ResponderAsync("oi", null, usuario.Id);

        resposta.Should().NotBeNullOrWhiteSpace();
        resposta.Should().NotBe(chatIa.RespostaFixa);
    }
}
