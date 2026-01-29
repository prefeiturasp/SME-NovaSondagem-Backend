using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infra.Services;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Interfaces;

public class IServicoTelemetriaTeste
{
    private class ServicoTelemetriaMock : IServicoTelemetria
    {
        public string? UltimaAcaoNome { get; private set; }
        public string? UltimaTelemetriaNome { get; private set; }
        public string? UltimaTelemetriaValor { get; private set; }
        public string? UltimosParametros { get; private set; }
        public object? UltimoRetorno { get; set; }
        public ServicoTelemetria.ServicoTelemetriaTransacao? UltimaTransacao { get; private set; }
        public Exception? UltimaExcecao { get; private set; }
        public bool FinalizouTransacao { get; private set; }

        public Task<dynamic> RegistrarComRetornoAsync<T>(Func<Task<object>> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros)
        {
            UltimaAcaoNome = acaoNome;
            UltimaTelemetriaNome = telemetriaNome;
            UltimaTelemetriaValor = telemetriaValor;
            UltimosParametros = parametros;
            UltimoRetorno = "retornoAsyncComParametros";
            return Task.FromResult((dynamic)UltimoRetorno!);
        }

        public Task<dynamic> RegistrarComRetornoAsync<T>(Func<Task<object>> acao, string acaoNome, string telemetriaNome, string telemetriaValor)
        {
            UltimaAcaoNome = acaoNome;
            UltimaTelemetriaNome = telemetriaNome;
            UltimaTelemetriaValor = telemetriaValor;
            UltimoRetorno = "retornoAsync";
            return Task.FromResult((dynamic)UltimoRetorno!);
        }

        public dynamic RegistrarComRetorno<T>(Func<object> acao, string acaoNome, string telemetriaNome, string telemetriaValor)
        {
            UltimaAcaoNome = acaoNome;
            UltimaTelemetriaNome = telemetriaNome;
            UltimaTelemetriaValor = telemetriaValor;
            UltimoRetorno = "retornoSync";
            return UltimoRetorno!;
        }

        public void Registrar(Action acao, string acaoNome, string telemetriaNome, string telemetriaValor)
        {
            UltimaAcaoNome = acaoNome;
            UltimaTelemetriaNome = telemetriaNome;
            UltimaTelemetriaValor = telemetriaValor;
            acao();
        }

        public Task RegistrarAsync(Func<Task> acao, string acaoNome, string telemetriaNome, string telemetriaValor)
        {
            UltimaAcaoNome = acaoNome;
            UltimaTelemetriaNome = telemetriaNome;
            UltimaTelemetriaValor = telemetriaValor;
            return acao();
        }

        public ServicoTelemetria.ServicoTelemetriaTransacao IniciarTransacao(string nome)
        {
            UltimaTransacao = new ServicoTelemetria.ServicoTelemetriaTransacao(nome);
            return UltimaTransacao;
        }

        public void FinalizarTransacao(ServicoTelemetria.ServicoTelemetriaTransacao servicoTelemetriaTransacao)
        {
            FinalizouTransacao = true;
            UltimaTransacao = servicoTelemetriaTransacao;
        }

        public void RegistrarExcecao(ServicoTelemetria.ServicoTelemetriaTransacao servicoTelemetriaTransacao, Exception ex)
        {
            UltimaTransacao = servicoTelemetriaTransacao;
            UltimaExcecao = ex;
        }
    }
        
    [Fact]
    public void Deve_RegistrarComRetorno_Sincrono()
    {
        var mock = new ServicoTelemetriaMock();
        var result = mock.RegistrarComRetorno<object>(() => "ok", "acao3", "telemetria3", "valor3");

        Assert.Equal("acao3", mock.UltimaAcaoNome);
        Assert.Equal("telemetria3", mock.UltimaTelemetriaNome);
        Assert.Equal("valor3", mock.UltimaTelemetriaValor);
        Assert.Equal("retornoSync", result);
    }

    [Fact]
    public void Deve_Registrar_Acao()
    {
        var mock = new ServicoTelemetriaMock();
        bool executou = false;
        mock.Registrar(() => executou = true, "acao4", "telemetria4", "valor4");

        Assert.Equal("acao4", mock.UltimaAcaoNome);
        Assert.Equal("telemetria4", mock.UltimaTelemetriaNome);
        Assert.Equal("valor4", mock.UltimaTelemetriaValor);
        Assert.True(executou);
    }

    [Fact]
    public async Task Deve_RegistrarAsync_Acao()
    {
        var mock = new ServicoTelemetriaMock();
        bool executou = false;
        await mock.RegistrarAsync(async () => { executou = true; await Task.CompletedTask; }, "acao5", "telemetria5", "valor5");

        Assert.Equal("acao5", mock.UltimaAcaoNome);
        Assert.Equal("telemetria5", mock.UltimaTelemetriaNome);
        Assert.Equal("valor5", mock.UltimaTelemetriaValor);
        Assert.True(executou);
    }

    [Fact]
    public void Deve_IniciarTransacao()
    {
        var mock = new ServicoTelemetriaMock();
        var transacao = mock.IniciarTransacao("rota");

        Assert.NotNull(transacao);
        Assert.Equal(transacao, mock.UltimaTransacao);
    }

    [Fact]
    public void Deve_FinalizarTransacao()
    {
        var mock = new ServicoTelemetriaMock();
        var transacao = new ServicoTelemetria.ServicoTelemetriaTransacao("rota2");

        mock.FinalizarTransacao(transacao);

        Assert.True(mock.FinalizouTransacao);
        Assert.Equal(transacao, mock.UltimaTransacao);
    }

    [Fact]
    public void Deve_RegistrarExcecao()
    {
        var mock = new ServicoTelemetriaMock();
        var transacao = new ServicoTelemetria.ServicoTelemetriaTransacao("rota3");
        var ex = new InvalidOperationException("erro");

        mock.RegistrarExcecao(transacao, ex);

        Assert.Equal(transacao, mock.UltimaTransacao);
        Assert.Equal(ex, mock.UltimaExcecao);
    }
}
