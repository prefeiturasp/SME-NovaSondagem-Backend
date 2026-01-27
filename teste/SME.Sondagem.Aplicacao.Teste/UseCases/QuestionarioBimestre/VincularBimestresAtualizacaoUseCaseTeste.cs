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
    public class VincularBimestresAtualizacaoUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestionarioBimestre> _repositorioMock;
        private readonly Mock<IRepositorioQuestionario> _repositorioQuestionarioMock;
        private readonly Mock<IRepositorioBimestre> _repositorioBimestreMock;
        private readonly Mock<IValidator<VincularBimestresDto>> _validatorVincularMock;
        private readonly Mock<IValidator<AtualizarVinculosBimestresDto>> _validatorAtualizarMock;
        private readonly VincularBimestresUseCase _useCase;

        public VincularBimestresAtualizacaoUseCaseTeste()
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
        public async Task ExecutarAtualizacaoAsync_Deve_Atualizar_Vinculos_Com_Sucesso()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1, 2, 3 }
            };

            _validatorAtualizarMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
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

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(4L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(4, "Bimestre 4"));

            var vinculosAtuais = new List<Dominio.Entidades.Questionario.QuestionarioBimestre>
            {
                new Dominio.Entidades.Questionario.QuestionarioBimestre(10, 1),
                new Dominio.Entidades.Questionario.QuestionarioBimestre(10, 4)
            };

            _repositorioMock
                .Setup(x => x.ObterPorQuestionarioIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculosAtuais);

            _repositorioMock
                .Setup(x => x.ExcluirPorQuestionarioEBimestreAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _repositorioMock
                .Setup(x => x.CriarMultiplosAsync(It.IsAny<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await _useCase.ExecutarAtualizacaoAsync(dto);

            Assert.True(resultado);
        }

        [Fact]
        public async Task ExecutarAtualizacaoAsync_Deve_Remover_Todos_Vinculos_Quando_Lista_Vazia()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int>()
            };

            _validatorAtualizarMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioMock
                .Setup(x => x.ExcluirPorQuestionarioIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await _useCase.ExecutarAtualizacaoAsync(dto);

            Assert.True(resultado);
            _repositorioMock.Verify(x => x.ExcluirPorQuestionarioIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecutarAtualizacaoAsync_Deve_Remover_Todos_Vinculos_Quando_Lista_Null()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = null!
            };

            _validatorAtualizarMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioMock
                .Setup(x => x.ExcluirPorQuestionarioIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await _useCase.ExecutarAtualizacaoAsync(dto);

            Assert.True(resultado);
            _repositorioMock.Verify(x => x.ExcluirPorQuestionarioIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecutarAtualizacaoAsync_Deve_Lancar_ValidationException_Quando_Dto_Invalido()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = null,
                BimestreIds = new List<int> { 1 }
            };

            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("QuestionarioId", "ID é obrigatório")
            };

            _validatorAtualizarMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            await Assert.ThrowsAsync<ValidationException>(() => _useCase.ExecutarAtualizacaoAsync(dto));
        }

        [Fact]
        public async Task ExecutarAtualizacaoAsync_Deve_Lancar_RegraNegocioException_Quando_Questionario_Nao_Existe()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 999,
                BimestreIds = new List<int> { 1 }
            };

            _validatorAtualizarMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.Questionario.Questionario?)null);

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() => _useCase.ExecutarAtualizacaoAsync(dto));
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)exception.StatusCode);
        }

        [Fact]
        public async Task ExecutarAtualizacaoAsync_Deve_Lancar_RegraNegocioException_Quando_Bimestre_Nao_Existe()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 999 }
            };

            _validatorAtualizarMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(999L, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.Bimestre?)null);

            var exception = await Assert.ThrowsAsync<RegraNegocioException>(() => _useCase.ExecutarAtualizacaoAsync(dto));
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)exception.StatusCode);
        }

        [Fact]
        public async Task ExecutarAtualizacaoAsync_Deve_Adicionar_Novos_Bimestres()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1, 2, 3 }
            };

            _validatorAtualizarMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
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

            var vinculosAtuais = new List<Dominio.Entidades.Questionario.QuestionarioBimestre>
            {
                new Dominio.Entidades.Questionario.QuestionarioBimestre(10, 1)
            };

            _repositorioMock
                .Setup(x => x.ObterPorQuestionarioIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculosAtuais);

            _repositorioMock
                .Setup(x => x.CriarMultiplosAsync(It.IsAny<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _useCase.ExecutarAtualizacaoAsync(dto);

            _repositorioMock.Verify(x => x.CriarMultiplosAsync(
                It.Is<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(l =>
                    l.Count == 2 &&
                    l.Any(v => v.BimestreId == 2) &&
                    l.Any(v => v.BimestreId == 3)),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecutarAtualizacaoAsync_Deve_Remover_Bimestres_Nao_Presentes_Na_Lista()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1 }
            };

            _validatorAtualizarMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(1L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(1, "Bimestre 1"));

            var vinculosAtuais = new List<Dominio.Entidades.Questionario.QuestionarioBimestre>
            {
                new Dominio.Entidades.Questionario.QuestionarioBimestre(10, 1),
                new Dominio.Entidades.Questionario.QuestionarioBimestre(10, 2),
                new Dominio.Entidades.Questionario.QuestionarioBimestre(10, 3)
            };

            _repositorioMock
                .Setup(x => x.ObterPorQuestionarioIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculosAtuais);

            _repositorioMock
                .Setup(x => x.ExcluirPorQuestionarioEBimestreAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _useCase.ExecutarAtualizacaoAsync(dto);

            _repositorioMock.Verify(x => x.ExcluirPorQuestionarioEBimestreAsync(10, 2, It.IsAny<CancellationToken>()), Times.Once);
            _repositorioMock.Verify(x => x.ExcluirPorQuestionarioEBimestreAsync(10, 3, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecutarAtualizacaoAsync_Deve_Manter_Bimestres_Existentes_Na_Lista()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1, 2 }
            };

            _validatorAtualizarMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarQuestionarioMock());

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(1L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(1, "Bimestre 1"));

            _repositorioBimestreMock
                .Setup(x => x.ObterPorIdAsync(2L, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarBimestreMock(2, "Bimestre 2"));

            var vinculosAtuais = new List<Dominio.Entidades.Questionario.QuestionarioBimestre>
            {
                new Dominio.Entidades.Questionario.QuestionarioBimestre(10, 1),
                new Dominio.Entidades.Questionario.QuestionarioBimestre(10, 2)
            };

            _repositorioMock
                .Setup(x => x.ObterPorQuestionarioIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculosAtuais);

            await _useCase.ExecutarAtualizacaoAsync(dto);

            _repositorioMock.Verify(x => x.ExcluirPorQuestionarioEBimestreAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _repositorioMock.Verify(x => x.CriarMultiplosAsync(It.IsAny<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecutarAtualizacaoAsync_Deve_Remover_Duplicados_Da_Lista()
        {
            var dto = new AtualizarVinculosBimestresDto
            {
                QuestionarioId = 10,
                BimestreIds = new List<int> { 1, 1, 2, 2, 3 }
            };

            _validatorAtualizarMock
                .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositorioQuestionarioMock
                .Setup(x => x.ObterPorIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
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
                .Setup(x => x.ObterPorQuestionarioIdAsync(dto.QuestionarioId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Dominio.Entidades.Questionario.QuestionarioBimestre>());

            _repositorioMock
                .Setup(x => x.CriarMultiplosAsync(It.IsAny<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _useCase.ExecutarAtualizacaoAsync(dto);

            _repositorioMock.Verify(x => x.CriarMultiplosAsync(
                It.Is<List<Dominio.Entidades.Questionario.QuestionarioBimestre>>(l => l.Count == 3),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}