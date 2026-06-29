using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class AlunoPapService : IAlunoPapService
    {
        private const int CacheTtlMinutos = 15;

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRepositorioCache repositorioCache;

        public AlunoPapService(IHttpClientFactory httpClientFactory, IRepositorioCache repositorioCache)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.repositorioCache = repositorioCache ?? throw new ArgumentNullException(nameof(repositorioCache));
        }

        public async Task<Dictionary<int, bool>> VerificarAlunosPossuemProgramaPapAsync(IEnumerable<int> codigosAlunos, int anoLetivo, CancellationToken cancellationToken = default)
        {
            var resultado = new Dictionary<int, bool>();

            if (codigosAlunos == null || !codigosAlunos.Any())
                return resultado;

            var codigosOrdenados = codigosAlunos.OrderBy(x => x).ToList();
            var codigosHash = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(string.Join(",", codigosOrdenados))));
            var chave = $"sondagem-alunos-pap:{anoLetivo}:{codigosHash}";

            var cached = await repositorioCache.ObterRedisAsync<Dictionary<int, bool>>(chave);
            if (cached != null) return cached;

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

                    await repositorioCache.SalvarRedisAsync(chave, resultado, CacheTtlMinutos);
                    return resultado;
                }
            }

            // Se n�o houver resposta ou erro, inicializa todos como false
            foreach (var codigoAluno in codigosAlunos)
            {
                resultado[codigoAluno] = false;
            }

            return resultado;
        }
    }
}