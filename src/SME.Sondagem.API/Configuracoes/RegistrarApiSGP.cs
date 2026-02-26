using SME.Sondagem.Infrastructure.Services;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.API.Configuracoes;

[ExcludeFromCodeCoverage]
public static class RegistrarApiSGP
{
    public static void Registrar(IServiceCollection services, IConfiguration configuration)
    {
        var urlApiSGP = configuration.GetValue<string>("UrlApiSGP");

        if (string.IsNullOrWhiteSpace(urlApiSGP))
            throw new InvalidOperationException("A configuração 'UrlApiSGP' é obrigatória.");

        services.AddHttpClient("ApiSGP", client =>
        {
            client.BaseAddress = new Uri(urlApiSGP);
            client.DefaultRequestHeaders.Add("x-sgp-api-key", configuration.GetSection("ApiKeySGPApi").Value);
        });

        services.AddHttpClient(ServicoSGPConstants.SERVICO, client =>
        {
            client.BaseAddress = new Uri(urlApiSGP);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-sgp-api-key", configuration.GetSection("ApiKeySGPApi").Value);
        });
    }
}
