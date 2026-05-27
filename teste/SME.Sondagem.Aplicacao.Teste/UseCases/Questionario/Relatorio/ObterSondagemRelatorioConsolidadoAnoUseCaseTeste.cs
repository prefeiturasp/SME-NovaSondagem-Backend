using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoAnoUseCaseTeste
{
    private readonly Mock<IRepositorioRespostaAluno> _mockRespostaAluno;
    private readonly Mock<IAbrangenciaService> _mockAbrangencia;
    private readonly ObterSondagemRelatorioConsolidadoAnoUseCase _useCase;

    public ObterSondagemRelatorioConsolidadoAnoUseCaseTeste()
    {
        _mockRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        _mockAbrangencia = new Mock<IAbrangenciaService>();
        _mockAbrangencia
            .Setup(x => x.ObterAbrangenciaCompletaAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<string>(), new List<string>(), new List<string>()));

        var repos = new RepositoriosSondagem(
            new Mock<IRepositorioSondagem>().Object,
            new Mock<IRepositorioQuestao>().Object,
            _mockRespostaAluno.Object,
            new Mock<IRepositorioBimestre>().Object,
            new Mock<IRepositorioComponenteCurricular>().Object,
            new Mock<IRepositorioProficiencia>().Object,
            new Mock<IRepositorioRacaCor>().Object,
            new Mock<IRepositorioGeneroSexo>().Object
        );

        _useCase = new ObterSondagemRelatorioConsolidadoAnoUseCase(
            repos,
            new Mock<IRepositorioElasticTurma>().Object,
            _mockAbrangencia.Object);
    }

    private static List<RelatorioOpcaoRespostaDto> CriarOpcoes() =>
    [
        new RelatorioOpcaoRespostaDto { Id = 1, Descricao = "Sim", Ordem = 1 },
        new RelatorioOpcaoRespostaDto { Id = 2, Descricao = "Não", Ordem = 2 }
    ];

    private static RelatorioRespostaAlunoDto CriarResposta(
        int questaoId,
        string questaoNome,
        int opcaoRespostaId,
        int? anoTurma,
        IEnumerable<RelatorioOpcaoRespostaDto>? opcoes = null) =>
        new()
        {
            QuestaoId = questaoId,
            QuestaoNome = questaoNome,
            OpcaoRespostaId = opcaoRespostaId,
            AnoTurma = anoTurma,
            OpcoesDisponiveis = opcoes ?? CriarOpcoes()
        };

    [Fact]
    public async Task ObterSondagemRelatorio_SemRespostas_RetornaTituloSemDados()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2025 };
        _mockRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        Assert.Contains("Sem Dados", resultado.Titulo);
        Assert.Empty(resultado.Questoes);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_ComRespostas_RetornaTituloComAnoLetivo()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2025 };
        _mockRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync([CriarResposta(1, "Q1", 1, anoTurma: 3)]);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        Assert.Equal("Relatório Consolidado de Sondagem por Questões - 2025", resultado.Titulo);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_ComRespostas_AgrupaQuestoesPorId()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2025 };
        _mockRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                CriarResposta(1, "Q1", 1, anoTurma: 1),
                CriarResposta(1, "Q1", 1, anoTurma: 2),
                CriarResposta(2, "Q2", 1, anoTurma: 1)
            ]);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        Assert.Equal(2, resultado.Questoes.Count());
    }

    [Fact]
    public async Task ObterSondagemRelatorio_PopulaTotaisPorAnoTurma()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2025 };
        _mockRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                CriarResposta(1, "Q1", 1, anoTurma: 1),
                CriarResposta(1, "Q1", 1, anoTurma: 1),
                CriarResposta(1, "Q1", 1, anoTurma: 2)
            ]);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var totais = resultado.Questoes.Single().TotaisPorAnoTurma!.ToList();
        Assert.Equal(2, totais.Count);
        Assert.Equal(1, totais[0].AnoTurma);
        Assert.Equal(2, totais[0].Quantidade);
        Assert.Equal(2, totais[1].AnoTurma);
        Assert.Equal(1, totais[1].Quantidade);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_AnoTurmaNull_ExcluidoDosTotaisPorAnoTurma()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2025 };
        _mockRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                CriarResposta(1, "Q1", 1, anoTurma: 3),
                CriarResposta(1, "Q1", 1, anoTurma: null)
            ]);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var totais = resultado.Questoes.Single().TotaisPorAnoTurma!.ToList();
        Assert.Single(totais);
        Assert.Equal(3, totais[0].AnoTurma);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_TotaisPorAnoTurmaOrdenadosPorAno()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2025 };
        _mockRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                CriarResposta(1, "Q1", 1, anoTurma: 5),
                CriarResposta(1, "Q1", 1, anoTurma: 2),
                CriarResposta(1, "Q1", 1, anoTurma: 3)
            ]);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var anos = resultado.Questoes.Single().TotaisPorAnoTurma!.Select(t => t.AnoTurma).ToList();
        Assert.Equal([2, 3, 5], anos);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_CalculaPercentualPorAnoTurmaCorretamente()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2025 };
        _mockRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                CriarResposta(1, "Q1", 1, anoTurma: 1),
                CriarResposta(1, "Q1", 1, anoTurma: 1),
                CriarResposta(1, "Q1", 1, anoTurma: 1),
                CriarResposta(1, "Q1", 1, anoTurma: 2)
            ]);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert — 3 de 4 = 75%, 1 de 4 = 25%
        var totais = resultado.Questoes.Single().TotaisPorAnoTurma!.ToList();
        Assert.Equal(75, totais.First(t => t.AnoTurma == 1).Percentual);
        Assert.Equal(25, totais.First(t => t.AnoTurma == 2).Percentual);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_SemAbrangencia_ConsultaAbrangenciaService()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2025 };
        _mockRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(It.IsAny<FiltroConsolidadoDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        _mockAbrangencia.Verify(x => x.ObterAbrangenciaCompletaAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
