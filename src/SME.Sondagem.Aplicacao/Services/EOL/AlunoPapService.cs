using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using System.Net;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class AlunoPapService : IAlunoPapService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AlunoPapService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<Dictionary<int, bool>> VerificarAlunosPossuemProgramaPapAsync(IEnumerable<int> codigosAlunos, int anoLetivo, CancellationToken cancellationToken = default)
        {
            var resultado = new Dictionary<int, bool>();

            if (codigosAlunos == null || !codigosAlunos.Any())
                return resultado;

            var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);

            var codigosAlunosString = string.Join("&codigosAlunos=", codigosAlunos);
            var url = string.Format(ServicoEolConstants.URL_ALUNOS_ALUNOS_PAP, anoLetivo) + $"?codigosAlunos={codigosAlunosString}";

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(180));

            var response = await httpClient.GetAsync(url, cts.Token);

            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var alunosPapJson = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!string.IsNullOrEmpty(alunosPapJson))
                {
                    var alunosPap = JsonConvert.DeserializeObject<IEnumerable<DadosMatriculaAlunoTipoPapDto>>(alunosPapJson);
                    var codigosComPap = alunosPap?.Select(x => x.CodigoAluno).ToHashSet() ?? new HashSet<int>();

                    foreach (var codigoAluno in codigosAlunos)
                    {
                        resultado[codigoAluno] = codigosComPap.Contains(codigoAluno);
                    }

                    return resultado;
                }
            }

            // Se não houver resposta ou erro, inicializa todos como false
            foreach (var codigoAluno in codigosAlunos)
            {
                resultado[codigoAluno] = false;
            }

            return resultado;
        }
    }
}