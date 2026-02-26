using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio.Integracao;
using SME.Sondagem.Infrastructure.Services;
using System.Net;
using System.Text;

namespace SME.Sondagem.Aplicacao.Services.SGP;

public class SolicitacaoRelatorioService : ISolicitacaoRelatorioService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SolicitacaoRelatorioService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> ObterSolicitacaoRelatorioAsync(FiltroSolicitacaoRelatorioIntegracaoSgpDto filtroRelatorio, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient(ServicoSGPConstants.SERVICO);

        var url = ServicoSGPConstants.URL_SOLICITACAO_RELATORIO;

        var body = JsonConvert.SerializeObject(filtroRelatorio, new JsonSerializerSettings());
        var resposta = await httpClient.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));

        if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
            return false;

        var json = await resposta.Content.ReadAsStringAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(json))
            return JsonConvert.DeserializeObject<bool>(json);

        return false;
    }

    public async Task<bool> RegistrarSolicitacaoRelatorioAsync(FiltroSolicitacaoRelatorioIntegracaoSgpDto filtroRelatorio, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient(ServicoSGPConstants.SERVICO);

        var url = ServicoSGPConstants.URL_SOLICITACAO_RELATORIO;

        var body = JsonConvert.SerializeObject(filtroRelatorio, new JsonSerializerSettings());
        var resposta = await httpClient.PutAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));

        if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
            return false;

        var json = await resposta.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(json))
            return true;

        return false;
    }
}
