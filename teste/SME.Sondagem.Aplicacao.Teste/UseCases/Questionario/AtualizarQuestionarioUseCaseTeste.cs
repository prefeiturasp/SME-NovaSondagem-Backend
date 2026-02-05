using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario
{
    public class AtualizarQuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestionario> _questionarioRepositorioMock;
        private readonly AtualizarQuestionarioUseCase _useCase;

        public AtualizarQuestionarioUseCaseTeste()
        {
            _questionarioRepositorioMock = new Mock<IRepositorioQuestionario>();
            _useCase = new AtualizarQuestionarioUseCase(_questionarioRepositorioMock.Object);
        }

        [Fact]
        public async Task Deve_Atualizar_Questionario_ComSucesso()
        {
            var questionarioId = 1;
            var questionarioExistente = new Dominio.Entidades.Questionario.Questionario(
                "Questionario Original",
                TipoQuestionario.SondagemLeitura,
                2024,
                1,
                1,
                1,
                1,
                1
            )
            {
                Id = questionarioId,
                CriadoEm = DateTime.UtcNow,
                CriadoPor = "Sistema",
                CriadoRF = "1234567"
            };

            var questionarioDto = new QuestionarioDto
            {
                Nome = "Questionario Atualizado",
                Tipo = TipoQuestionario.SondagemLeitura,
                AnoLetivo = 2024,
                ComponenteCurricularId = 2,
                ProficienciaId = 2,
                SondagemId = 1,
                ModalidadeId = 2,
                SerieAno = 2
            };

            _questionarioRepositorioMock
                .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(questionarioExistente);

            _questionarioRepositorioMock
                .Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Questionario.Questionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var resultado = await _useCase.ExecutarAsync(questionarioId, questionarioDto);

            Assert.NotNull(resultado);
            Assert.Equal(questionarioId, resultado.Id);
            Assert.Equal("Questionario Atualizado", resultado.Nome);
            Assert.Equal(2, resultado.ComponenteCurricularId);
            Assert.Equal(2, resultado.ProficienciaId);
            Assert.Equal(2, resultado.ModalidadeId);
            Assert.Equal(2, resultado.SerieAno);
            _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()), Times.Once);
            _questionarioRepositorioMock.Verify(x => x.SalvarAsync(questionarioExistente, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Null_Quando_Questionario_Nao_Encontrado()
        {
            var questionarioId = 999;
            var questionarioDto = new QuestionarioDto
            {
                Nome = "Questionario Teste",
                Tipo = TipoQuestionario.SondagemLeitura,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1,
                ModalidadeId = 1,
                SerieAno = 1
            };

            _questionarioRepositorioMock
                .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dominio.Entidades.Questionario.Questionario?)null);

            var resultado = await _useCase.ExecutarAsync(questionarioId, questionarioDto);

            Assert.Null(resultado);
            _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()), Times.Once);
            _questionarioRepositorioMock.Verify(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Questionario.Questionario>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Retornar_Null_Quando_Atualizacao_Falhar()
        {
            var questionarioId = 1;
            var questionarioExistente = new Dominio.Entidades.Questionario.Questionario(
                "Questionario Original",
                TipoQuestionario.SondagemLeitura,
                2024,
                1,
                1,
                1,
                1,
                1
            )
            {
                Id = questionarioId
            };

            var questionarioDto = new QuestionarioDto
            {
                Nome = "Questionario Atualizado",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1,
                ModalidadeId = 1,
                SerieAno = 1
            };

            _questionarioRepositorioMock
                .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(questionarioExistente);

            _questionarioRepositorioMock
                .Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Questionario.Questionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var resultado = await _useCase.ExecutarAsync(questionarioId, questionarioDto);

            Assert.Null(resultado);
            _questionarioRepositorioMock.Verify(x => x.SalvarAsync(questionarioExistente, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Passar_Cancellation_Token_Para_Repositorio()
        {
            var questionarioId = 1;
            var cancellationToken = new CancellationToken();
            var questionarioExistente = new Dominio.Entidades.Questionario.Questionario(
                "Questionario Original",
                TipoQuestionario.SondagemLeitura,
                2024,
                1,
                1,
                1,
                1,
                1
            )
            {
                Id = questionarioId
            };

            var questionarioDto = new QuestionarioDto
            {
                Nome = "Questionario Atualizado",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1,
                ModalidadeId = 1,
                SerieAno = 1
            };

            _questionarioRepositorioMock
                .Setup(x => x.ObterPorIdAsync(questionarioId, cancellationToken))
                .ReturnsAsync(questionarioExistente);

            _questionarioRepositorioMock
                .Setup(x => x.SalvarAsync(questionarioExistente, cancellationToken))
                .ReturnsAsync(1);

            await _useCase.ExecutarAsync(questionarioId, questionarioDto, cancellationToken);

            _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, cancellationToken), Times.Once);
            _questionarioRepositorioMock.Verify(x => x.SalvarAsync(questionarioExistente, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Deve_Manter_Tipo_Ano_Letivo_E_Sondagem_Id_Originais()
        {
            var questionarioId = 1;
            var questionarioExistente = new Dominio.Entidades.Questionario.Questionario(
                "Questionario Original",
                TipoQuestionario.SondagemEscrita,
                2024,
                1,
                1,
                5,
                1,
                1
            )
            {
                Id = questionarioId
            };

            var questionarioDto = new QuestionarioDto
            {
                Nome = "Questionario Atualizado",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2025,
                ComponenteCurricularId = 2,
                ProficienciaId = 2,
                SondagemId = 10,
                ModalidadeId = 2,
                SerieAno = 2
            };

            _questionarioRepositorioMock
                .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(questionarioExistente);

            _questionarioRepositorioMock
                .Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Questionario.Questionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var resultado = await _useCase.ExecutarAsync(questionarioId, questionarioDto);

            Assert.NotNull(resultado);
            Assert.Equal(TipoQuestionario.SondagemEscrita, resultado.Tipo); // Mant�m o original
            Assert.Equal(2024, resultado.AnoLetivo); // Mant�m o original
            Assert.Equal(5, resultado.SondagemId); // Mant�m o original
            Assert.Equal("Questionario Atualizado", resultado.Nome); // Atualiza
            Assert.Equal(2, resultado.ComponenteCurricularId); // Atualiza
            Assert.Equal(2, resultado.ProficienciaId); // Atualiza
            Assert.Equal(2, resultado.ModalidadeId); // Atualiza
            Assert.Equal(2, resultado.SerieAno); // Atualiza
        }

        [Fact]
        public async Task Deve_Retornar_Dto_Com_Todos_Os_Campos_Mapeados()
        {
            var questionarioId = 1;
            var dataAtual = DateTime.UtcNow;
            var questionarioExistente = new Dominio.Entidades.Questionario.Questionario(
                "Questionario Original",
                TipoQuestionario.SondagemEscrita,
                2024,
                1,
                1,
                1,
                1,
                1
            )
            {
                Id = questionarioId,
                CriadoEm = dataAtual,
                CriadoPor = "Usuario Criacao",
                CriadoRF = "1234567",
                AlteradoEm = dataAtual.AddDays(1),
                AlteradoPor = "Usuario Alteracao",
                AlteradoRF = "9876543"
            };

            var questionarioDto = new QuestionarioDto
            {
                Nome = "Questionario Atualizado",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2024,
                ComponenteCurricularId = 2,
                ProficienciaId = 2,
                SondagemId = 1,
                ModalidadeId = 2,
                SerieAno = 2
            };

            _questionarioRepositorioMock
                .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(questionarioExistente);

            _questionarioRepositorioMock
                .Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Questionario.Questionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var resultado = await _useCase.ExecutarAsync(questionarioId, questionarioDto);

            Assert.NotNull(resultado);
            Assert.Equal(questionarioId, resultado.Id);
            Assert.Equal("Questionario Atualizado", resultado.Nome);
            Assert.Equal(TipoQuestionario.SondagemEscrita, resultado.Tipo);
            Assert.Equal(2024, resultado.AnoLetivo);
            Assert.Equal(2, resultado.ComponenteCurricularId);
            Assert.Equal(2, resultado.ProficienciaId);
            Assert.Equal(1, resultado.SondagemId);
            Assert.Equal(2, resultado.ModalidadeId);
            Assert.Equal(2, resultado.SerieAno);
            Assert.Equal(dataAtual, resultado.CriadoEm);
            Assert.Equal("Usuario Criacao", resultado.CriadoPor);
            Assert.Equal("1234567", resultado.CriadoRF);
            Assert.Equal(dataAtual.AddDays(1), resultado.AlteradoEm);
            Assert.Equal("Usuario Alteracao", resultado.AlteradoPor);
            Assert.Equal("9876543", resultado.AlteradoRF);
        }
    }
}
