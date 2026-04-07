using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SME.Sondagem.Aplicacao.Services.EOL
{
    public class DadosAlunosService : IDadosAlunosService
    {
        private const int TamanhoBatch = 100; 
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IAlunoTurmaService _alunoTurmaService;
        private readonly IRepositorioGeneroSexo _repositorioGeneroSexo;
        private readonly IRepositorioRacaCor _repositorioRacaCor;

        public DadosAlunosService(IHttpClientFactory httpClientFactory, IAlunoTurmaService alunoTurmaService, IRepositorioGeneroSexo repositorioGeneroSexo
            , IRepositorioRacaCor repositorioRacaCor)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _alunoTurmaService = alunoTurmaService ?? throw new ArgumentNullException(nameof(alunoTurmaService));
            _repositorioGeneroSexo = repositorioGeneroSexo ?? throw new ArgumentNullException(nameof(repositorioGeneroSexo));
            _repositorioGeneroSexo = repositorioGeneroSexo ?? throw new ArgumentNullException(nameof(repositorioGeneroSexo));
            _repositorioRacaCor = repositorioRacaCor ?? throw new ArgumentNullException(nameof(repositorioRacaCor));
        }

        public async Task<IEnumerable<AlunoEolDto>> ObterDadosAlunosPorCodigoUe(List<string> codigoAlunos, CancellationToken cancellationToken = default)
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
                cancellationToken.ThrowIfCancellationRequested();

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

        public async Task<IEnumerable<AlunoRacaGeneroDto>> ObterDadosRacaGeneroAlunos(int turmaId, CancellationToken cancellationToken = default)
        {
            var dadosAlunos = await _alunoTurmaService.InformacoesAlunosPorTurma(turmaId, cancellationToken);

            if (dadosAlunos == null || !dadosAlunos.Any())
                return [];

            var dadosGeneroSexo = await _repositorioGeneroSexo.ListarAsync(cancellationToken);
            var dadosRacaCor = await _repositorioRacaCor.ListarAsync(cancellationToken);

            var dados = dadosAlunos.Select(x => new AlunoRacaGeneroDto
            {
                CodigoAluno = x.CodigoAluno,
                Raca = ConverterCodigoRacaParaDescricao(x.Raca),
                Sexo = ConverterCodigoGeneroParaDescricao(x.Sexo),
                SexoId = ObterIdSexoGenero(x.Sexo, [.. dadosGeneroSexo]),
                RacaId = ObterIdRaca(x.Raca, [.. dadosRacaCor]),
            }).ToList();

            return dados;
        }

        private static int? ObterIdRaca(string codigoRaca, List<RacaCor> dadosRacaCor)
        {
            return dadosRacaCor.FirstOrDefault(g => string.Equals(g.Descricao, codigoRaca, StringComparison.OrdinalIgnoreCase))?.Id;
        }
        private static int? ObterIdSexoGenero(string codigoGenero,List<GeneroSexo> dadosGeneroSexo)
        {
           return dadosGeneroSexo.FirstOrDefault(g => string.Equals(g.Sigla, codigoGenero, StringComparison.OrdinalIgnoreCase))?.Id;
        }

        private static string ConverterCodigoGeneroParaDescricao(string codigoGenero)
        {
            return codigoGenero?.ToUpperInvariant() switch
            {
                "M" => "Masculino",
                "F" => "Feminino",
                _ => string.IsNullOrWhiteSpace(codigoGenero) ? string.Empty : codigoGenero
            };
        }

        private static string ConverterCodigoRacaParaDescricao(string codigoRaca)
        {
            if (string.IsNullOrWhiteSpace(codigoRaca))
                return string.Empty;

            var racaUpper = codigoRaca.ToUpperInvariant();
            return racaUpper switch
            {
                "BRANCA" => "Branca",
                "PRETA" => "Preta",
                "PARDA" => "Parda",
                "AMARELA" => "Amarela",
                "INDIGENA" => "Indígena",
                "INDÍGENA" => "Indígena",
                _ => char.ToUpperInvariant(racaUpper[0]) + racaUpper[1..].ToLowerInvariant()
            };
        }
    }
}