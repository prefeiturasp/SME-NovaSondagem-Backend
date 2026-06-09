using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Aplicacao.Services.SGP
{
    public class ConsultaDeDresService : IConsultaDeDresService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServicoLog _servicoLog;

        public ConsultaDeDresService(IHttpClientFactory httpClientFactory, IServicoLog servicoLog)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(servicoLog));
            _servicoLog = servicoLog ?? throw new ArgumentNullException(nameof(servicoLog));
        }

        public async Task<IEnumerable<ObterDresSgpDto>> ObterDresSgpAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var resultado = new List<ObterDresSgpDto>();
                var httpClient = _httpClientFactory.CreateClient(ServicoSgpConstants.SERVICO);
                var url = ServicoSgpConstants.URL_OBTER_DRES;
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return resultado;
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                resultado = JsonConvert.DeserializeObject<List<ObterDresSgpDto>>(json) ?? resultado;
                return resultado;
            }
            catch (Exception e)
            {
                _servicoLog.Registrar($"Erro ao buscar DREs no SGP ConsultaDeDresService {e.InnerException} , {e.StackTrace}", e);
                return [];

            }

        }
    }
}
