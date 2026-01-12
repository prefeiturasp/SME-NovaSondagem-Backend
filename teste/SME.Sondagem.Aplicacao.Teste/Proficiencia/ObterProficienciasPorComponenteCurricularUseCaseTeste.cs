using Moq;
using SME.Sondagem.Aplicacao.UseCases.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using SME.Sondagem.Infra.Exceptions;
using System.Net;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Proficiencia
{
    public class ObterProficienciasPorComponenteCurricularUseCaseTeste
    {
        private readonly Mock<IRepositorioProficiencia> _proficienciaRepositorioMock;
        private readonly ObterProficienciasPorComponenteCurricularUseCase _useCase;

        public ObterProficienciasPorComponenteCurricularUseCaseTeste()
        {
            _proficienciaRepositorioMock = new Mock<IRepositorioProficiencia>();
            _useCase = new ObterProficienciasPorComponenteCurricularUseCase(_proficienciaRepositorioMock.Object);
        }

        [Fact]
        public void Construtor_QuandoRepositorioNulo_DeveLancarArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new ObterProficienciasPorComponenteCurricularUseCase(null));

            Assert.Equal("proficienciaRepositorio", exception.ParamName);
        }

        [Fact]
        public async Task ExecutarAsync_QuandoComponenteCurricularIdZero_DeveLancarNegocioException()
        {
            var componenteCurricularId = 0L;

            var exception = await Assert.ThrowsAsync<NegocioException>(() =>
                _useCase.ExecutarAsync(componenteCurricularId));

            Assert.Equal(MensagemNegocioComuns.INFORMAR_ID_MAIOR_QUE_ZERO, exception.Message);

            _proficienciaRepositorioMock.Verify(
                x => x.ObterProeficienciaPorComponenteCurricular(It.IsAny<long>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecutarAsync_QuandoNenhumRegistroEncontrado_DeveLancarNegocioException()
        {
            var componenteCurricularId = 1L;
            var listaVazia = Enumerable.Empty<ProficienciaDto>();

            _proficienciaRepositorioMock
                .Setup(x => x.ObterProeficienciaPorComponenteCurricular(componenteCurricularId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(listaVazia);

            var exception = await Assert.ThrowsAsync<NegocioException>(() =>
                _useCase.ExecutarAsync(componenteCurricularId));

            Assert.Equal(MensagemNegocioComuns.NENHUM_REGISTRO_ENCONTRADO, exception.Message);

            _proficienciaRepositorioMock.Verify(
                x => x.ObterProeficienciaPorComponenteCurricular(componenteCurricularId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_QuandoEncontraProficiencias_DeveRetornarLista()
        {
            var componenteCurricularId = 1L;
            var proficienciasEsperadas = new List<ProficienciaDto>
            {
                new ProficienciaDto { Id = 1, Nome = "Proficiência 1" },
                new ProficienciaDto { Id = 2, Nome = "Proficiência 2" }
            };

            _proficienciaRepositorioMock
                .Setup(x => x.ObterProeficienciaPorComponenteCurricular(componenteCurricularId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(proficienciasEsperadas);

            var resultado = await _useCase.ExecutarAsync(componenteCurricularId);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.Equal(proficienciasEsperadas, resultado);

            _proficienciaRepositorioMock.Verify(
                x => x.ObterProeficienciaPorComponenteCurricular(componenteCurricularId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_QuandoChamadoComCancellationToken_DevePassarTokenParaRepositorio()
        {
            var componenteCurricularId = 1L;
            var cancellationToken = new CancellationToken();
            var proficiencias = new List<ProficienciaDto>
            {
                new ProficienciaDto { Id = 1, Nome = "Proficiência 1" }
            };

            _proficienciaRepositorioMock
                .Setup(x => x.ObterProeficienciaPorComponenteCurricular(componenteCurricularId, cancellationToken))
                .ReturnsAsync(proficiencias);

            var resultado = await _useCase.ExecutarAsync(componenteCurricularId, cancellationToken);

            Assert.NotNull(resultado);
            _proficienciaRepositorioMock.Verify(
                x => x.ObterProeficienciaPorComponenteCurricular(componenteCurricularId, cancellationToken),
                Times.Once);
        }

        [Theory]
        [InlineData(1L)]
        [InlineData(999L)]
        [InlineData(long.MaxValue)]
        public async Task ExecutarAsync_QuandoComponenteCurricularIdValido_DeveConsultarRepositorio(long componenteCurricularId)
        {
            var proficiencias = new List<ProficienciaDto>
            {
                new ProficienciaDto { Id = 1, Nome = "Teste" }
            };

            _proficienciaRepositorioMock
                .Setup(x => x.ObterProeficienciaPorComponenteCurricular(componenteCurricularId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(proficiencias);

            var resultado = await _useCase.ExecutarAsync(componenteCurricularId);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
        }
    }
}
