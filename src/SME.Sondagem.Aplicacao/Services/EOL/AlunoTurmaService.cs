using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using System.Net;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class AlunoTurmaService : IAlunoTurmaService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AlunoTurmaService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<DadosAlunoPorTurmaDTO>> InformacoesAlunosPorTurma(long codigoTurma, CancellationToken cancellationToken = default)
        {
            var resultado = new List<DadosAlunoPorTurmaDTO>();

            if (codigoTurma == 0)
                return resultado;

            var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);

            var url = string.Format(ServicoEolConstants.URL_ALUNOS_TURMA_INFORMACOES, codigoTurma);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(180));

            var response = await httpClient.GetAsync(url, cts.Token);

            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var alunosTurmaJson = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!string.IsNullOrEmpty(alunosTurmaJson))
                {
                    var alunosTurma = JsonConvert.DeserializeObject<IEnumerable<DadosAlunoPorTurmaDTO>>(alunosTurmaJson);
                    return alunosTurma ?? resultado;
                }
            }

            return resultado;
        }
    }
}