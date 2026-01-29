using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Interfaces;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Interfaces;

public class IServicoLogTeste
{
    private class ServicoLogMock : IServicoLog
    {
        public Exception? UltimaExcecao { get; private set; }
        public string? UltimaMensagem { get; private set; }
        public LogNivel? UltimoNivel { get; private set; }
        public string? UltimoErro { get; private set; }
        public string? UltimasObservacoes { get; private set; }
        public string? UltimoStackTrace { get; private set; }

        public void Registrar(Exception ex)
        {
            UltimaExcecao = ex;
        }

        public void Registrar(string mensagem, Exception ex)
        {
            UltimaMensagem = mensagem;
            UltimaExcecao = ex;
        }

        public void Registrar(LogNivel nivel, string erro, string observacoes, string stackTrace)
        {
            UltimoNivel = nivel;
            UltimoErro = erro;
            UltimasObservacoes = observacoes;
            UltimoStackTrace = stackTrace;
        }
    }

    [Fact]
    public void Deve_Registrar_Excecao()
    {
        var mock = new ServicoLogMock();
        var ex = new InvalidOperationException("erro");

        mock.Registrar(ex);

        Assert.Equal(ex, mock.UltimaExcecao);
        Assert.Null(mock.UltimaMensagem);
    }

    [Fact]
    public void Deve_Registrar_MensagemEExcecao()
    {
        var mock = new ServicoLogMock();
        var ex = new Exception("erro2");

        mock.Registrar("mensagem", ex);

        Assert.Equal("mensagem", mock.UltimaMensagem);
        Assert.Equal(ex, mock.UltimaExcecao);
    }

    [Fact]
    public void Deve_Registrar_ComNivelErroObservacoesStackTrace()
    {
        var mock = new ServicoLogMock();

        mock.Registrar(LogNivel.Informacao, "erro", "obs", "stack");

        Assert.Equal(LogNivel.Informacao, mock.UltimoNivel);
        Assert.Equal("erro", mock.UltimoErro);
        Assert.Equal("obs", mock.UltimasObservacoes);
        Assert.Equal("stack", mock.UltimoStackTrace);
    }
}
