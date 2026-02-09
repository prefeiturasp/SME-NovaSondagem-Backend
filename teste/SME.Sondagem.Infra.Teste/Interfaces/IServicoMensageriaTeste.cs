using SME.Sondagem.Infra.Fila;
using SME.Sondagem.Infra.Interfaces;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Interfaces;

public class IServicoMensageriaTeste
{
    private class ServicoMensageriaMock : IServicoMensageria
    {
        public MensagemRabbit? UltimaMensagem { get; private set; }
        public string? UltimaRota { get; private set; }
        public string? UltimoExchange { get; private set; }
        public string? UltimaAcao { get; private set; }
        public bool Retorno { get; set; } = true;

        public Task<bool> Publicar(MensagemRabbit mensagemRabbit, string rota, string exchange, string nomeAcao)
        {
            UltimaMensagem = mensagemRabbit;
            UltimaRota = rota;
            UltimoExchange = exchange;
            UltimaAcao = nomeAcao;
            return Task.FromResult(Retorno);
        }
    }

    [Fact]
    public async Task Deve_Publicar_Mensagem_Com_Sucesso()
    {
        var mock = new ServicoMensageriaMock();
        var mensagem = new MensagemRabbit("teste", Guid.NewGuid());

        var resultado = await mock.Publicar(mensagem, "rota", "exchange", "acao");

        Assert.True(resultado);
        Assert.Equal(mensagem, mock.UltimaMensagem);
        Assert.Equal("rota", mock.UltimaRota);
        Assert.Equal("exchange", mock.UltimoExchange);
        Assert.Equal("acao", mock.UltimaAcao);
    }

    [Fact]
    public async Task Deve_RetornarFalse_QuandoConfigurado()
    {
        var mock = new ServicoMensageriaMock { Retorno = false };
        var mensagem = new MensagemRabbit("falha", Guid.NewGuid());

        var resultado = await mock.Publicar(mensagem, "rota2", "exchange2", "acao2");

        Assert.False(resultado);
        Assert.Equal(mensagem, mock.UltimaMensagem);
        Assert.Equal("rota2", mock.UltimaRota);
        Assert.Equal("exchange2", mock.UltimoExchange);
        Assert.Equal("acao2", mock.UltimaAcao);
    }
}
