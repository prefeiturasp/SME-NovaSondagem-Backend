using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos;
using System.Text;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class UeComDreEolService : IUeComDreEolService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public UeComDreEolService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<UeComDreEolDto>> ObterUesComDrePorCodigosUes(IEnumerable<string> codigosUes,CancellationToken cancellationToken = default)
        {
            try
            {
                var resultado = new List<UeComDreEolDto>();

                if (!codigosUes.Any())
                    return resultado;

                var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);
                var jsonParaPost = new StringContent(JsonConvert.SerializeObject(codigosUes), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(ServicoEolConstants.URL_OBTER_UE_COM_DRE, jsonParaPost, cancellationToken);

                if (!response.IsSuccessStatusCode) return resultado;
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                resultado = JsonConvert.DeserializeObject<List<UeComDreEolDto>>(json) ?? resultado;
                return resultado;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
