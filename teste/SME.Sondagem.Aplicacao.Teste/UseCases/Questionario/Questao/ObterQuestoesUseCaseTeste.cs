using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questao;

public class ObterQuestoesUseCaseTeste
{
    private readonly Mock<IRepositorioQuestao> _repositorioQuestaoMock;
    private readonly ObterQuestoesUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ObterQuestoesUseCaseTeste()
    {
        _repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        _useCase = new ObterQuestoesUseCase(_repositorioQuestaoMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarListaDeQuestoesDto()
    {
        var questoes = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao>
        {
            new(
                questionarioId: 1,
                ordem: 1,
                nome: "Questao 1",
                observacao: "Observacao 1",
                obrigatorio: true,
                tipo: TipoQuestao.Texto,
                opcionais: "",
                somenteLeitura: false,
                dimensao: 1,
                grupoQuestoesId: null
            ),
            new(
                questionarioId: 2,
                ordem: 2,
                nome: "Questao 2",
                observacao: "Observacao 2",
                obrigatorio: false,
                tipo: TipoQuestao.Radio,
                opcionais: "",
                somenteLeitura: false,
                dimensao: 1,
                grupoQuestoesId: null
            )
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(questoes);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        var resultadoList = resultado.ToList();
        Assert.Equal(2, resultadoList.Count);

        var primeira = resultadoList.First(x => x.Nome == "Questao 1");
        Assert.Equal("Questao 1", primeira.Nome);
        Assert.Equal(1, primeira.QuestionarioId);

        var segunda = resultadoList.First(x => x.Nome == "Questao 2");
        Assert.Equal("Questao 2", segunda.Nome);
        Assert.Equal(2, segunda.QuestionarioId);

        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoNaoHaQuestoes_DeveRetornarListaVazia()
    {
        var questoesVazias = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao>();

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(questoesVazias);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        Assert.Empty(resultado);

        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(cancellationTokenCancelado));

        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComCancellationTokenCorreto()
    {
        var cancellationTokenCustom = new CancellationTokenSource().Token;
        var questoes = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao>();

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(cancellationTokenCustom))
            .ReturnsAsync(questoes);

        await _useCase.ExecutarAsync(cancellationTokenCustom);

        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(_cancellationToken));

        Assert.Equal("Erro do repositório", exception.Message);
        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveMaperarTodasAsPropriedadesCorretamente()
    {
        var questoes = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao>
        {
            new(
                questionarioId: 100,
                ordem: 3,
                nome: "Matemática Básica",
                observacao: "Observação detalhada",
                obrigatorio: true,
                tipo: TipoQuestao.Numerico,
                opcionais: "opcionais",
                somenteLeitura: false,
                dimensao: 2,
                grupoQuestoesId: 1,
                tamanho: 50,
                mascara: "###.###",
                placeHolder: "Digite aqui",
                nomeComponente: "ComponenteX"
            )
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(questoes);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var dto = resultado.Single();
        Assert.Equal("Matemática Básica", dto.Nome);
        Assert.Equal(100, dto.QuestionarioId);
        Assert.Equal(1, dto.GrupoQuestoesId);
        Assert.Equal(3, dto.Ordem);
        Assert.Equal("Observação detalhada", dto.Observacao);
        Assert.True(dto.Obrigatorio);
        Assert.Equal(TipoQuestao.Numerico, dto.Tipo);
        Assert.Equal("opcionais", dto.Opcionais);
        Assert.False(dto.SomenteLeitura);
        Assert.Equal(2, dto.Dimensao);
        Assert.Equal(50, dto.Tamanho);
        Assert.Equal("###.###", dto.Mascara);
        Assert.Equal("Digite aqui", dto.PlaceHolder);
        Assert.Equal("ComponenteX", dto.NomeComponente);
    }

    [Fact]
    public async Task ExecutarAsync_ComMuitasQuestoes_DeveMantarPerformance()
    {
        var questoes = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao>();

        for (int i = 1; i <= 1000; i++)
        {
            questoes.Add(new SME.Sondagem.Dominio.Entidades.Questionario.Questao(
                questionarioId: i % 5 + 1,
                ordem: i,
                nome: $"Questao {i}",
                observacao: $"Observacao {i}",
                obrigatorio: i % 2 == 0,
                tipo: TipoQuestao.Texto,
                opcionais: "",
                somenteLeitura: false,
                dimensao: 1,
                grupoQuestoesId: null
            ));
        }

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(questoes);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var resultadoList = resultado.ToList();
        Assert.Equal(1000, resultadoList.Count);
        Assert.Equal("Questao 1", resultadoList.First().Nome);
        Assert.Equal("Questao 1000", resultadoList.Last().Nome);

        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveMapearPropriedadesDeAuditoria()
    {
        var dataEspecifica = new DateTime(2023, 10, 15, 14, 30, 0);

        var questao = new SME.Sondagem.Dominio.Entidades.Questionario.Questao(
            questionarioId: 1,
            ordem: 1,
            nome: "Questao Teste",
            observacao: "Teste",
            obrigatorio: true,
            tipo: TipoQuestao.Texto,
            opcionais: "",
            somenteLeitura: false,
            dimensao: 1,
            grupoQuestoesId: null
        );

        var questoes = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao> { questao };

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(questoes);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var dto = resultado.Single();
        Assert.NotNull(dto.CriadoPor);
        Assert.NotNull(dto.CriadoRF);
    }
}