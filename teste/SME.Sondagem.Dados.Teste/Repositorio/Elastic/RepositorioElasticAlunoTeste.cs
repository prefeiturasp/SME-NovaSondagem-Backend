using Elastic.Clients.Elasticsearch;
using Moq;
using SME.Sondagem.Dados.Repositorio.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Interfaces;
using Xunit;

namespace SME.Sondagem.Dados.Testes.Repositorio.Elastic
{
    internal class RepositorioElasticAlunoTestDouble : RepositorioElasticAluno
    {
        public List<AlunoElasticDto> Itens { get; } = new();

        public RepositorioElasticAlunoTestDouble(IServicoTelemetria servicoTelemetria, ElasticsearchClient elasticClient)
            : base(servicoTelemetria, elasticClient) { }

        protected override bool EhRespostaValida<TResponse>(SearchResponse<TResponse> response) => true;

        protected override IReadOnlyCollection<TResponse> ObterDocumentos<TResponse>(SearchResponse<TResponse> response)
        {
            if (typeof(TResponse) == typeof(AlunoElasticDto))
                return (IReadOnlyCollection<TResponse>) (object) Itens;

            return base.ObterDocumentos(response);
        }
    }

    public class RepositorioElasticAlunoTeste
    {
        private readonly Mock<IServicoTelemetria> _mockServicoTelemetria;
        private readonly Mock<ElasticsearchClient> _mockElasticClient;
        private readonly RepositorioElasticAlunoTestDouble _repositorio;

        public RepositorioElasticAlunoTeste()
        {
            _mockServicoTelemetria = new Mock<IServicoTelemetria>();

            var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"));
            _mockElasticClient = new Mock<ElasticsearchClient>(settings);

            _repositorio = new RepositorioElasticAlunoTestDouble(
                _mockServicoTelemetria.Object,
                _mockElasticClient.Object
            );
        }

        [Fact]
        public void Construtor_DeveInicializarCorretamente()
        {           
            var repositorio = new RepositorioElasticAluno(
                _mockServicoTelemetria.Object,
                _mockElasticClient.Object
            );

            Assert.NotNull(repositorio);
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoServicoTelemetriaForNulo()
        {
            _ = Assert.Throws<ArgumentNullException>(() =>
                new RepositorioElasticAluno(null!, _mockElasticClient.Object));
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoElasticClientForNulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RepositorioElasticAluno(_mockServicoTelemetria.Object, null!));
        }

        [Fact]
        public async Task ObterAlunosPorIdTurma_DeveRetornarAlunosDistintos_QuandoExistiremAlunos()
        {
            var idTurma = 123;
            var cancellationToken = CancellationToken.None;

            var alunosEsperados = new List<AlunoElasticDto>
            {
                new AlunoElasticDto { CodigoAluno = 1, CodigoMatricula = 100, NomeAluno = "Aluno 1", CodigoTurma = idTurma },
                new AlunoElasticDto { CodigoAluno = 1, CodigoMatricula = 101, NomeAluno = "Aluno 1", CodigoTurma = idTurma },
                new AlunoElasticDto { CodigoAluno = 2, CodigoMatricula = 102, NomeAluno = "Aluno 2", CodigoTurma = idTurma }
            };

            ConfigurarMocksParaRetornar(alunosEsperados);

            var resultado = await _repositorio.ObterAlunosPorIdTurma(idTurma, cancellationToken);

            Assert.NotNull(resultado);
            var listaResultado = resultado.ToList();
            Assert.Equal(2, listaResultado.Count); 
            Assert.Contains(listaResultado, a => a.CodigoAluno == 1);
            Assert.Contains(listaResultado, a => a.CodigoAluno == 2);

            VerificarChamadaTelemetria("Obter alunos por Id da turma", Times.Once());
        }

