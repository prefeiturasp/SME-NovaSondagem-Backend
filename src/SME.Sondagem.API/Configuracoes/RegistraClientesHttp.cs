using Polly;
using Polly.Extensions.Http;
using SME.Sondagem.Infra.EnvironmentVariables;
using System.Net;

namespace SME.Sondagem.API.Configuracoes;

public static class RegistraClientesHttp
{
    public static void Registrar(IServiceCollection services, GithubOptions githubOptions)
    {
        var policy = ObterPolicyBaseHttp();

        services.AddHttpClient(name: "githubApi", c =>
        {
            c.BaseAddress = new Uri(githubOptions.Url);
            c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            c.DefaultRequestHeaders.Add("User-Agent", "SGP");

        }).AddPolicyHandler(policy);
    }

    static IAsyncPolicy<HttpResponseMessage> ObterPolicyBaseHttp()
    {
        return HttpPolicyExtensions
             .HandleTransientHttpError()
             .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
             .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                         retryAttempt)));
    }
}