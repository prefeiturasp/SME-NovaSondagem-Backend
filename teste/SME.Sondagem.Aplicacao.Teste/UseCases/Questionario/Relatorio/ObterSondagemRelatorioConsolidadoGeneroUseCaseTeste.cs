using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using Xunit;
using GeneroDominio = SME.Sondagem.Dominio.Entidades.GeneroSexo;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoGeneroUseCaseTeste
{
    private readonly Mock<IRepositorioRespostaAluno> _mockRepositorioRespostaAluno;
    private readonly Mock<IRepositorioElasticTurma> _mockRepositorioElasticTurma;
    private readonly Mock<IRepositorioGeneroSexo> _mockRepositorioGenero;
    private readonly RepositoriosSondagem _repositoriosSondagem;
    private readonly ObterSondagemRelatorioConsolidadoGeneroUseCase _useCase;

    public ObterSondagemRelatorioConsolidadoGeneroUseCaseTeste()
    {
        _mockRepositorioRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        _mockRepositorioElasticTurma = new Mock<IRepositorioElasticTurma>();
        _mockRepositorioGenero = new Mock<IRepositorioGeneroSexo>();

        _repositoriosSondagem = new RepositoriosSondagem(
            new Mock<IRepositorioSondagem>().Object,
            new Mock<IRepositorioQuestao>().Object,
            _mockRepositorioRespostaAluno.Object,
            new Mock<IRepositorioBimestre>().Object,
            new Mock<IRepositorioComponenteCurricular>().Object,
            new Mock<IRepositorioProficiencia>().Object,
            new Mock<IRepositorioRacaCor>().Object,
            _mockRepositorioGenero.Object
        );

        _useCase = new ObterSondagemRelatorioConsolidadoGeneroUseCase(_repositoriosSondagem, _mockRepositorioElasticTurma.Object);
        
        // Setup padrão de gêneros
        _mockRepositorioGenero
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GeneroDominio>());
    }

    private static List<RelatorioOpcaoRespostaDto> CriarOpcoes() =>
    [
        new RelatorioOpcaoRespostaDto { Id = 1, Descricao = "Certa",  Ordem = 1 },
        new RelatorioOpcaoRespostaDto { Id = 2, Descricao = "Errada", Ordem = 2 }
    ];

    private static RelatorioRespostaAlunoDto CriarResposta(
        int alunoId,
        int questaoId,
        int opcaoRespostaId,
        int? generoId = null) =>
        new RelatorioRespostaAlunoDto
        {
            AlunoId = alunoId,
            QuestaoId = questaoId,
            OpcaoRespostaId = opcaoRespostaId,
            GeneroSexoId = generoId,
            OpcoesDisponiveis = CriarOpcoes()
        };

    [Fact]
    public async Task ObterSondagemRelatorio_DeveAgruparPorGeneroUtilizandoTabelaReferencia()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto();
        
        var generosTabela = new List<GeneroDominio>
        {
            new GeneroDominio { Id = 1, Descricao = "Feminino", Sigla = "F" },
            new GeneroDominio { Id = 2, Descricao = "Masculino", Sigla = "M" }
        };

        _mockRepositorioGenero
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(generosTabela.AsEnumerable());

        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(101, 1, 1, generoId: 1),
            CriarResposta(102, 1, 1, generoId: 1),
            CriarResposta(103, 1, 1, generoId: 2)
        };

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        // Act
        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);

        // Assert
        var questao = resultado.Questoes.Single();
        var respostaCerta = questao?.Respostas?.First(r => r.Resposta == "Certa");
        
        Assert.NotNull(respostaCerta?.Generos);
        Assert.Equal(2, respostaCerta?.Generos?.Count());
        
        var fem = respostaCerta?.Generos?.First(g => g.Genero == "Feminino");
        Assert.Equal(2, fem?.Quantidade);

        var masc = respostaCerta?.Generos?.First(g => g.Genero == "Masculino");
        Assert.Equal(1, masc?.Quantidade);
    }

    [Fact]
    public async Task ObterSondagemRelatorio_DeveFiltrarPorGeneroId_QuandoInformado()
    {
        // Arrange
        var filtro = new FiltroConsolidadoDto { GeneroId = 1 };
        
        var generosTabela = new List<GeneroDominio>
        {
            new GeneroDominio { Id = 1, Descricao = "Feminino", Sigla = "F" },
            new GeneroDominio { Id = 2, Descricao = "Masculino", Sigla = "M" }
        };

        _mockRepositorioGenero
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(generosTabela.AsEnumerable());

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RelatorioRespostaAlunoDto>());

        // Act
        // Mesmo sem dados, deve mostrar o header do gênero filtrado
        // Aqui verificamos se a _generosReferencia (usada no processamento) teria apenas 1 item
        // Como o resultado é "Sem Dados" se não houver respostas em NEHUMA questão, 
        // mas se houvesse uma questão e zero respostas para esse gênero, apareceria.
        // Vamos forçar uma resposta para outra questão só para ver o header.
        
        var respostas = new List<RelatorioRespostaAlunoDto>
        {
            CriarResposta(1, 1, 1, generoId: 1)
        };
        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasParaRelatorioConsolidadoAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostas);

        var resultado = await _useCase.ObterSondagemRelatorio(filtro, CancellationToken.None);
        
        var generosExibidos = resultado.Questoes.FirstOrDefault()?.Respostas?.FirstOrDefault()?.Generos ?? [];
        Assert.Single(generosExibidos);
        Assert.Equal("Feminino", generosExibidos.First().Genero);
    }
}
