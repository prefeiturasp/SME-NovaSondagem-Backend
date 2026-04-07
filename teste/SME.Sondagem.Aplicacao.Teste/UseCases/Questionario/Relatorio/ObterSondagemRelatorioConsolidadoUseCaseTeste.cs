using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoUseCaseTeste
{
    private readonly Mock<IRepositorioRespostaAluno> _mockRepositorioRespostaAluno;
    private readonly RepositoriosSondagem _repositoriosSondagem;
    private readonly ObterSondagemRelatorioConsolidadoRacaUseCase _useCase;

    public ObterSondagemRelatorioConsolidadoUseCaseTeste()
    {
        _mockRepositorioRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        
        _repositoriosSondagem = new RepositoriosSondagem(
            new Mock<IRepositorioSondagem>().Object,
            new Mock<IRepositorioQuestao>().Object,
            _mockRepositorioRespostaAluno.Object,
            new Mock<IRepositorioBimestre>().Object,
            new Mock<IRepositorioComponenteCurricular>().Object,
            new Mock<IRepositorioProficiencia>().Object
        );

        _useCase = new ObterSondagemRelatorioConsolidadoRacaUseCase(_repositoriosSondagem);
    }

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
        Assert.Empty(resultado.Anos);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveAgruparPorAnoERespostaCorretamente()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };
        var opcoes = new List<RelatorioOpcaoRespostaDto>
        {
            new RelatorioOpcaoRespostaDto { Id = 1, Descricao = "Certa", Ordem = 1 },
            new RelatorioOpcaoRespostaDto { Id = 2, Descricao = "Errada", Ordem = 2 }
        };

        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            new RelatorioRespostaAlunoDto { AlunoId = 101, AnoLetivo = 2026, OpcaoRespostaId = 1, RacaCor = new RelatorioRacaCorDto { Descricao = "Branca" }, OpcoesDisponiveis = opcoes },
            new RelatorioRespostaAlunoDto { AlunoId = 102, AnoLetivo = 2026, OpcaoRespostaId = 1, RacaCor = new RelatorioRacaCorDto { Descricao = "Parda" }, OpcoesDisponiveis = opcoes },
            new RelatorioRespostaAlunoDto { AlunoId = 103, AnoLetivo = 2026, OpcaoRespostaId = 2, RacaCor = new RelatorioRacaCorDto { Descricao = "Parda" }, OpcoesDisponiveis = opcoes }
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var ano = resultado.Anos.First();
        Assert.Equal(3, ano.TotalEstudantes);
        
        var respostaCerta = ano.Respostas.First(r => r.Resposta == "Certa");
        Assert.Equal(2, respostaCerta.Total);
        Assert.Equal((2.0/3.0)*100, respostaCerta.Percentual, 1);
        Assert.Equal(2, respostaCerta.Racas.Count());

        var respostaErrada = ano.Respostas.First(r => r.Resposta == "Errada");
        Assert.Equal(1, respostaErrada.Total);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveCalcularPercentualPorRacaDentroDaResposta()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };
        var opcoes = new List<RelatorioOpcaoRespostaDto> { new RelatorioOpcaoRespostaDto { Id = 1, Descricao = "Resposta", Ordem = 1 } };

        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            new RelatorioRespostaAlunoDto { AlunoId = 1, AnoLetivo = 2026, OpcaoRespostaId = 1, RacaCor = new RelatorioRacaCorDto { Descricao = "Branca" }, OpcoesDisponiveis = opcoes },
            new RelatorioRespostaAlunoDto { AlunoId = 2, AnoLetivo = 2026, OpcaoRespostaId = 1, RacaCor = new RelatorioRacaCorDto { Descricao = "Branca" }, OpcoesDisponiveis = opcoes },
            new RelatorioRespostaAlunoDto { AlunoId = 3, AnoLetivo = 2026, OpcaoRespostaId = 1, RacaCor = new RelatorioRacaCorDto { Descricao = "Parda" }, OpcoesDisponiveis = opcoes },
            new RelatorioRespostaAlunoDto { AlunoId = 4, AnoLetivo = 2026, OpcaoRespostaId = 1, RacaCor = new RelatorioRacaCorDto { Descricao = "Parda" }, OpcoesDisponiveis = opcoes }
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var racas = resultado.Anos.First().Respostas.First().Racas.ToList();
        Assert.All(racas, r => Assert.Equal(50, r.Percentual));
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveTratarRacaComoNaoInformado_QuandoForNulo()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto();
        var opcoes = new List<RelatorioOpcaoRespostaDto> { new RelatorioOpcaoRespostaDto { Id = 1, Descricao = "Op", Ordem = 1 } };
        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            new RelatorioRespostaAlunoDto { AlunoId = 1, OpcaoRespostaId = 1, RacaCor = null, OpcoesDisponiveis = opcoes }
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var raca = resultado.Anos.First().Respostas.First().Racas.First();
        Assert.Equal("Não Informado", raca.Raca);
    }
}
