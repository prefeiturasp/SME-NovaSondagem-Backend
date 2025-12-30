using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questao;

public class ObterQuestaoPorIdUseCaseTeste
{
    private readonly Mock<IRepositorioQuestao> _repositorioQuestaoMock;
    private readonly ObterQuestaoPorIdUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ObterQuestaoPorIdUseCaseTeste()
    {
        _repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        _useCase = new ObterQuestaoPorIdUseCase(_repositorioQuestaoMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_QuestaoExiste_DeveRetornarQuestaoDto()
    {
        const long id = 1;
        var questao = new SME.Sondagem.Dominio.Entidades.Questionario.Questao(
            questionarioId: 1,
            grupoQuestoesId: 2,
            ordem: 1,
            nome: "Questao Teste",
            observacao: "Observacao teste",
            obrigatorio: true,
            tipo: TipoQuestao.Texto,
            opcionais: "{}",
            somenteLeitura: false,
            dimensao: 100,
            tamanho: 50,
            mascara: null,
            placeHolder: "Digite aqui",
            nomeComponente: "input-text")
        {
            Id = (int)id,
            CriadoEm = DateTime.Now,
            CriadoPor = "Usuario1",
            CriadoRF = "RF001",
            AlteradoEm = DateTime.Now.AddHours(1),
            AlteradoPor = "Usuario2",
            AlteradoRF = "RF002"
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: _cancellationToken))
            .ReturnsAsync(questao);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Questao Teste", resultado.Nome);
        Assert.Equal(2, resultado.GrupoQuestoesId);
        Assert.Equal(1, resultado.QuestionarioId);
        Assert.Equal(1, resultado.Ordem);
        Assert.Equal("Observacao teste", resultado.Observacao);
        Assert.True(resultado.Obrigatorio);
        Assert.Equal(TipoQuestao.Texto, resultado.Tipo);
        Assert.Equal("Usuario1", resultado.CriadoPor);
        Assert.Equal("RF001", resultado.CriadoRF);
        Assert.Equal(questao.CriadoEm, resultado.CriadoEm);
        Assert.Equal(questao.AlteradoEm, resultado.AlteradoEm);
        Assert.Equal("Usuario2", resultado.AlteradoPor);
        Assert.Equal("RF002", resultado.AlteradoRF);

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuestaoNaoExiste_DeveRetornarNull()
    {
        const long id = 999;

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.Questao?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.Null(resultado);

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComParametrosCorretos()
    {
        const long id = 42;
        var cancellationTokenCustom = new CancellationToken(false);

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: cancellationTokenCustom))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.Questao?)null);

        await _useCase.ExecutarAsync(id, cancellationTokenCustom);

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: cancellationTokenCancelado), Times.Once);
    }
}