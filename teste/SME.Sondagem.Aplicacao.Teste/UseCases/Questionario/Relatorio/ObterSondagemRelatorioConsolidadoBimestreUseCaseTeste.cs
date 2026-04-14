using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoBimestreUseCaseTeste
{
    private readonly Mock<IRepositorioRespostaAluno> _mockRepositorioRespostaAluno;
    private readonly Mock<IRepositorioElasticTurma> _mockRepositorioElasticTurma;
    private readonly RepositoriosSondagem _repositoriosSondagem;
    private readonly ObterSondagemRelatorioConsolidadoBimestreUseCase _useCase;

    public ObterSondagemRelatorioConsolidadoBimestreUseCaseTeste()
    {
        _mockRepositorioRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        _mockRepositorioElasticTurma = new Mock<IRepositorioElasticTurma>();

        _repositoriosSondagem = new RepositoriosSondagem(
            new Mock<IRepositorioSondagem>().Object,
            new Mock<IRepositorioQuestao>().Object,
            _mockRepositorioRespostaAluno.Object,
            new Mock<IRepositorioBimestre>().Object,
            new Mock<IRepositorioComponenteCurricular>().Object,
            new Mock<IRepositorioProficiencia>().Object
        );

        _useCase = new ObterSondagemRelatorioConsolidadoBimestreUseCase(_repositoriosSondagem, _mockRepositorioElasticTurma.Object);
    }

    private static List<RelatorioOpcaoRespostaDto> CriarOpcoes() =>
    [
        new RelatorioOpcaoRespostaDto { Id = 1, Descricao = "Certa",  Ordem = 1 },
        new RelatorioOpcaoRespostaDto { Id = 2, Descricao = "Errada", Ordem = 2 }
    ];

    private static RelatorioRespostaAlunoDto CriarResposta(
        int alunoId,
        int questaoId,
        string questaoNome,
        int opcaoRespostaId,
        int? bimestreId = null,
        string? bimestreDescricao = null,
        IEnumerable<RelatorioOpcaoRespostaDto>? opcoes = null) =>
        new RelatorioRespostaAlunoDto
        {
            AlunoId = alunoId,
            QuestaoId = questaoId,
            QuestaoNome = questaoNome,
            OpcaoRespostaId = opcaoRespostaId,
            BimestreId = bimestreId,
            BimestreDescricao = bimestreDescricao,
            OpcoesDisponiveis = opcoes ?? CriarOpcoes()
        };

    [Fact]
    public async Task ObterSondagemRelatorio_DeveRetornarVazio_QuandoNaoHouverDados()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto();
        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RelatorioRespostaAlunoDto>());

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        Assert.Contains("Sem Dados", resultado.Titulo);
        Assert.Empty(resultado.Questoes);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveAgruparPorBimestreCorretamente()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };
        var opcoes = CriarOpcoes();

        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(101, 1, "Produção", opcaoRespostaId: 1, bimestreId: 1, opcoes: null),
            CriarResposta(102, 1, "Produção", opcaoRespostaId: 1, bimestreId: 1, opcoes: null),
            CriarResposta(103, 1, "Produção", opcaoRespostaId: 1, bimestreId: 2, opcoes: null)
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var questao = resultado.Questoes.Single();
        var respostaCerta = questao?.Respostas?.First(r => r.Resposta == "Certa");
        
        Assert.NotNull(respostaCerta?.Bimestres);
        Assert.Equal(2, respostaCerta?.Bimestres?.Count());
        
        var bimestre1 = respostaCerta?.Bimestres?.First(b => b.Bimestre == "1º bimestre");
        Assert.Equal(2, bimestre1?.Quantidade);

        var bimestre2 = respostaCerta?.Bimestres?.First(b => b.Bimestre == "2º bimestre");
        Assert.Equal(1, bimestre2?.Quantidade);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveTratarBimestreInicialCorretamente()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto();
        var opcoes = CriarOpcoes();
        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, 1, "Q1", opcaoRespostaId: 1, bimestreId: 0, opcoes: null)
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var bimestre = resultado?.Questoes?.First()?.Respostas?.First()?.Bimestres?.First();
        Assert.NotNull(bimestre);
        Assert.Equal("Sondagem inicial", bimestre.Bimestre);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveCalcularPercentuaisCorretamente()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };
        var opcoes = CriarOpcoes();

        // 4 respostas no total da questão
        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, 1, "Q1", opcaoRespostaId: 1, bimestreId: 1, opcoes: null),
            CriarResposta(2, 1, "Q1", opcaoRespostaId: 1, bimestreId: 1, opcoes: null),
            CriarResposta(3, 1, "Q1", opcaoRespostaId: 1, bimestreId: 2, opcoes: null),
            CriarResposta(4, 1, "Q1", opcaoRespostaId: 1, bimestreId: 2, opcoes: null)
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var bimestres = resultado?.Questoes?.First()?.Respostas?.First()?.Bimestres?.ToList() ?? [];
        // Cada bimestre tem 2 respostas de um total de 4 -> 50%
        Assert.All(bimestres, b => Assert.Equal(50, b.Percentual));
    }
}
