using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario;

public class ObterQuestionarioPorIdUseCaseTeste
{
    private readonly Mock<IRepositorioQuestionario> _questionarioRepositorioMock;
    private readonly ObterQuestionarioPorIdUseCase _useCase;

    public ObterQuestionarioPorIdUseCaseTeste()
    {
        _questionarioRepositorioMock = new Mock<IRepositorioQuestionario>();
        _useCase = new ObterQuestionarioPorIdUseCase(_questionarioRepositorioMock.Object);
    }

    [Fact]
    public async Task Deve_Retornar_Questionario_Quando_Encontrado()
    {
        var questionarioId = 1;
        var questionario = new Dominio.Entidades.Questionario.Questionario(
            "Questionário Teste",
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
            CriadoEm = DateTime.UtcNow,
            CriadoPor = "Sistema",
            CriadoRF = "1234567"
        };

        _questionarioRepositorioMock
            .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(questionario);

        var resultado = await _useCase.ExecutarAsync(questionarioId);

        Assert.NotNull(resultado);
        Assert.Equal(questionarioId, resultado.Id);
        Assert.Equal("Questionário Teste", resultado.Nome);
        Assert.Equal(TipoQuestionario.SondagemEscrita, resultado.Tipo);
        Assert.Equal(2024, resultado.AnoLetivo);
        _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Retornar_Null_Quando_Questionario_Nao_Encontrado()
    {
        var questionarioId = 999L;

        _questionarioRepositorioMock
            .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dominio.Entidades.Questionario.Questionario?)null);

        var resultado = await _useCase.ExecutarAsync(questionarioId);

        Assert.Null(resultado);
        _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Passar_Cancellation_Token_Para_Repositorio()
    {
        var questionarioId = 1L;
        var cancellationToken = new CancellationToken();

        _questionarioRepositorioMock
            .Setup(x => x.ObterPorIdAsync(questionarioId, cancellationToken))
            .ReturnsAsync((Dominio.Entidades.Questionario.Questionario?)null);

        await _useCase.ExecutarAsync(questionarioId, cancellationToken);

        _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Deve_Mapear_Todos_Os_Campos_Do_Questionario()
    {
        var questionarioId = 1;
        var dataAtual = DateTime.UtcNow;
        var questionario = new Dominio.Entidades.Questionario.Questionario(
            "Questionário Completo",
            TipoQuestionario.SondagemEscrita,
            2024,
            2,
            3,
            4,
            5,
            6
        )
        {
            Id = questionarioId,
            CriadoEm = dataAtual,
            CriadoPor = "Usuário Teste",
            CriadoRF = "9876543",
            AlteradoEm = dataAtual.AddDays(1),
            AlteradoPor = "Usuário Alteração",
            AlteradoRF = "1111111"
        };

        _questionarioRepositorioMock
            .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(questionario);

        var resultado = await _useCase.ExecutarAsync(questionarioId);

        Assert.NotNull(resultado);
        Assert.Equal(questionarioId, resultado.Id);
        Assert.Equal("Questionário Completo", resultado.Nome);
        Assert.Equal(TipoQuestionario.SondagemEscrita, resultado.Tipo);
        Assert.Equal(2024, resultado.AnoLetivo);
        Assert.Equal(2, resultado.ComponenteCurricularId);
        Assert.Equal(3, resultado.ProficienciaId);
        Assert.Equal(4, resultado.SondagemId);
        Assert.Equal(5, resultado.ModalidadeId);
        Assert.Equal(6, resultado.SerieAno);
        Assert.Equal(dataAtual, resultado.CriadoEm);
        Assert.Equal("Usuário Teste", resultado.CriadoPor);
        Assert.Equal("9876543", resultado.CriadoRF);
        Assert.Equal(dataAtual.AddDays(1), resultado.AlteradoEm);
        Assert.Equal("Usuário Alteração", resultado.AlteradoPor);
        Assert.Equal("1111111", resultado.AlteradoRF);
    }
}