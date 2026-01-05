using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using SME.Sondagem.Infra.EnvironmentVariables;
using System.Net;

namespace SME.Sondagem.API.Configuracoes;

public static class RegistraClientesHttp
{
    public static void Registrar(IServiceCollection services, GithubOptions githubOptions)
    {
        if (!Uri.TryCreate(githubOptions.Url, UriKind.Absolute, out var baseUri))
            throw new InvalidOperationException("GithubOptions.Url inválida ou não configurada.");

        var policy = ObterPolicyBaseHttp();

        services.AddHttpClient("githubApi", c =>
        {
            c.BaseAddress = baseUri;
            c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            c.DefaultRequestHeaders.Add("User-Agent", "SGP");
        })
        .AddPolicyHandler(policy);
    }
    static AsyncRetryPolicy<HttpResponseMessage> ObterPolicyBaseHttp()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            );
    }
}
