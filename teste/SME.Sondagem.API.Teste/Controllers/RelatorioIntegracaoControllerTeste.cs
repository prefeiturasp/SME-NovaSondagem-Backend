using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using Xunit;

namespace SME.Sondagem.API.Teste.Controllers;

public class RelatorioIntegracaoControllerTeste
{
    private readonly Mock<IObterSondagemRelatorioPorTurmaUseCase> _obterSondagemRelatorioPorTurmaUseCaseMock;
    private readonly RelatorioIntegracaoController _controller;

    public RelatorioIntegracaoControllerTeste()
    {
        _obterSondagemRelatorioPorTurmaUseCaseMock = new Mock<IObterSondagemRelatorioPorTurmaUseCase>();
        _controller = new RelatorioIntegracaoController();
    }

    [Fact]
    public async Task ObterRelatorioSondagemPorTurma_DeveRetornarOkComRelatorio_QuandoUseCaseRetornarDados()
    {
        // Arrange
        var filtro = new FiltroQuestionario
        {
            TurmaId = 1,
            ProficienciaId = 1,
            ComponenteCurricularId = 1,
            Modalidade = 5,
            Ano = 1,
            AnoLetivo = 2024,
            SemestreId = 1,
            UeCodigo = "123456",
            BimestreId = 1
        };
        var cancellationToken = CancellationToken.None;

        var relatorioEsperado = new QuestionarioSondagemRelatorioDto
        {
            TituloTabelaRespostas = "Relatório de Sondagem - Turma 1",
            Estudantes = new List<EstudanteQuestionarioDto>
            {
                new EstudanteQuestionarioDto
                {
                    Codigo = 1001,
                    Nome = "João Silva",
                    NumeroAlunoChamada = "1",
                    Pap = false,
                    LinguaPortuguesaSegundaLingua = false,
                    PossuiDeficiencia = false,
                    Raca = "Parda",
                    Genero = "Masculino"
                },
                new EstudanteQuestionarioDto
                {
                    Codigo = 1002,
                    Nome = "Maria Santos",
                    NumeroAlunoChamada = "2",
                    Pap = true,
                    LinguaPortuguesaSegundaLingua = false,
                    PossuiDeficiencia = true,
                    Raca = "Branca",
                    Genero = "Feminino"
                }
            }
        };

        _obterSondagemRelatorioPorTurmaUseCaseMock
            .Setup(x => x.ObterSondagemRelatorio(filtro, cancellationToken))
            .ReturnsAsync(relatorioEsperado);

        // Act
        var result = await _controller.ObterRelatorioSondagemPorTurma(
            filtro,
            _obterSondagemRelatorioPorTurmaUseCaseMock.Object,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var retorno = Assert.IsType<QuestionarioSondagemRelatorioDto>(okResult.Value);

        Assert.Equal(relatorioEsperado.TituloTabelaRespostas, retorno.TituloTabelaRespostas);
        Assert.NotNull(retorno.Estudantes);
        Assert.Equal(2, retorno.Estudantes.Count());

        var primeiroEstudante = retorno.Estudantes.First();
        Assert.Equal(1001, primeiroEstudante.Codigo);
        Assert.Equal("João Silva", primeiroEstudante.Nome);
        Assert.Equal("1", primeiroEstudante.NumeroAlunoChamada);
        Assert.False(primeiroEstudante.Pap);
        Assert.False(primeiroEstudante.LinguaPortuguesaSegundaLingua);
        Assert.False(primeiroEstudante.PossuiDeficiencia);
        Assert.Equal("Parda", primeiroEstudante.Raca);
        Assert.Equal("Masculino", primeiroEstudante.Genero);

        var segundoEstudante = retorno.Estudantes.Last();
        Assert.Equal(1002, segundoEstudante.Codigo);
        Assert.Equal("Maria Santos", segundoEstudante.Nome);
        Assert.Equal("2", segundoEstudante.NumeroAlunoChamada);
        Assert.True(segundoEstudante.Pap);
        Assert.False(segundoEstudante.LinguaPortuguesaSegundaLingua);
        Assert.True(segundoEstudante.PossuiDeficiencia);
        Assert.Equal("Branca", segundoEstudante.Raca);
        Assert.Equal("Feminino", segundoEstudante.Genero);

        _obterSondagemRelatorioPorTurmaUseCaseMock.Verify(
            x => x.ObterSondagemRelatorio(filtro, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ObterRelatorioSondagemPorTurma_DeveRetornarOkComListaVazia_QuandoNaoHouverEstudantes()
    {
        // Arrange
        var filtro = new FiltroQuestionario
        {
            TurmaId = 2,
            ProficienciaId = 1,
            ComponenteCurricularId = 1,
            Modalidade = 5,
            Ano = 2,
            AnoLetivo = 2024,
            SemestreId = 1,
            UeCodigo = "654321"
        };
        var cancellationToken = CancellationToken.None;

        var relatorioEsperado = new QuestionarioSondagemRelatorioDto
        {
            TituloTabelaRespostas = "Relatório Sem Estudantes",
            Estudantes = new List<EstudanteQuestionarioDto>()
        };

        _obterSondagemRelatorioPorTurmaUseCaseMock
            .Setup(x => x.ObterSondagemRelatorio(filtro, cancellationToken))
            .ReturnsAsync(relatorioEsperado);

        // Act
        var result = await _controller.ObterRelatorioSondagemPorTurma(
            filtro,
            _obterSondagemRelatorioPorTurmaUseCaseMock.Object,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var retorno = Assert.IsType<QuestionarioSondagemRelatorioDto>(okResult.Value);

        Assert.Equal(relatorioEsperado.TituloTabelaRespostas, retorno.TituloTabelaRespostas);
        Assert.NotNull(retorno.Estudantes);
        Assert.Empty(retorno.Estudantes);

        _obterSondagemRelatorioPorTurmaUseCaseMock.Verify(
            x => x.ObterSondagemRelatorio(filtro, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ObterRelatorioSondagemPorTurma_DeveRetornarOk_QuandoFiltroForNulo()
    {
        // Arrange
        FiltroQuestionario? filtro = null;
        var cancellationToken = CancellationToken.None;

        var relatorioEsperado = new QuestionarioSondagemRelatorioDto
        {
            TituloTabelaRespostas = "Relatório Filtro Nulo",
            Estudantes = new List<EstudanteQuestionarioDto>()
        };

        _obterSondagemRelatorioPorTurmaUseCaseMock
            .Setup(x => x.ObterSondagemRelatorio(filtro!, cancellationToken))
            .ReturnsAsync(relatorioEsperado);

        // Act
        var result = await _controller.ObterRelatorioSondagemPorTurma(
            filtro!,
            _obterSondagemRelatorioPorTurmaUseCaseMock.Object,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var retorno = Assert.IsType<QuestionarioSondagemRelatorioDto>(okResult.Value);

        Assert.Equal(relatorioEsperado.TituloTabelaRespostas, retorno.TituloTabelaRespostas);
        Assert.NotNull(retorno.Estudantes);
        Assert.Empty(retorno.Estudantes);

        _obterSondagemRelatorioPorTurmaUseCaseMock.Verify(
            x => x.ObterSondagemRelatorio(filtro!, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ObterRelatorioSondagemPorTurma_DeveRetornarOk_QuandoCancellationTokenForCancelado()
    {
        // Arrange
        var filtro = new FiltroQuestionario
        {
            TurmaId = 1,
            ProficienciaId = 1
        };
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        var cancellationToken = cancellationTokenSource.Token;

        var relatorioEsperado = new QuestionarioSondagemRelatorioDto
        {
            TituloTabelaRespostas = "Relatório Token Cancelado",
            Estudantes = new List<EstudanteQuestionarioDto>()
        };

        _obterSondagemRelatorioPorTurmaUseCaseMock
            .Setup(x => x.ObterSondagemRelatorio(filtro, cancellationToken))
            .ReturnsAsync(relatorioEsperado);

        // Act
        var result = await _controller.ObterRelatorioSondagemPorTurma(
            filtro,
            _obterSondagemRelatorioPorTurmaUseCaseMock.Object,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var retorno = Assert.IsType<QuestionarioSondagemRelatorioDto>(okResult.Value);

        Assert.Equal(relatorioEsperado.TituloTabelaRespostas, retorno.TituloTabelaRespostas);
        Assert.NotNull(retorno.Estudantes);
        Assert.Empty(retorno.Estudantes);

        _obterSondagemRelatorioPorTurmaUseCaseMock.Verify(
            x => x.ObterSondagemRelatorio(filtro, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ObterRelatorioSondagemPorTurma_DeveRetornarOk_QuandoRelatorioRetornarNull()
    {
        // Arrange
        var filtro = new FiltroQuestionario
        {
            TurmaId = 1,
            ProficienciaId = 1
        };
        var cancellationToken = CancellationToken.None;

        QuestionarioSondagemRelatorioDto? relatorioEsperado = null;

        _obterSondagemRelatorioPorTurmaUseCaseMock
            .Setup(x => x.ObterSondagemRelatorio(filtro, cancellationToken))
            .ReturnsAsync(relatorioEsperado!);

        // Act
        var result = await _controller.ObterRelatorioSondagemPorTurma(
            filtro,
            _obterSondagemRelatorioPorTurmaUseCaseMock.Object,
            cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Null(okResult.Value);

        _obterSondagemRelatorioPorTurmaUseCaseMock.Verify(
            x => x.ObterSondagemRelatorio(filtro, cancellationToken),
            Times.Once);
    }
}