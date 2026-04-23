using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using Xunit;
using DominioC = SME.Sondagem.Dominio.Entidades.RacaCor;
using DominioG = SME.Sondagem.Dominio.Entidades.GeneroSexo;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoRacaGeneroUseCaseTeste
{
    private readonly Mock<IRepositorioRespostaAluno> _mockRepositorioRespostaAluno;
    private readonly Mock<IRepositorioElasticTurma> _mockRepositorioElasticTurma;
    private readonly Mock<IRepositorioRacaCor> _mockRepositorioRacaCor;
    private readonly Mock<IRepositorioGeneroSexo> _mockRepositorioGeneroSexo;
    private readonly RepositoriosSondagem _repositoriosSondagem;
    private readonly ObterSondagemRelatorioConsolidadoRacaGeneroUseCase _useCase;

    public ObterSondagemRelatorioConsolidadoRacaGeneroUseCaseTeste()
    {
        _mockRepositorioRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        _mockRepositorioElasticTurma = new Mock<IRepositorioElasticTurma>();
        _mockRepositorioRacaCor = new Mock<IRepositorioRacaCor>();
        _mockRepositorioGeneroSexo = new Mock<IRepositorioGeneroSexo>();

        _repositoriosSondagem = new RepositoriosSondagem(
            new Mock<IRepositorioSondagem>().Object,
            new Mock<IRepositorioQuestao>().Object,
            _mockRepositorioRespostaAluno.Object,
            new Mock<IRepositorioBimestre>().Object,
            new Mock<IRepositorioComponenteCurricular>().Object,
            new Mock<IRepositorioProficiencia>().Object,
            _mockRepositorioRacaCor.Object,
            _mockRepositorioGeneroSexo.Object
            
        );

        _useCase = new ObterSondagemRelatorioConsolidadoRacaGeneroUseCase(_repositoriosSondagem, _mockRepositorioElasticTurma.Object);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveSerExaustivo_MostrandoMatrizGereRaça()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { AnoLetivo = 2026 };

        var generos = new List<DominioG>
        {
            new() { Id = 1, Descricao = "Masculino" },
            new() { Id = 2, Descricao = "Feminino" }
        };

        var racas = new List<DominioC>
        {
            new() { Id = 1, Descricao = "Branca", CodigoEolRacaCor = 1 },
            new() { Id = 2, Descricao = "Parda",  CodigoEolRacaCor = 2 }
        };

        _mockRepositorioGeneroSexo.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync(generos.AsEnumerable());
        _mockRepositorioRacaCor.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync(racas.AsEnumerable());

        // Apenas Masculino/Branca tem resposta
        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            new() { 
                AlunoId = 1, 
                QuestaoId = 1, 
                QuestaoNome = "Q1", 
                OpcaoRespostaId = 1, 
                GeneroSexoId = 1, 
                RacaCorId = 1,
                OpcoesDisponiveis = [ new RelatorioOpcaoRespostaDto { Id = 1, Descricao = "Op", Ordem = 1 } ]
            }
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var questao = resultado.Questoes.First();
        var resposta = questao?.Respostas?.First();

        Assert.Equal(2, resposta?.GenerosComRacas?.Count()); // Masculino e Feminino
        
        var masc = resposta?.GenerosComRacas?.First(g => g.Genero == "Masculino");
        Assert.Equal(1, masc?.TotalGenero);
        Assert.Equal(2, masc?.Racas?.Count());
        Assert.Contains(masc?.Racas ?? Enumerable.Empty<RelatorioConsolidadoRacaDto>(), r => r.Raca == "Branca" && r.Quantidade == 1);
        Assert.Contains(masc?.Racas ?? Enumerable.Empty<RelatorioConsolidadoRacaDto>(), r => r.Raca == "Parda" && r.Quantidade == 0);

        var fem = resposta?.GenerosComRacas?.First(g => g.Genero == "Feminino");
        Assert.Equal(0, fem?.TotalGenero);
        Assert.Equal(2, fem?.Racas?.Count());
        Assert.Contains(fem?.Racas ?? Enumerable.Empty<RelatorioConsolidadoRacaDto>(), r => r.Raca == "Branca" && r.Quantidade == 0);
        Assert.Contains(fem?.Racas ?? Enumerable.Empty<RelatorioConsolidadoRacaDto>(), r => r.Raca == "Parda" && r.Quantidade == 0);
    }
}
