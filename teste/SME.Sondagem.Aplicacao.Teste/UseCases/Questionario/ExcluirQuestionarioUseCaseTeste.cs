using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario;

public class ExcluirQuestionarioUseCaseTeste
{
    private readonly Mock<IRepositorioQuestionario> _questionarioRepositorioMock;
    private readonly ExcluirQuestionarioUseCase _useCase;

    public ExcluirQuestionarioUseCaseTeste()
    {
        _questionarioRepositorioMock = new Mock<IRepositorioQuestionario>();
        _useCase = new ExcluirQuestionarioUseCase(_questionarioRepositorioMock.Object);
    }

    [Fact]
    public async Task Deve_Excluir_Questionario_Com_Sucesso()
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
            Id = questionarioId
        };

        _questionarioRepositorioMock
            .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(questionario);

        _questionarioRepositorioMock
            .Setup(x => x.RemoverLogico(questionarioId, null,It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(questionarioId);

        Assert.True(resultado);
        _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()), Times.Once);
        _questionarioRepositorioMock.Verify(x => x.RemoverLogico(questionarioId, null,It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Retornar_False_Quando_Questionario_Nao_Encontrado()
    {
        var questionarioId = 999;

        _questionarioRepositorioMock
            .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dominio.Entidades.Questionario.Questionario?)null);

        var resultado = await _useCase.ExecutarAsync(questionarioId);

        Assert.False(resultado);
        _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()), Times.Once);
        _questionarioRepositorioMock.Verify(x => x.RemoverLogico(It.IsAny<long>(),null, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Retornar_False_Quando_Exclusao_Falhar()
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
            Id = questionarioId
        };

        _questionarioRepositorioMock
            .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(questionario);

        _questionarioRepositorioMock
            .Setup(x => x.RemoverLogico(questionarioId, null,It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var resultado = await _useCase.ExecutarAsync(questionarioId);

        Assert.False(resultado);
        _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()), Times.Once);
        _questionarioRepositorioMock.Verify(x => x.RemoverLogico(questionarioId, null,It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Passar_Cancellation_Token_Para_Repositorio()
    {
        var questionarioId = 1;
        var cancellationToken = new CancellationToken();
        var questionario = new Dominio.Entidades.Questionario.Questionario(
            "Questionário Teste",
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

        _questionarioRepositorioMock
            .Setup(x => x.ObterPorIdAsync(questionarioId, cancellationToken))
            .ReturnsAsync(questionario);

        _questionarioRepositorioMock
            .Setup(x => x.RemoverLogico(questionarioId, null,cancellationToken))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(questionarioId, cancellationToken);

        _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, cancellationToken), Times.Once);
        _questionarioRepositorioMock.Verify(x => x.RemoverLogico(questionarioId, null,cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Deve_Validar_Existencia_Antes_De_Excluir()
    {
        var questionarioId = 1;

        _questionarioRepositorioMock
            .Setup(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dominio.Entidades.Questionario.Questionario?)null);

        var resultado = await _useCase.ExecutarAsync(questionarioId);

        Assert.False(resultado);
        _questionarioRepositorioMock.Verify(x => x.ObterPorIdAsync(questionarioId, It.IsAny<CancellationToken>()), Times.Once);
        _questionarioRepositorioMock.Verify(x => x.RemoverLogico(questionarioId, null,It.IsAny<CancellationToken>()), Times.Never, 
            "Não deve tentar excluir se o questionário não existe");
    }
}
