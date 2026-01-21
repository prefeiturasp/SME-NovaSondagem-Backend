using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario
{
    public class CriarQuestionarioUseCaseTeste
    {
        private readonly Mock<IRepositorioQuestionario> _questionarioRepositorioMock;
        private readonly CriarQuestionarioUseCase _useCase;

        public CriarQuestionarioUseCaseTeste()
        {
            _questionarioRepositorioMock = new Mock<IRepositorioQuestionario>();
            _useCase = new CriarQuestionarioUseCase(_questionarioRepositorioMock.Object);
        }

        [Fact]
        public async Task Deve_Criar_Questionario_Com_Sucesso()
        {
            var questionarioId = 1L;
            var questionarioDto = new QuestionarioDto
            {
                Nome = "Question�rio Teste",
                Tipo = TipoQuestionario.SondagemLeitura,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1,
                ModalidadeId = 1,
                SerieAno = 1,
                CriadoEm = DateTime.UtcNow,
                CriadoPor = "Sistema",
                CriadoRF = "1234567"
            };

            _questionarioRepositorioMock
                .Setup(x => x.CriarAsync(It.IsAny<Dominio.Entidades.Questionario.Questionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(questionarioId);

            var resultado = await _useCase.ExecutarAsync(questionarioDto);

            Assert.Equal(questionarioId, resultado);
            _questionarioRepositorioMock.Verify(x => x.CriarAsync(
                It.Is<Dominio.Entidades.Questionario.Questionario>(q =>
                    q.Nome == questionarioDto.Nome &&
                    q.Tipo == questionarioDto.Tipo &&
                    q.AnoLetivo == questionarioDto.AnoLetivo &&
                    q.ComponenteCurricularId == questionarioDto.ComponenteCurricularId &&
                    q.ProficienciaId == questionarioDto.ProficienciaId &&
                    q.SondagemId == questionarioDto.SondagemId &&
                    q.ModalidadeId == questionarioDto.ModalidadeId &&
                    q.SerieAno == questionarioDto.SerieAno),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Passar_Cancellation_Token_Para_Repositorio()
        {
            var cancellationToken = new CancellationToken();
            var questionarioDto = new QuestionarioDto
            {
                Nome = "Question�rio Teste",
                Tipo = TipoQuestionario.SondagemEscrita,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1,
                ModalidadeId = 1,
                SerieAno = 1
            };

            _questionarioRepositorioMock
                .Setup(x => x.CriarAsync(It.IsAny<Dominio.Entidades.Questionario.Questionario>(), cancellationToken))
                .ReturnsAsync(1L);

            await _useCase.ExecutarAsync(questionarioDto, cancellationToken);

            _questionarioRepositorioMock.Verify(x => x.CriarAsync(
                It.IsAny<Dominio.Entidades.Questionario.Questionario>(),
                cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Deve_Mapear_Todos_Os_Campos_Do_Dto()
        {
            var dataAtual = DateTime.UtcNow;
            var questionarioDto = new QuestionarioDto
            {
                Nome = "Questionario Completo",
                Tipo = TipoQuestionario.SondagemLeitura,
                AnoLetivo = 2024,
                ComponenteCurricularId = 2,
                ProficienciaId = 3,
                SondagemId = 4,
                ModalidadeId = 5,
                SerieAno = 6,
                CriadoEm = dataAtual,
                CriadoPor = "Usuario Teste",
                CriadoRF = "9876543",
                AlteradoEm = dataAtual.AddDays(1),
                AlteradoPor = "Usuario Alteracao",
                AlteradoRF = "1111111"
            };

            _questionarioRepositorioMock
                .Setup(x => x.CriarAsync(It.IsAny<Dominio.Entidades.Questionario.Questionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1L);

            await _useCase.ExecutarAsync(questionarioDto);

            _questionarioRepositorioMock.Verify(x => x.CriarAsync(
                It.Is<Dominio.Entidades.Questionario.Questionario>(q =>
                    q.Nome == questionarioDto.Nome &&
                    q.Tipo == questionarioDto.Tipo &&
                    q.AnoLetivo == questionarioDto.AnoLetivo &&
                    q.ComponenteCurricularId == questionarioDto.ComponenteCurricularId &&
                    q.ProficienciaId == questionarioDto.ProficienciaId &&
                    q.SondagemId == questionarioDto.SondagemId &&
                    q.ModalidadeId == questionarioDto.ModalidadeId &&
                    q.SerieAno == questionarioDto.SerieAno &&
                    q.CriadoEm == questionarioDto.CriadoEm &&
                    q.CriadoPor == questionarioDto.CriadoPor &&
                    q.CriadoRF == questionarioDto.CriadoRF &&
                    q.AlteradoEm == questionarioDto.AlteradoEm &&
                    q.AlteradoPor == questionarioDto.AlteradoPor &&
                    q.AlteradoRF == questionarioDto.AlteradoRF),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Criar_Questionario_Com_Modalidade_Nula()
        {
            var questionarioDto = new QuestionarioDto
            {
                Nome = "Questionario Teste",
                Tipo = TipoQuestionario.SondagemLeitura,
                AnoLetivo = 2024,
                ComponenteCurricularId = 1,
                ProficienciaId = 1,
                SondagemId = 1,
                ModalidadeId = null,
                SerieAno = null
            };

            _questionarioRepositorioMock
                .Setup(x => x.CriarAsync(It.IsAny<Dominio.Entidades.Questionario.Questionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1L);

            await _useCase.ExecutarAsync(questionarioDto);

            _questionarioRepositorioMock.Verify(x => x.CriarAsync(
                It.Is<Dominio.Entidades.Questionario.Questionario>(q =>
                    q.ModalidadeId == null &&
                    q.SerieAno == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
