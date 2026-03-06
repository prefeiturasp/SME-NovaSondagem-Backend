using System.Text;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class DadosAlunosService : IDadosAlunosService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public DadosAlunosService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<AlunoEolDto>> ObterDadosAlunosPorCodigoUe(string codigoUe, CancellationToken cancellationToken = default)
        {
            var resultado = new List<AlunoEolDto>();

            if (string.IsNullOrEmpty(codigoUe)) return resultado;

            var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);
            var url = $"alunos/ues/{codigoUe}/anosLetivos/{DateTime.Now.Year}";
            var resposta = await httpClient.GetAsync(url, cancellationToken);

            if (!resposta.IsSuccessStatusCode) return resultado;
            var json = await resposta.Content.ReadAsStringAsync(cancellationToken);
            resultado = JsonConvert.DeserializeObject<List<AlunoEolDto>>(json) ?? resultado;
            return resultado;
        }
    }
}