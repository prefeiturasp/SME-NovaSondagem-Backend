using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos;
using System.Text;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class DadosAlunosService : IDadosAlunosService
    {
        private const int TamanhoBatch = 100; // ajuste conforme o limite da API
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRepositorioElasticAluno _repositorioElasticAluno;

        public DadosAlunosService(IHttpClientFactory httpClientFactory, IRepositorioElasticAluno repositorioElasticAluno)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this._repositorioElasticAluno = repositorioElasticAluno ?? throw new ArgumentNullException(nameof(repositorioElasticAluno));
        }

        public async Task<IEnumerable<AlunoEolDto>> ObterDadosAlunosPorCodigoUe2(
            List<string> codigoAlunos,
            CancellationToken cancellationToken = default)
        {
            if (codigoAlunos.Count == 0)
                return [];

            int anoAtual = DateTime.Now.Year;

            var batches = codigoAlunos
                .Distinct()
                .Select((codigo, index) => new { codigo, index })
                .GroupBy(x => x.index / TamanhoBatch)
                .Select(g => g.Select(x => x.codigo).ToList())
                .ToList();

            var tasks = batches.Select(batch =>
                _repositorioElasticAluno.ObterAlunosPorCodigo(batch, anoAtual, cancellationToken));

            var resultados = await Task.WhenAll(tasks);

            return resultados
                .SelectMany(lista => lista)
                .Select(item => new AlunoEolDto
                {
                    CodigoAluno = item.CodigoAluno,
                    CodigoEscola = item.CodigoEscola,
                    CodigoSituacaoMatricula = item.CodigoSituacaoMatricula,
                    CodigoTurma = item.CodigoTurma,
                    DataMatricula = item.DataMatricula,
                    NomeAluno = item.NomeAluno,
                    SituacaoMatricula = item.SituacaoMatricula
                });
        }

        public async Task<IEnumerable<AlunoEolDto>> ObterDadosAlunosPorCodigoUe(List<string> codigoAlunos, bool buscarNoEol, CancellationToken cancellationToken = default)
        {
            var resultado = new List<AlunoEolDto>();

            if (codigoAlunos.Count == 0) return resultado;

            if (buscarNoEol)
            {
                var busca = await BuscarNoEol(codigoAlunos,cancellationToken);
                if(busca.Any())
                    resultado.AddRange(busca);
            }
            else
            {
                var busca = await BuscarNoElastic(codigoAlunos, cancellationToken);
                if (busca.Any())
                    resultado.AddRange(busca);
            }

            return resultado;


        }
        private async Task<IEnumerable<AlunoEolDto>> BuscarNoEol(List<string> codigoAlunos, CancellationToken cancellationToken = default)
        {
            var resultado = new List<AlunoEolDto>();

            if (codigoAlunos.Count == 0) return resultado;

            var codigosDistintos = codigoAlunos.Distinct().ToList();
            var batches = codigosDistintos
                .Select((codigo, index) => new { codigo, index })
                .GroupBy(x => x.index / TamanhoBatch)
                .Select(g => g.Select(x => x.codigo).ToList())
                .ToList();

            var httpClient = httpClientFactory.CreateClient(ServicoEolConstants.SERVICO);
            var url = "alunos/obter-nomes-alunos";
            int anoAtual = DateTime.Now.Year;
            foreach (var batch in batches)
            {

                var requestBody = new
                {
                    codigosAlunos = batch,
                    anoLetivo = anoAtual
                };

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var jsonParaPost = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var resposta = await httpClient.PostAsync(url, jsonParaPost, cancellationToken);

                if (!resposta.IsSuccessStatusCode) continue;

                var json = await resposta.Content.ReadAsStringAsync(cancellationToken);
                var parcial = JsonConvert.DeserializeObject<List<AlunoEolDto>>(json);
                if (parcial != null)
                    resultado.AddRange(parcial);
            }

            return resultado;
        }
        private async Task<IEnumerable<AlunoEolDto>> BuscarNoElastic(List<string> codigoAlunos, CancellationToken cancellationToken = default)
        {
            var resultado = new List<AlunoEolDto>();
            if (codigoAlunos.Count == 0) return resultado;

            var codigosDistintos = codigoAlunos.Distinct().ToList();
            var batches = codigosDistintos
                .Select((codigo, index) => new { codigo, index })
                .GroupBy(x => x.index / TamanhoBatch)
                .Select(g => g.Select(x => x.codigo).ToList())
                .ToList();
            int anoAtual = DateTime.Now.Year;
            var retornoElastic = new List<AlunoElasticDto>();

            foreach (var batch in batches)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var lista = await _repositorioElasticAluno.ObterAlunosPorCodigo(batch, anoAtual, cancellationToken);
                if (lista.Any())
                    retornoElastic.AddRange(lista);
            }

            foreach (var item in retornoElastic)
            {

                var aluno = new AlunoEolDto
                {
                    CodigoAluno = item.CodigoAluno,
                    CodigoEscola = item.CodigoEscola,
                    CodigoSituacaoMatricula = item.CodigoSituacaoMatricula,
                    CodigoTurma = item.CodigoTurma,
                    DataMatricula = item.DataMatricula,
                    NomeAluno = item.NomeAluno,
                    SituacaoMatricula = item.SituacaoMatricula
                };
                resultado.Add(aluno);
            }

            return resultado;
        }
    }
}