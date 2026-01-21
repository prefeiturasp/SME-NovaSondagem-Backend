using SME.Sondagem.Infra.Services;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.API.Configuracoes
{
    [ExcludeFromCodeCoverage]
    public static class RegistraApiEol
    {
        public static void Registrar(IServiceCollection services, IConfiguration configuration)
        {
            var urlApiEol = configuration.GetValue<string>("UrlApiEol");

            if (string.IsNullOrWhiteSpace(urlApiEol))
                throw new InvalidOperationException("A configuração 'UrlApiEol' é obrigatória.");

            services.AddHttpClient("ApiEol", client =>
            {
                client.BaseAddress = new Uri(urlApiEol);
                client.DefaultRequestHeaders.Add("x-api-eol-key", configuration.GetSection("ApiKeyEolApi").Value);
            });

            services.AddHttpClient(ServicoEolConstants.SERVICO, client =>
            {
                client.BaseAddress = new Uri(urlApiEol);
                client.DefaultRequestHeaders.Add("x-api-eol-key", configuration.GetSection("ApiKeyEolApi").Value);
                client.Timeout = TimeSpan.FromSeconds(180);
            });
        }
    }

}
