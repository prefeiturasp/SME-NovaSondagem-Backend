using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
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
    private readonly Mock<IAbrangenciaService> _mockAbrangenciaService;
    private readonly RepositoriosSondagem _repositoriosSondagem;
    private readonly ObterSondagemRelatorioConsolidadoBimestreUseCase _useCase;

    public ObterSondagemRelatorioConsolidadoBimestreUseCaseTeste()
    {
        _mockRepositorioRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        _mockRepositorioElasticTurma = new Mock<IRepositorioElasticTurma>();
        _mockRepositorioBimestre = new Mock<IRepositorioBimestre>();
        _mockAbrangenciaService = new Mock<IAbrangenciaService>();
        _mockAbrangenciaService
            .Setup(x => x.ObterAbrangenciaCompletaAsync(It.IsAny<AbrangenciaFiltroQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<string>(), new List<string>(), new List<string>()));

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

        _useCase = new ObterSondagemRelatorioConsolidadoBimestreUseCase(_repositoriosSondagem, _mockRepositorioElasticTurma.Object, _mockAbrangenciaService.Object);

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
    public async Task ObterSondagemRelatorio_ModalidadeEjaSemSemestre_DeveLancarExcecao()
    {
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026, Modalidade = 3 };

        await Assert.ThrowsAsync<SME.Sondagem.Dominio.RegraNegocioException>(
            () => _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None));
    }

    [Fact]
    public async Task ObterSondagemRelatorio_ModalidadeEjaComBimestreIdSemSemestre_NaoDeveLancarExcecao()
    {
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026, Modalidade = 3, BimestreId = 4 };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RelatorioRespostaAlunoDto>());

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        Assert.Contains("Sem Dados", resultado.Titulo);
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
    public async Task ObterSondagemRelatorio_DeveCalcularPercentuaisCorretamente()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };

        _mockRepositorioBimestre
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BimestreDominio> {
                new BimestreDominio(1, "1º") { Id = 1 },
                new BimestreDominio(2, "2º") { Id = 2 }
            });

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
        // Cada bimestre tem 2 respostas "Certa" de um total de 2 naquele bimestre -> 100%
        Assert.All(bimestres, b => Assert.Equal(100, b.Percentual));
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
    public void BimestreModalidadeEjaStrategy_DeveRetornarApenas1e2Bimestre()
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
        Assert.Contains(resultado, b => b.Id == 3);
    }

    [Fact]
    public void BimestreModalidadeEjaStrategy_DeveManterDescricoesOriginais()
    {
        var strategy = new BimestreModalidadeEjaStrategy();
        var bimestres = new List<BimestreDominio>
        {
            new BimestreDominio(1, "1° bimestre") { Id = 2 },
            new BimestreDominio(2, "2° bimestre") { Id = 3 }
        };

        var resultado = strategy.AplicarRegras(bimestres, null).ToList();

        Assert.Equal("1° bimestre", resultado.First(b => b.Id == 2).Descricao);
        Assert.Equal("2° bimestre", resultado.First(b => b.Id == 3).Descricao);
    }

    [Fact]
    public void BimestreModalidadeEjaStrategy_SegundoSemestre_DeveRetornarBimestre4e5RenomeadosPara1e2()
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

        var resultado = strategy.AplicarRegras(bimestres, null, semestre: 2).ToList();

        Assert.Equal(2, resultado.Count);
        Assert.Equal("1° bimestre", resultado.Single(b => b.Id == 4).Descricao);
        Assert.Equal("2° bimestre", resultado.Single(b => b.Id == 5).Descricao);
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
    public async Task ObterSondagemRelatorio_ModalidadeEja_DeveExibirApenas2Bimestres()
    {
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026, Modalidade = 3, SemestreId = 1 };

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

        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, 1, "Q1", opcaoRespostaId: 1, bimestreId: 2),
            CriarResposta(2, 1, "Q1", opcaoRespostaId: 1, bimestreId: 3),
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var bimestresExibidos = resultado.Questoes.First().Respostas!.First().Bimestres!.ToList();

        Assert.Equal(2, bimestresExibidos.Count);
        Assert.Contains(bimestresExibidos, b => b.Bimestre == "1° bimestre" && b.Quantidade == 1);
        Assert.Contains(bimestresExibidos, b => b.Bimestre == "2° bimestre" && b.Quantidade == 1);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_ModalidadeEjaSegundoSemestre_DeveExibirBimestres4e5Renomeados()
    {
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026, Modalidade = 3, SemestreId = 2 };

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

        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, 1, "Q1", opcaoRespostaId: 1, bimestreId: 4),
            CriarResposta(2, 1, "Q1", opcaoRespostaId: 1, bimestreId: 5),
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        var bimestresExibidos = resultado.Questoes.First().Respostas!.First().Bimestres!.ToList();

        Assert.Equal(2, bimestresExibidos.Count);
        Assert.Contains(bimestresExibidos, b => b.Bimestre == "1° bimestre" && b.Quantidade == 1);
        Assert.Contains(bimestresExibidos, b => b.Bimestre == "2° bimestre" && b.Quantidade == 1);
    }
}
