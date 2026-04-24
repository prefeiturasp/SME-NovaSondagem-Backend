using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio.Strategies.Bimestre;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using Xunit;
using BimestreDominio = SME.Sondagem.Dominio.Entidades.Bimestre;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoBimestreUseCaseTeste
{
    private readonly Mock<IRepositorioRespostaAluno> _mockRepositorioRespostaAluno;
    private readonly Mock<IRepositorioElasticTurma> _mockRepositorioElasticTurma;
    private readonly Mock<IRepositorioBimestre> _mockRepositorioBimestre;
    private readonly RepositoriosSondagem _repositoriosSondagem;
    private readonly ObterSondagemRelatorioConsolidadoBimestreUseCase _useCase;

    public ObterSondagemRelatorioConsolidadoBimestreUseCaseTeste()
    {
        _mockRepositorioRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        _mockRepositorioElasticTurma = new Mock<IRepositorioElasticTurma>();
        _mockRepositorioBimestre = new Mock<IRepositorioBimestre>();

        _repositoriosSondagem = new RepositoriosSondagem(
            new Mock<IRepositorioSondagem>().Object,
            new Mock<IRepositorioQuestao>().Object,
            _mockRepositorioRespostaAluno.Object,
            _mockRepositorioBimestre.Object,
            new Mock<IRepositorioComponenteCurricular>().Object,
            new Mock<IRepositorioProficiencia>().Object,
            new Mock<IRepositorioRacaCor>().Object,
            new Mock<IRepositorioGeneroSexo>().Object
        );

        _useCase = new ObterSondagemRelatorioConsolidadoBimestreUseCase(_repositoriosSondagem, _mockRepositorioElasticTurma.Object);

        // Setup padrão de bimestres para evitar erros de referência nula
        _mockRepositorioBimestre
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BimestreDominio>());
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
    public async Task ObterSondagemRelatorio_DeveAgruparPorBimestreUtilizandoTabelaBimestre()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };
        var opcoes = CriarOpcoes();

        var bimestresTabela = new List<BimestreDominio>
        {
            new BimestreDominio(0, "Sondagem inicial") { Id = 0 },
            new BimestreDominio(1, "1º bimestre") { Id = 1 },
            new BimestreDominio(2, "2º bimestre") { Id = 2 }
        };

        _mockRepositorioBimestre
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(bimestresTabela);

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
        Assert.Equal(3, respostaCerta?.Bimestres?.Count()); // 0, 1 e 2

        var inicial = respostaCerta?.Bimestres?.First(b => b.Bimestre == "Sondagem inicial");
        Assert.Equal(0, inicial?.Quantidade);

        var bimestre1 = respostaCerta?.Bimestres?.First(b => b.Bimestre == "1º bimestre");
        Assert.Equal(2, bimestre1?.Quantidade);

        var bimestre2 = respostaCerta?.Bimestres?.First(b => b.Bimestre == "2º bimestre");
        Assert.Equal(1, bimestre2?.Quantidade);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveCalcularPercentuaisCorretamentePorBimestre()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };

        _mockRepositorioBimestre
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BimestreDominio> {
                new BimestreDominio(1, "1º") { Id = 1 },
                new BimestreDominio(2, "2º") { Id = 2 }
            });

        // Bimestre 1: 3 estudantes (2 "Certa" e 1 "Errada")
        // Bimestre 2: 2 estudantes (1 "Certa" e 1 "Errada")
        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, 1, "Q1", opcaoRespostaId: 1, bimestreId: 1, opcoes: null),
            CriarResposta(2, 1, "Q1", opcaoRespostaId: 1, bimestreId: 1, opcoes: null),
            CriarResposta(3, 1, "Q1", opcaoRespostaId: 2, bimestreId: 1, opcoes: null),
            CriarResposta(4, 1, "Q1", opcaoRespostaId: 1, bimestreId: 2, opcoes: null),
            CriarResposta(5, 1, "Q1", opcaoRespostaId: 2, bimestreId: 2, opcoes: null)
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var respostaCerta = resultado?.Questoes?.First()?.Respostas?.First(r => r.Resposta == "Certa");
        var bimestres = respostaCerta?.Bimestres?.ToList() ?? [];

        var bimestre1 = bimestres.First(b => b.Bimestre == "1º");
        var bimestre2 = bimestres.First(b => b.Bimestre == "2º");

        // "Certa" no 1º bimestre: 2 de 3 estudantes = 66.67%
        Assert.Equal(66.67, bimestre1.Percentual);
        // "Certa" no 2º bimestre: 1 de 2 estudantes = 50%
        Assert.Equal(50, bimestre2.Percentual);
    }

    // ─── Testes das Strategies ───────────────────────────────────────────────

    [Fact]
    public void BimestreModalidadePadraoStrategy_DeveRetornarTodosOsBimestres()
    {
        var strategy = new BimestreModalidadePadraoStrategy();
        var bimestres = new List<BimestreDominio>
        {
            new BimestreDominio(1, "1° bimestre") { Id = 2 },
            new BimestreDominio(2, "2° bimestre") { Id = 3 },
            new BimestreDominio(4, "4° bimestre") { Id = 5 }
        };

        var resultado = strategy.AplicarRegras(bimestres, null).ToList();

        Assert.Equal(3, resultado.Count);
    }

    [Fact]
    public void BimestreModalidadePadraoStrategy_DeveAplicarFiltroQuandoInformado()
    {
        var strategy = new BimestreModalidadePadraoStrategy();
        var bimestres = new List<BimestreDominio>
        {
            new BimestreDominio(1, "1° bimestre") { Id = 2 },
            new BimestreDominio(4, "4° bimestre") { Id = 5 }
        };

        var resultado = strategy.AplicarRegras(bimestres, bimestreFiltrado: 2).ToList();

        Assert.Single(resultado);
        Assert.Equal(2, resultado[0].Id);
    }

    [Fact]
    public void BimestreModalidadeEjaStrategy_DeveRetornarApenas1e4Bimestre()
    {
        var strategy = new BimestreModalidadeEjaStrategy();
        var bimestres = new List<BimestreDominio>
        {
            new BimestreDominio(0, "Inicial")     { Id = 1 },
            new BimestreDominio(1, "1° bimestre") { Id = 2 },
            new BimestreDominio(2, "2° bimestre") { Id = 3 },
            new BimestreDominio(3, "3° bimestre") { Id = 4 },
            new BimestreDominio(4, "4° bimestre") { Id = 5 }
        };

        var resultado = strategy.AplicarRegras(bimestres, null).ToList();

        Assert.Equal(2, resultado.Count);
        Assert.Contains(resultado, b => b.Id == 2);
        Assert.Contains(resultado, b => b.Id == 5);
    }

    [Fact]
    public void BimestreModalidadeEjaStrategy_DeveRenomearQuartoBimestrePara2Bimestre()
    {
        var strategy = new BimestreModalidadeEjaStrategy();
        var bimestres = new List<BimestreDominio>
        {
            new BimestreDominio(1, "1° bimestre") { Id = 2 },
            new BimestreDominio(4, "4° bimestre") { Id = 5 }
        };

        var resultado = strategy.AplicarRegras(bimestres, null).ToList();

        Assert.Equal("1° bimestre", resultado.First(b => b.Id == 2).Descricao);
        Assert.Equal("2° bimestre", resultado.First(b => b.Id == 5).Descricao);
    }

    [Fact]
    public void BimestreModalidadeStrategyFactory_DeveRetornarEjaStrategy_QuandoModalidade3()
    {
        var strategy = BimestreModalidadeStrategyFactory.ObterPara(3);
        Assert.IsType<BimestreModalidadeEjaStrategy>(strategy);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void BimestreModalidadeStrategyFactory_DeveRetornarPadraoStrategy_QuandoOutrasModalidades(int modalidade)
    {
        var strategy = BimestreModalidadeStrategyFactory.ObterPara(modalidade);
        Assert.IsType<BimestreModalidadePadraoStrategy>(strategy);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_ModalidadeEja_DeveExibirApenas2Bimestres_Com4RenomeadoPara2()
    {
        // Arrange: Modalidade 3 (EJA), todos os bimestres no banco
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026, Modalidade = 3 };

        _mockRepositorioBimestre
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BimestreDominio>
            {
                new BimestreDominio(0, "Inicial")     { Id = 1 },
                new BimestreDominio(1, "1° bimestre") { Id = 2 },
                new BimestreDominio(2, "2° bimestre") { Id = 3 },
                new BimestreDominio(3, "3° bimestre") { Id = 4 },
                new BimestreDominio(4, "4° bimestre") { Id = 5 }
            });

        // Alunos responderam no 1° bimestre (Id=2) e no 4° (Id=5)
        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, 1, "Q1", opcaoRespostaId: 1, bimestreId: 2),
            CriarResposta(2, 1, "Q1", opcaoRespostaId: 1, bimestreId: 5),
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var bimestresExibidos = resultado.Questoes.First().Respostas!.First().Bimestres!.ToList();

        // Só 2 bimestres visíveis para EJA
        Assert.Equal(2, bimestresExibidos.Count);

        // 4° bimestre NÃO deve aparecer com o nome original
        Assert.DoesNotContain(bimestresExibidos, b => b.Bimestre == "4° bimestre");

        // 4° bimestre deve aparecer renomeado como "2° bimestre" com 1 resposta
        Assert.Contains(bimestresExibidos, b => b.Bimestre == "2° bimestre" && b.Quantidade == 1);

        // 1° bimestre mantém nome e tem 1 resposta
        Assert.Contains(bimestresExibidos, b => b.Bimestre == "1° bimestre" && b.Quantidade == 1);
    }
}