        [Fact]
        public async Task ObterAlunosPorIdTurma_DeveRetornarListaVazia_QuandoNaoExistiremAlunos()
        {
            var idTurma = 999;
            var cancellationToken = CancellationToken.None;

            ConfigurarMocksParaRetornar(new List<AlunoElasticDto>());

            var resultado = await _repositorio.ObterAlunosPorIdTurma(idTurma, cancellationToken);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObterAlunosPorIdTurma_DeveChamarTelemetriaComParametrosCorretos()
        {
            var idTurma = 456;
            var cancellationToken = CancellationToken.None;

            ConfigurarMocksParaRetornar(new List<AlunoElasticDto>());

            await _repositorio.ObterAlunosPorIdTurma(idTurma, cancellationToken);

            _mockServicoTelemetria.Verify(
                x => x.RegistrarComRetornoAsync<SearchResponse<AlunoElasticDto>>(
                    It.IsAny<Func<Task<object>>>(),
                    "Elastic",
                    "Obter alunos por Id da turma",
                    It.IsAny<string>(),
                    It.Is<string>(s => s.Contains(idTurma.ToString()))),
                Times.Once);
        }

        [Fact]
        public async Task ObterAlunosPorIdTurma_DeveAplicarFiltroCorreto()
        {
            var idTurma = 789;
            var cancellationToken = CancellationToken.None;

            var alunosDaTurma = new List<AlunoElasticDto>
            {
                new AlunoElasticDto { CodigoAluno = 10, CodigoTurma = idTurma, NomeAluno = "Aluno Turma Correta" }
            };

            ConfigurarMocksParaRetornar(alunosDaTurma);

            var resultado = await _repositorio.ObterAlunosPorIdTurma(idTurma, cancellationToken);

            Assert.NotNull(resultado);
            Assert.All(resultado, aluno => Assert.Equal(idTurma, aluno.CodigoTurma));
        }

        [Fact]
        public async Task ObterAlunosPorIdTurma_DeveManterApenasUmRegistroPorCodigoAluno()
        {
            var idTurma = 100;
            var cancellationToken = CancellationToken.None;

            var alunosComDuplicatas = new List<AlunoElasticDto>
            {
                new AlunoElasticDto { CodigoAluno = 1, CodigoMatricula = 100, NomeAluno = "Aluno 1", CodigoTurma = idTurma },
                new AlunoElasticDto { CodigoAluno = 1, CodigoMatricula = 101, NomeAluno = "Aluno 1", CodigoTurma = idTurma },
                new AlunoElasticDto { CodigoAluno = 1, CodigoMatricula = 102, NomeAluno = "Aluno 1", CodigoTurma = idTurma },
                new AlunoElasticDto { CodigoAluno = 2, CodigoMatricula = 200, NomeAluno = "Aluno 2", CodigoTurma = idTurma },
                new AlunoElasticDto { CodigoAluno = 2, CodigoMatricula = 201, NomeAluno = "Aluno 2", CodigoTurma = idTurma }
            };

            ConfigurarMocksParaRetornar(alunosComDuplicatas);

            var resultado = await _repositorio.ObterAlunosPorIdTurma(idTurma, cancellationToken);

            var listaResultado = resultado.ToList();
            Assert.Equal(2, listaResultado.Count);
            Assert.Single(listaResultado, a => a.CodigoAluno == 1);
            Assert.Single(listaResultado, a => a.CodigoAluno == 2);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(9999)]
        public async Task ObterAlunosPorIdTurma_DeveFuncionarComDiferentesIdsTurma(int idTurma)
        {
            var cancellationToken = CancellationToken.None;
            var alunos = new List<AlunoElasticDto>
            {
                new AlunoElasticDto { CodigoAluno = 1, CodigoTurma = idTurma, NomeAluno = "Aluno Teste" }
            };

            ConfigurarMocksParaRetornar(alunos);

            var resultado = await _repositorio.ObterAlunosPorIdTurma(idTurma, cancellationToken);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
        }

        [Fact]
        public async Task ObterAlunosPorIdTurma_DeveRetornarPrimeiroRegistro_QuandoHouverDuplicatas()
        {
            var idTurma = 555;
            var cancellationToken = CancellationToken.None;

            var alunosComDuplicatas = new List<AlunoElasticDto>
            {
                new AlunoElasticDto { CodigoAluno = 1, CodigoMatricula = 100, NomeAluno = "Primeiro Registro", CodigoTurma = idTurma },
                new AlunoElasticDto { CodigoAluno = 1, CodigoMatricula = 101, NomeAluno = "Segundo Registro", CodigoTurma = idTurma }
            };

            ConfigurarMocksParaRetornar(alunosComDuplicatas);

            var resultado = await _repositorio.ObterAlunosPorIdTurma(idTurma, cancellationToken);

            var listaResultado = resultado.ToList();
            Assert.Single(listaResultado);
            Assert.Equal(100, listaResultado.First().CodigoMatricula);
            Assert.Equal("Primeiro Registro", listaResultado.First().NomeAluno);
        }

        [Fact]
        public async Task ObterAlunosPorIdTurma_DevePreservarDadosCompletos_DosPrimeiroRegistros()
        {
            var idTurma = 888;
            var cancellationToken = CancellationToken.None;
            var dataEsperada = new DateTime(2024, 1, 15);

            var alunos = new List<AlunoElasticDto>
            {
                new AlunoElasticDto
                {
                    CodigoAluno = 5,
                    CodigoMatricula = 500,
                    NomeAluno = "João Silva",
                    NomeSocialAluno = "João",
                    NumeroAlunoChamada = "10",
                    DataNascimento = dataEsperada,
                    CodigoTurma = idTurma
                }
            };

            ConfigurarMocksParaRetornar(alunos);

            var resultado = await _repositorio.ObterAlunosPorIdTurma(idTurma, cancellationToken);

            var aluno = resultado.First();
            Assert.Equal(5, aluno.CodigoAluno);
            Assert.Equal(500, aluno.CodigoMatricula);
            Assert.Equal("João Silva", aluno.NomeAluno);
            Assert.Equal("João", aluno.NomeSocialAluno);
            Assert.Equal("10", aluno.NumeroAlunoChamada);
            Assert.Equal(dataEsperada, aluno.DataNascimento);
        }

        #region Métodos Auxiliares

        private void ConfigurarMocksParaRetornar(List<AlunoElasticDto> documentos)
        {
            _repositorio.Itens.Clear();
            _repositorio.Itens.AddRange(documentos);

            var responseType = typeof(SearchResponse<AlunoElasticDto>);
            var response = (SearchResponse<AlunoElasticDto>)Activator.CreateInstance(responseType, nonPublic: true)!;

            _mockServicoTelemetria
                .Setup(x => x.RegistrarComRetornoAsync<SearchResponse<AlunoElasticDto>>(
                    It.IsAny<Func<Task<object>>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Callback((Func<Task<object>> func, string _, string _, string _, string _) =>
                {
                    try
                    {
                        func().GetAwaiter().GetResult();
                    }
                    catch
                    {
                    }
                })
                .ReturnsAsync(response);
        }

        private void VerificarChamadaTelemetria(string nomeConsulta, Times times)
        {
            _mockServicoTelemetria.Verify(
                x => x.RegistrarComRetornoAsync<SearchResponse<AlunoElasticDto>>(
                    It.IsAny<Func<Task<object>>>(),
                    "Elastic",
                    nomeConsulta,
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                times);
        }

        #endregion
    }
}