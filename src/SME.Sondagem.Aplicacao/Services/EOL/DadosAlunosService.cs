using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos;
using Newtonsoft.Json;
using System.Text;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class DadosAlunosService : IDadosAlunosService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public DadosAlunosService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<AlunoEolDto>> ObterDadosAlunosPorCodigoUe(List<string> codigoAlunos, CancellationToken cancellationToken = default)
        {
            var resultado = new List<AlunoEolDto>();

            if (codigoAlunos.Count == 0) return resultado;

            var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);
            var url = $"alunos/obter-nomes-alunos";
            int anoAtual = DateTime.Now.Year;
            var requestBody = new
            {
                codigosAlunos = codigoAlunos,
                anoLetivo = anoAtual
            };
            var jsonParaPost = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var resposta = await httpClient.PostAsync(url, jsonParaPost, cancellationToken);

            if (!resposta.IsSuccessStatusCode) return resultado;
            var json = await resposta.Content.ReadAsStringAsync(cancellationToken);
            resultado = JsonConvert.DeserializeObject<List<AlunoEolDto>>(json) ?? resultado;
            return resultado;
        }
    }
}
