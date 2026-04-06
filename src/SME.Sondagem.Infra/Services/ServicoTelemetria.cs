using SME.Sondagem.Infra.EnvironmentVariables;
using SME.Sondagem.Infra.Interfaces;
using System.Diagnostics;

namespace SME.Sondagem.Infra.Services;

public class ServicoTelemetria : IServicoTelemetria
{
    public static readonly ActivitySource SondagemActivitySource = new ActivitySource("SME.Sondagem");
    private readonly TelemetriaOptions _telemetriaOptions;
    private const string Excecao = "Exception";
    private const string ExcecaoMenssagem = "exception.message";
    private const string ExcecaoStacktrace = "exception.stacktrace";

    public ServicoTelemetria(TelemetriaOptions telemetriaOptions)
    {
        _telemetriaOptions = telemetriaOptions ?? throw new ArgumentNullException(nameof(telemetriaOptions));
    }

    public ServicoTelemetriaTransacao IniciarTransacao(string rota)
    {
        var transacao = new ServicoTelemetriaTransacao(rota);
        
        var activity = SondagemActivitySource.StartActivity(rota);
        transacao.Activity = activity;
        transacao.InicioOperacao = DateTime.UtcNow;
        if (_telemetriaOptions.ApplicationInsights)
        {
             transacao.Temporizador = Stopwatch.StartNew();
        }

        return transacao;
    }

    public void FinalizarTransacao(ServicoTelemetriaTransacao servicoTelemetriaTransacao)
    {
        if (servicoTelemetriaTransacao.Activity != null)
        {
            servicoTelemetriaTransacao.Activity.SetStatus(ActivityStatusCode.Ok);
            servicoTelemetriaTransacao.Activity.Stop();
        }
        servicoTelemetriaTransacao.Temporizador?.Stop();
    }

    public void RegistrarExcecao(ServicoTelemetriaTransacao servicoTelemetriaTransacao, Exception ex)
    {
        if (servicoTelemetriaTransacao.Activity != null)
        {
            servicoTelemetriaTransacao.Activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            servicoTelemetriaTransacao.Activity.AddEvent(new ActivityEvent(Excecao, tags: new ActivityTagsCollection { { ExcecaoMenssagem, ex.Message }, { ExcecaoStacktrace, ex.StackTrace } }));
        }
    }

    public async Task<dynamic> RegistrarComRetornoAsync<T>(Func<Task<object>> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros)
    {
        using var activity = SondagemActivitySource.StartActivity(acaoNome);
        
        activity?.SetTag(telemetriaNome, telemetriaValor);
        if (!string.IsNullOrEmpty(parametros))
            activity?.SetTag("parametros", parametros);

        Stopwatch? temporizador = default;
        if (_telemetriaOptions.ApplicationInsights)
            temporizador = Stopwatch.StartNew();

        try
        {
            var result = await acao();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent(Excecao, tags: new ActivityTagsCollection { { ExcecaoMenssagem, ex.Message }, { ExcecaoStacktrace, ex.StackTrace } }));
            throw;
        }
        finally
        {
            temporizador?.Stop();
        }
    }

    public async Task<dynamic> RegistrarComRetornoAsync<T>(Func<Task<object>> acao, string acaoNome, string telemetriaNome, string telemetriaValor)
    {
        return await RegistrarComRetornoAsync<T>(acao, acaoNome, telemetriaNome, telemetriaValor, string.Empty);
    }

    public dynamic RegistrarComRetorno<T>(Func<object> acao, string acaoNome, string telemetriaNome, string telemetriaValor)
    {
        using var activity = SondagemActivitySource.StartActivity(acaoNome);
        activity?.SetTag(telemetriaNome, telemetriaValor);

        Stopwatch? temporizador = default;
        if (_telemetriaOptions.ApplicationInsights)
            temporizador = Stopwatch.StartNew();

        try
        {
            var result = acao();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent(Excecao, tags: new ActivityTagsCollection { { ExcecaoMenssagem, ex.Message }, { ExcecaoStacktrace, ex.StackTrace } }));
            throw;
        }
        finally
        {
            temporizador?.Stop();
        }
    }

    public void Registrar(Action acao, string acaoNome, string telemetriaNome, string telemetriaValor)
    {
        using var activity = SondagemActivitySource.StartActivity(acaoNome);
        activity?.SetTag(telemetriaNome, telemetriaValor);

        Stopwatch? temporizador = default;
        if (_telemetriaOptions.ApplicationInsights)
            temporizador = Stopwatch.StartNew();

        try
        {
            acao();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent(Excecao, tags: new ActivityTagsCollection { { ExcecaoMenssagem, ex.Message }, { ExcecaoStacktrace, ex.StackTrace } }));
            throw;
        }
        finally
        {
            temporizador?.Stop();
        }
    }

    public async Task RegistrarAsync(Func<Task> acao, string acaoNome, string telemetriaNome, string telemetriaValor)
    {
        using var activity = SondagemActivitySource.StartActivity(acaoNome);
        activity?.SetTag(telemetriaNome, telemetriaValor);

        Stopwatch? temporizador = default;
        if (_telemetriaOptions.ApplicationInsights)
            temporizador = Stopwatch.StartNew();

        try
        {
            await acao();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent(Excecao, tags: new ActivityTagsCollection { { ExcecaoMenssagem, ex.Message }, { ExcecaoStacktrace, ex.StackTrace } }));
            throw;
        }
        finally
        {
            temporizador?.Stop();
        }
    }

    public class ServicoTelemetriaTransacao
    {
        public ServicoTelemetriaTransacao(string nome)
        {
            Nome = nome;
            Sucesso = true;
        }

        public string Nome { get; set; }
        public DateTime InicioOperacao { get; set; }
        public Stopwatch? Temporizador { get; set; }
        public bool Sucesso { get; set; }
        public Activity? Activity { get; set; }
    }
}