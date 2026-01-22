using Elastic.Clients.Elasticsearch;
using Moq;
using SME.Sondagem.Dados.Repositorio.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Interfaces;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Elastic
{
    internal class RepositorioElasticTurmaTestDouble : RepositorioElasticTurma
    {
        public List<TurmaElasticDto> Itens { get; } = new();

        public RepositorioElasticTurmaTestDouble(IServicoTelemetria servicoTelemetria, ElasticsearchClient elasticClient)
            : base(servicoTelemetria, elasticClient) { }

        protected override bool EhRespostaValida<TResponse>(SearchResponse<TResponse> response) => true;

        protected override IReadOnlyCollection<TResponse> ObterDocumentos<TResponse>(SearchResponse<TResponse> response)
        {
            if (typeof(TResponse) == typeof(TurmaElasticDto))
                return (IReadOnlyCollection<TResponse>)(object)Itens;

            return base.ObterDocumentos(response);
        }
    }

    public class RepositorioElasticTurmaTeste
    {
        private readonly Mock<IServicoTelemetria> _mockServicoTelemetria;
        private readonly Mock<ElasticsearchClient> _mockElasticClient;
        private readonly RepositorioElasticTurmaTestDouble _repositorio;

        public RepositorioElasticTurmaTeste()
        {
            _mockServicoTelemetria = new Mock<IServicoTelemetria>();

            var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"));
            _mockElasticClient = new Mock<ElasticsearchClient>(settings);

            _repositorio = new RepositorioElasticTurmaTestDouble(
                _mockServicoTelemetria.Object,
                _mockElasticClient.Object
            );  
        }

        [Fact]
        public void Construtor_DeveInicializarCorretamente()
        {
            var repositorio = new RepositorioElasticTurma(
                _mockServicoTelemetria.Object,
                _mockElasticClient.Object
            );

            Assert.NotNull(repositorio);
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoServicoTelemetriaForNulo()
        {
            _ = Assert.Throws<ArgumentNullException>(() =>
                new RepositorioElasticTurma(null!, _mockElasticClient.Object));
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_QuandoElasticClientForNulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RepositorioElasticTurma(_mockServicoTelemetria.Object, null!));
        }

        [Fact]
        public async Task ObterTurmaPorId_FiltroComTurmaIdZero_DeveRetornarNull()
        {
            var filtro = new FiltroQuestionario { TurmaId = 0 };
            ConfigurarMocksParaRetornar(new List<TurmaElasticDto>());

            var resultado = await _repositorio.ObterTurmaPorId(filtro, CancellationToken.None);

            Assert.Null(resultado);
            VerificarChamadaTelemetria("Obter turma por código", Times.Never());
        }

        [Fact]
        public async Task ObterTurmaPorId_TurmaEncontrada_DeveRetornarTurma()
        {
            var filtro = new FiltroQuestionario { TurmaId = 123 };
            var itens = new List<TurmaElasticDto>
            {
                new TurmaElasticDto
                {
                    CodigoTurma = 123,
                    CodigoEscola = "094765",
                    AnoLetivo = 2024,
                    NomeTurma = "1A",
                    SerieEnsino = "1",
                    Modalidade = 5
                }
            };

            ConfigurarMocksParaRetornar(itens);

            var resultado = await _repositorio.ObterTurmaPorId(filtro, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(123, resultado!.CodigoTurma);
            Assert.Equal("094765", resultado.CodigoEscola);
            Assert.Equal(2024, resultado.AnoLetivo);
            Assert.Equal("1A", resultado.NomeTurma);
            Assert.Equal("1", resultado.SerieEnsino);
            Assert.Equal(5, resultado.Modalidade);
        }

        [Fact]
        public async Task ObterTurmaPorId_TurmaNaoEncontrada_DeveRetornarNull()
        {
            var filtro = new FiltroQuestionario { TurmaId = 999 };
            ConfigurarMocksParaRetornar(new List<TurmaElasticDto>());

            var resultado = await _repositorio.ObterTurmaPorId(filtro, CancellationToken.None);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterTurmaPorId_ComTurmaCompleta_DeveManterTodosOsCampos()
        {
            var filtro = new FiltroQuestionario { TurmaId = 789 };
            var itens = new List<TurmaElasticDto>
            {
                new TurmaElasticDto
                {
                    CodigoTurma = 789,
                    CodigoEscola = "999999",
                    AnoLetivo = 2024,
                    NomeTurma = "3B",
                    SerieEnsino = "3",
                    NomeFiltro = "3º Ano B",
                    Modalidade = 5,
                    AnoTurma = "3",
                    TipoTurma = 1,
                    Componentes = new List<ComponenteCurricularElasticDto>
                    {
                        new ComponenteCurricularElasticDto { Codigo = 1, Nome = "Português" }
                    }
                }
            };

            ConfigurarMocksParaRetornar(itens);

            var resultado = await _repositorio.ObterTurmaPorId(filtro, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(789, resultado!.CodigoTurma);
            Assert.Equal("999999", resultado.CodigoEscola);
            Assert.Equal(2024, resultado.AnoLetivo);
            Assert.Equal("3B", resultado.NomeTurma);
            Assert.Equal("3", resultado.SerieEnsino);
            Assert.Equal("3º Ano B", resultado.NomeFiltro);
            Assert.Equal(5, resultado.Modalidade);
            Assert.Equal("3", resultado.AnoTurma);
            Assert.Equal(1, resultado.TipoTurma);
            Assert.NotNull(resultado.Componentes);
            Assert.Single(resultado.Componentes!);
            Assert.Equal(1, resultado.Componentes!.First().Codigo);
            Assert.Equal("Português", resultado.Componentes!.First().Nome);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999999)]
        public async Task ObterTurmaPorId_ComDiferentesTurmaIds_DeveRetornarTurmaCorrespondente(int turmaId)
        {
            var filtro = new FiltroQuestionario { TurmaId = turmaId };
            ConfigurarMocksParaRetornar(new List<TurmaElasticDto>
            {
                new TurmaElasticDto { CodigoTurma = turmaId }
            });

            var resultado = await _repositorio.ObterTurmaPorId(filtro, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(turmaId, resultado!.CodigoTurma);
        }

        private void ConfigurarMocksParaRetornar(List<TurmaElasticDto> documentos)
        {
            _repositorio.Itens.Clear();
            _repositorio.Itens.AddRange(documentos);

            var searchResponse = (SearchResponse<TurmaElasticDto>)Activator.CreateInstance(typeof(SearchResponse<TurmaElasticDto>), nonPublic: true)!;

            _mockServicoTelemetria
                .Setup(x => x.RegistrarComRetornoAsync<SearchResponse<TurmaElasticDto>>(
                    It.IsAny<Func<Task<object>>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Callback((Func<Task<object>> func, string _, string _, string _, string _) =>
                {
                    try { func().GetAwaiter().GetResult(); } catch { }
                })
                .ReturnsAsync(searchResponse);

            _mockServicoTelemetria
                .Setup(x => x.RegistrarComRetornoAsync<GetResponse<TurmaElasticDto>>(
                    It.IsAny<Func<Task<object>>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() =>
                {
                    var getResponse = (GetResponse<TurmaElasticDto>)Activator.CreateInstance(typeof(GetResponse<TurmaElasticDto>), nonPublic: true)!;

                    var foundProp = typeof(GetResponse<TurmaElasticDto>).GetProperty("Found");
                    var sourceProp = typeof(GetResponse<TurmaElasticDto>).GetProperty("Source");

                    var doc = documentos.FirstOrDefault();
                    foundProp?.SetValue(getResponse, doc != null);
                    sourceProp?.SetValue(getResponse, doc);

                    return getResponse;
                });
        }

        private void VerificarChamadaTelemetria(string nomeConsulta, Times times)
        {
            _mockServicoTelemetria.Verify(
                x => x.RegistrarComRetornoAsync<GetResponse<TurmaElasticDto>>(
                    It.IsAny<Func<Task<object>>>(),
                    "Elastic",
                    nomeConsulta,
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                times);
        }
    }
}
