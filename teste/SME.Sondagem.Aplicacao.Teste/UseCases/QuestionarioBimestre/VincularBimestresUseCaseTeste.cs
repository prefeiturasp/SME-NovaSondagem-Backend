using FluentValidation;
using FluentValidation.Results;
using Moq;
using SME.Sondagem.Aplicacao.UseCases.QuestionarioBimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;
using System.Net;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.QuestionarioBimestre
{
    public class VincularBimestresUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestionarioBimestre> _repositorioMock;
        private readonly Mock<IRepositorioQuestionario> _repositorioQuestionarioMock;
        private readonly Mock<IRepositorioBimestre> _repositorioBimestreMock;
        private readonly Mock<IValidator<VincularBimestresDto>> _validatorVincularMock;
        private readonly Mock<IValidator<AtualizarVinculosBimestresDto>> _validatorAtualizarMock;
        private readonly VincularBimestresUseCase _useCase;

        public VincularBimestresUseCaseTeste()
        {
            _repositorioMock = new Mock<IRepositorioQuestionarioBimestre>();
            _repositorioQuestionarioMock = new Mock<IRepositorioQuestionario>();
            _repositorioBimestreMock = new Mock<IRepositorioBimestre>();
            _validatorVincularMock = new Mock<IValidator<VincularBimestresDto>>();
            _validatorAtualizarMock = new Mock<IValidator<AtualizarVinculosBimestresDto>>();

            _useCase = new VincularBimestresUseCase(
                _repositorioMock.Object,
                _repositorioQuestionarioMock.Object,
                _repositorioBimestreMock.Object,
                _validatorVincularMock.Object,
                _validatorAtualizarMock.Object);
        }

        private static Dominio.Entidades.Questionario.Questionario CriarQuestionarioMock()
        {
            return new Dominio.Entidades.Questionario.Questionario(
                nome: "Questionário Teste",
                tipo: TipoQuestionario.SondagemEscrita,
                anoLetivo: 2024,
                componenteCurricularId: 1,
                proficienciaId: 1,
                sondagemId: 1,
                modalidadeId: 1,
                serieAno: 1
            );
        }

        private static Dominio.Entidades.Bimestre CriarBimestreMock(int codEol, string descricao)
        {
            return new Dominio.Entidades.Bimestre(codEol, descricao);
        }

        [Fact]
        public async Task ExecutarAsync_Deve_Vincular_Bimestres_Com_Sucesso()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1, 2 }
            };

            _validatorVincularMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(1L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(1, "Bimestre 1"));

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(2L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(2, "Bimestre 2"));

            _repositorioMock
                .Setup(x => x.ExisteVinculoAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repositorioMock
                .Setup(x => x.CriarMultiplosAsync(It.IsAny<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await _useCase.ExecutarAsync(dto);

            Assert.True(resultado);
            _repositorioMock.Verify(x => x.CriarMultiplosAsync(
                It.Is<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(l => l.Count == 2),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_Deve_Lancar_ValidationException_Quando_Dto_Invalido()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 0,
                BimestreIds = new List<int>()
            };

            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("QuestionarioId", "ID inválido")
            };

            _validatorVincularMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            await Assert.ThrowsAsync<ValidationException>(() => _useCase.ExecutarAsync(dto));
        }

        [Fact]
        public async Task ExecutarAsync_Deve_Lancar_RegraNegocioException_Quando_Questionario_Nao_Existe()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 999,
                BimestreIds = new List<int> { 1 }
            };

            _validatorVincularMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.Questionario.Questionario?)null);

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() => _useCase.ExecutarAsync(dto));
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)exception.StatusCode);
        }

        [Fact]
        public async Task ExecutarAsync_Deve_Lancar_RegraNegocioException_Quando_Bimestre_Nao_Existe()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 999 }
            };

            _validatorVincularMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(999L, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.Bimestre?)null);

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() => _useCase.ExecutarAsync(dto));
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)exception.StatusCode);
        }

        [Fact]
        public async Task ExecutarAsync_Deve_Ignorar_Bimestres_Ja_Vinculados()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1, 2, 3 }
            };

            _validatorVincularMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(1L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(1, "Bimestre 1"));

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(2L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(2, "Bimestre 2"));

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(3L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(3, "Bimestre 3"));

            _repositorioMock
                .Setup(x => x.ExisteVinculoAsync(dto.QuestionarioId, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _repositorioMock
                .Setup(x => x.ExisteVinculoAsync(dto.QuestionarioId, It.IsIn(1, 3), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repositorioMock
                .Setup(x => x.CriarMultiplosAsync(It.IsAny<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await _useCase.ExecutarAsync(dto);

            Assert.True(resultado);
            _repositorioMock.Verify(x => x.CriarMultiplosAsync(
                It.Is<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(l => l.Count == 2),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_Deve_Lancar_RegraNegocioException_Quando_Todos_Bimestres_Ja_Vinculados()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1, 2 }
            };

            _validatorVincularMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(1L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(1, "Bimestre 1"));

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(2L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(2, "Bimestre 2"));

            _repositorioMock
                .Setup(x => x.ExisteVinculoAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() => _useCase.ExecutarAsync(dto));
            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)exception.StatusCode);
            Assert.Contains("já estão vinculados", exception.Message);
        }

        [Fact]
        public async Task ExecutarAsync_Deve_Remover_Bimestres_Duplicados_Da_Lista()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1, 1, 2, 2, 3 }
            };

            _validatorVincularMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(1L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(1, "Bimestre 1"));

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(2L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(2, "Bimestre 2"));

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(3L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(3, "Bimestre 3"));

            _repositorioMock
                .Setup(x => x.ExisteVinculoAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _repositorioMock
                .Setup(x => x.CriarMultiplosAsync(It.IsAny<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _useCase.ExecutarAsync(dto);

            _repositorioMock.Verify(x => x.CriarMultiplosAsync(
                It.Is<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(l => l.Count == 3),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_Deve_Passar_CancellationToken_Para_Repositorios()
        {
            var dto = new VincularBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1 }
            };
            var cancellationToken = new CancellationToken();

            _validatorVincularMock
                .Setup(x => x.ValidateAsync(dto, cancellationToken))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId, cancellationToken))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(1L, cancellationToken))
                .ReturnsAsync(CriarBimestreMock(1, "Bimestre 1"));

            _repositorioMock
                .Setup(x => x.ExisteVinculoAsync(dto.QuestionarioId, 1, cancellationToken))
                .ReturnsAsync(false);

            _repositorioMock
                .Setup(x => x.CriarMultiplosAsync(It.IsAny<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(), cancellationToken))
                .ReturnsAsync(true);

            await _useCase.ExecutarAsync(dto, cancellationToken);

            _repositorioQuestionarioMock.Verify(x => x.ObterPorIdAsync(dto.QuestionarioId, cancellationToken), Times.Once);
            _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(1L, cancellationToken), Times.Once);
            _repositorioMock.Verify(x => x.CriarMultiplosAsync(It.IsAny<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(), cancellationToken), Times.Once);
        }
    }
}