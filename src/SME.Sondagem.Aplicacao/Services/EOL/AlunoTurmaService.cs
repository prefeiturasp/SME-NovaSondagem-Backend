using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using System.Net;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class AlunoTurmaService : IAlunoTurmaService
    {
        private const int CacheTtlMinutos = 15;

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRepositorioCache repositorioCache;

        public AlunoTurmaService(IHttpClientFactory httpClientFactory, IRepositorioCache repositorioCache)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.repositorioCache = repositorioCache ?? throw new ArgumentNullException(nameof(repositorioCache));
        }

        public async Task<IEnumerable<DadosAlunoPorTurmaDto>> InformacoesAlunosPorTurma(long codigoTurma, CancellationToken cancellationToken = default)
        {
            var resultado = new List<DadosAlunoPorTurmaDto>();

            if (codigoTurma == 0)
                return resultado;

            var chave = $"sondagem-aluno-turma-informacoes:{codigoTurma}";
            var cached = await repositorioCache.ObterRedisAsync<List<DadosAlunoPorTurmaDto>>(chave);
            if (cached != null) return cached;

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
                    var alunosTurma = JsonConvert.DeserializeObject<List<DadosAlunoPorTurmaDto>>(alunosTurmaJson) ?? resultado;
                    await repositorioCache.SalvarRedisAsync(chave, alunosTurma, CacheTtlMinutos);
                    return alunosTurma;
                }
            }

            return resultado;
        }
    }
}