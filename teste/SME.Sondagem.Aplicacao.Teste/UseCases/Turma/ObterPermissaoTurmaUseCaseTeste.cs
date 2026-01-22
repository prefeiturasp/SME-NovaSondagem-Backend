using Moq;
using SME.Sondagem.Aplicacao.UseCases.Turma;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Turma
{
    public class ObterPermissaoTurmaUseCaseTeste
    {
        private readonly Mock<IRepositorioElasticTurma> repositorioElasticTurmaMock;

        public ObterPermissaoTurmaUseCaseTeste()
        {
            repositorioElasticTurmaMock = new Mock<IRepositorioElasticTurma>();
        }

        private static TurmaElasticDto CriarTurma(
            int modalidade,
            string anoTurma,
            int anoLetivo)
        {
            return new TurmaElasticDto
            {
                Modalidade = modalidade,
                AnoTurma = anoTurma,
                AnoLetivo = anoLetivo
            };
        }

        [Fact]
        public async Task ObterPermissaoTurma_TurmaNaoLocalizada_DeveLancarExcecao400()
        {
            repositorioElasticTurmaMock
                .Setup(x => x.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))!
                .ReturnsAsync((TurmaElasticDto?)null);

            var useCase = new ObterPermissaoTurmaUseCase(
                repositorioElasticTurmaMock.Object);

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
                useCase.ObterPermissaoTurma(123456, CancellationToken.None));

            Assert.Equal(400, exception.StatusCode);
            Assert.Equal("Turma não localizada", exception.Message);
        }

        [Fact]
        public async Task ObterPermissaoTurma_ModalidadeNaoPermitida_DeveLancarExcecao()
        {
            var turma = CriarTurma(
                modalidade: (int)Modalidade.EducacaoInfantil,
                anoTurma: "1",
                anoLetivo: DateTime.Now.Year);

            repositorioElasticTurmaMock
                .Setup(x => x.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turma);

            var useCase = new ObterPermissaoTurmaUseCase(
                repositorioElasticTurmaMock.Object);

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
                useCase.ObterPermissaoTurma(123456, CancellationToken.None));

            Assert.Contains("Somente é possível utilizar a Sondagem", exception.Message);
        }

        [Fact]
        public async Task ObterPermissaoTurma_SerieAnoNaoPermitido_DeveLancarExcecao()
        {
            var turma = CriarTurma(
                modalidade: (int)Modalidade.Fundamental,
                anoTurma: "5",
                anoLetivo: DateTime.Now.Year);

            repositorioElasticTurmaMock
                .Setup(x => x.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turma);

            var useCase = new ObterPermissaoTurmaUseCase(
                repositorioElasticTurmaMock.Object);

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
                useCase.ObterPermissaoTurma(123456, CancellationToken.None));

            Assert.Contains("Somente é possível utilizar a Sondagem", exception.Message);
        }

        [Fact]
        public async Task ObterPermissaoTurma_AnoLetivoAnterior_DeveLancarExcecao()
        {
            var turma = CriarTurma(
                modalidade: (int)Modalidade.Fundamental,
                anoTurma: "2",
                anoLetivo: DateTime.Now.Year - 1);

            repositorioElasticTurmaMock
                .Setup(x => x.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turma);

            var useCase = new ObterPermissaoTurmaUseCase(
                repositorioElasticTurmaMock.Object);

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
                useCase.ObterPermissaoTurma(123456, CancellationToken.None));

            Assert.Equal(
                "A Sondagem não se aplica para turmas deste ano letivo.",
                exception.Message);
        }

        [Fact]
        public async Task ObterPermissaoTurma_TurmaValida_DeveRetornarTrue()
        {
            var turma = CriarTurma(
                modalidade: (int)Modalidade.Fundamental,
                anoTurma: "3",
                anoLetivo: DateTime.Now.Year);

            repositorioElasticTurmaMock
                .Setup(x => x.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turma);

            var useCase = new ObterPermissaoTurmaUseCase(
                repositorioElasticTurmaMock.Object);

            var result = await useCase.ObterPermissaoTurma(123456, CancellationToken.None);

            Assert.True(result);
        }
    }
}
