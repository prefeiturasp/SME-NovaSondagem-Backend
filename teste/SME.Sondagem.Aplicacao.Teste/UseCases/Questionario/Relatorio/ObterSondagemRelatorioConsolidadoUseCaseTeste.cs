using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using Xunit;
using DominioR = SME.Sondagem.Dominio.Entidades.RacaCor;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoUseCaseTeste
{
    private readonly Mock<IRepositorioRespostaAluno> _mockRepositorioRespostaAluno;
    private readonly Mock<IRepositorioElasticTurma> _mockRepositorioElasticTurma;
    private readonly Mock<IRepositorioRacaCor> _mockRepositorioRacaCor;
    private readonly RepositoriosSondagem _repositoriosSondagem;
    private readonly ObterSondagemRelatorioConsolidadoRacaUseCase _useCase;

    public ObterSondagemRelatorioConsolidadoUseCaseTeste()
    {
        _mockRepositorioRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        _mockRepositorioElasticTurma = new Mock<IRepositorioElasticTurma>();
        _mockRepositorioRacaCor = new Mock<IRepositorioRacaCor>();

        _repositoriosSondagem = new RepositoriosSondagem(
            new Mock<IRepositorioSondagem>().Object,
            new Mock<IRepositorioQuestao>().Object,
            _mockRepositorioRespostaAluno.Object,
            new Mock<IRepositorioBimestre>().Object,
            new Mock<IRepositorioComponenteCurricular>().Object,
            new Mock<IRepositorioProficiencia>().Object,
            _mockRepositorioRacaCor.Object,
            new Mock<IRepositorioGeneroSexo>().Object
        );

        _useCase = new ObterSondagemRelatorioConsolidadoRacaUseCase(_repositoriosSondagem, _mockRepositorioElasticTurma.Object);

        _mockRepositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DominioR> { 
                new() { Id = 1, Descricao = "Branca", CodigoEolRacaCor = 1 },
                new() { Id = 2, Descricao = "Parda",  CodigoEolRacaCor = 2 }
            }.AsEnumerable());
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
        int? opcaoRespostaId = null,
        int? racaId = null,
        int? generoId = null,
        IEnumerable<RelatorioOpcaoRespostaDto>? opcoes = null) =>
        new RelatorioRespostaAlunoDto
        {
            AlunoId = alunoId,
            QuestaoId = questaoId,
            QuestaoNome = questaoNome,
            OpcaoRespostaId = opcaoRespostaId,
            RacaCorId = racaId,
            GeneroSexoId = generoId,
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
    public async Task ObterSondagemRelatorio_DeveAgruparPorQuestaoERespostaCorretamente()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };
        var opcoes = CriarOpcoes();

        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(101, 1, "Produção", opcaoRespostaId: 1, racaId: 1),
            CriarResposta(102, 1, "Produção", opcaoRespostaId: 1, racaId: 2),
            CriarResposta(103, 1, "Produção", opcaoRespostaId: 2, racaId: 2)
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var questao = resultado.Questoes.Single();
        Assert.Equal(1, questao.QuestaoId);
        Assert.Equal("Produção", questao.QuestaoNome);
        Assert.Equal(3, questao.TotalEstudantes);

        var respostaCerta = questao?.Respostas?.First(r => r.Resposta == "Certa");
        Assert.Equal(2, respostaCerta?.Total);
        Assert.Equal(Math.Round((2.0 / 3.0) * 100, 2), respostaCerta?.Percentual);
        Assert.Equal(2, respostaCerta?.Racas?.Count());

        var respostaErrada = questao?.Respostas?.First(r => r.Resposta == "Errada");
        Assert.Equal(1, respostaErrada?.Total);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveAgruparMultiplasQuestoesIndependentemente()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };
        var opcoes = CriarOpcoes();

        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, questaoId: 1, "Leitura",  opcaoRespostaId: 1, opcoes: opcoes),
            CriarResposta(2, questaoId: 1, "Leitura",  opcaoRespostaId: 2, opcoes: opcoes),
            CriarResposta(3, questaoId: 2, "Produção", opcaoRespostaId: 1, opcoes: opcoes)
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        Assert.Equal(2, resultado.Questoes.Count());

        var questaoLeitura  = resultado.Questoes.Single(q => q.QuestaoNome == "Leitura");
        var questaoProdução = resultado.Questoes.Single(q => q.QuestaoNome == "Produção");

        Assert.Equal(2, questaoLeitura.TotalEstudantes);
        Assert.Equal(1, questaoProdução.TotalEstudantes);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveCalcularPercentualSobreTotalDaQuestao()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };
        var opcoes = new List<RelatorioOpcaoRespostaDto>
        {
            new RelatorioOpcaoRespostaDto { Id = 1, Descricao = "Resposta", Ordem = 1 }
        };

        // 4 respostas na questão → cada raça tem 2 = 50%
        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, 1, "Q1", opcaoRespostaId: 1, racaId: 1),
            CriarResposta(2, 1, "Q1", opcaoRespostaId: 1, racaId: 1),
            CriarResposta(3, 1, "Q1", opcaoRespostaId: 1, racaId: 2),
            CriarResposta(4, 1, "Q1", opcaoRespostaId: 1, racaId: 2)
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert - percentual de cada raça = 2/4 = 50% (sobre o total da questão)
        var racas = resultado?.Questoes?.First()?.Respostas?.First()?.Racas?.ToList() ?? [];
        Assert.All(racas, r => Assert.Equal(50, r.Percentual));
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveTratarRacaNulaComoNaoInformado_SendoExaustivo()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto();
        var racasReferencia = new List<DominioR>
        {
            new() { Id = 1, Descricao = "Branca", CodigoEolRacaCor = 1 }
        };
        _mockRepositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync(racasReferencia.AsEnumerable());

        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, 1, "Q1", opcaoRespostaId: 1, racaId: null)
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert - racaId null não bate com ID 1 da referência, mas a referência deve aparecer
        var raca = resultado?.Questoes?.First()?.Respostas?.First()?.Racas?.First();
        Assert.NotNull(raca);
        Assert.Equal("Branca", raca.Raca);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveSerExaustivo_MostrandoRacasSemResposta()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };

        var racasReferencia = new List<DominioR>
        {
            new() { Id = 1, Descricao = "Branca", CodigoEolRacaCor = 1 },
            new() { Id = 2, Descricao = "Parda",  CodigoEolRacaCor = 2 },
            new() { Id = 3, Descricao = "Preta",  CodigoEolRacaCor = 3 }
        };

        _mockRepositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync(racasReferencia.AsEnumerable());

        // Apenas Branca tem resposta
        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, 1, "Q1", opcaoRespostaId: 1, racaId: 1)
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var questao = resultado.Questoes.First();
        var resposta = questao.Respostas?.First()!;

        Assert.Equal(3, resposta?.Racas?.Count());
        Assert.Contains(resposta?.Racas ?? [], r => r.Raca == "Branca" && r.Quantidade == 1);
        Assert.Contains(resposta?.Racas ?? [], r => r.Raca == "Parda" && r.Quantidade == 0);
        Assert.Contains(resposta?.Racas ?? [], r => r.Raca == "Preta" && r.Quantidade == 0);
    }
}
