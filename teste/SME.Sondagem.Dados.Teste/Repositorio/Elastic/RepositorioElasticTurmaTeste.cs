using Moq;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace SME.Sondagem.Dados.Teste.Repositorio.Elastic
{
    public class RepositorioElasticTurmaTeste
    {
        private readonly Mock<IRepositorioElasticTurma> _repositorioMock;

        public RepositorioElasticTurmaTeste()
        {
            _repositorioMock = new Mock<IRepositorioElasticTurma>();
        }

        [Fact]
        public async Task ObterTurmaPorId_FiltroComTurmaIdZero_DeveRetornarNull()
        {
            var filtro = new FiltroQuestionario { TurmaId = 0 };

            _repositorioMock
                .Setup(x => x.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            var resultado = await _repositorioMock.Object.ObterTurmaPorId(filtro, CancellationToken.None);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterTurmaPorId_TurmaEncontrada_DeveRetornarTurma()
        {
            var filtro = new FiltroQuestionario { TurmaId = 123 };
            var turmaEsperada = new TurmaElasticDto
            {
                CodigoTurma = 123,
                CodigoEscola = "094765",
                AnoLetivo = 2024,
                NomeTurma = "1A",
                SerieEnsino = "1",
                Modalidade = 5
            };

            _repositorioMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmaEsperada);

            var resultado = await _repositorioMock.Object.ObterTurmaPorId(filtro, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(123, resultado.CodigoTurma);
            Assert.Equal("094765", resultado.CodigoEscola);
            Assert.Equal(2024, resultado.AnoLetivo);
            Assert.Equal("1A", resultado.NomeTurma);
            Assert.Equal("1", resultado.SerieEnsino);
            Assert.Equal(5, resultado.Modalidade);

            _repositorioMock.Verify(
                x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ObterTurmaPorId_TurmaNaoEncontrada_DeveRetornarNull()
        {
            var filtro = new FiltroQuestionario { TurmaId = 999 };

            _repositorioMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            var resultado = await _repositorioMock.Object.ObterTurmaPorId(filtro, CancellationToken.None);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterTurmaPorId_ComTurmaCompleta_DeveManterTodosOsCampos()
        {
            var filtro = new FiltroQuestionario { TurmaId = 789 };
            var turmaEsperada = new TurmaElasticDto
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
                    new ComponenteCurricularElasticDto
                    {
                        Codigo = 1,
                        Nome = "Português"
                    }
                }
            };

            _repositorioMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmaEsperada);

            var resultado = await _repositorioMock.Object.ObterTurmaPorId(filtro, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(789, resultado.CodigoTurma);
            Assert.Equal("999999", resultado.CodigoEscola);
            Assert.Equal(2024, resultado.AnoLetivo);
            Assert.Equal("3B", resultado.NomeTurma);
            Assert.Equal("3", resultado.SerieEnsino);
            Assert.Equal("3º Ano B", resultado.NomeFiltro);
            Assert.Equal(5, resultado.Modalidade);
            Assert.Equal("3", resultado.AnoTurma);
            Assert.Equal(1, resultado.TipoTurma);
            Assert.NotNull(resultado.Componentes);
            Assert.Single(resultado.Componentes);
            Assert.Equal(1, resultado.Componentes.First().Codigo);
            Assert.Equal("Português", resultado.Componentes.First().Nome);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999999)]
        public async Task ObterTurmaPorId_ComDiferentesTurmaIds_DeveRetornarTurmaCorrespondente(int turmaId)
        {
            var filtro = new FiltroQuestionario { TurmaId = turmaId };
            var turmaEsperada = new TurmaElasticDto { CodigoTurma = turmaId };

            _repositorioMock
                .Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmaEsperada);

            var resultado = await _repositorioMock.Object.ObterTurmaPorId(filtro, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(turmaId, resultado.CodigoTurma);
        }
    }
}
