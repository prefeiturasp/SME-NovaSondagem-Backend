using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using Xunit;

namespace SME.Sondagem.API.Teste.Controllers;

public class RelatorioControllerTeste
{
    private readonly Mock<IObterSondagemRelatorioPorTurmaUseCase> _obterSondagemRelatorioPorTurmaUseCaseMock;
    private readonly RelatorioController _controller;

    public RelatorioControllerTeste()
    {
        _obterSondagemRelatorioPorTurmaUseCaseMock = new Mock<IObterSondagemRelatorioPorTurmaUseCase>();
        _controller = new RelatorioController();
    }

    [Fact]
    public async Task ObterRelatorioSondagemPorTurma_DeveRetornarOkComRelatorio()
    {
        var filtro = new FiltroQuestionario
        {
            TurmaId = 1,
            ProficienciaId = 1,
            ComponenteCurricularId = 1,
            Modalidade = 5,
            Ano = 1
        };
        var cancellationToken = CancellationToken.None;

        var relatorioEsperado = new QuestionarioSondagemRelatorioDto
        {
            TituloTabelaRespostas = "Relatório de Sondagem",
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
                }
            }
        };

        _obterSondagemRelatorioPorTurmaUseCaseMock
            .Setup(x => x.ObterSondagemRelatorio(filtro, cancellationToken))
            .ReturnsAsync(relatorioEsperado);

        var result = await _controller.ObterRelatorioSondagemPorTurma(
            filtro,
            _obterSondagemRelatorioPorTurmaUseCaseMock.Object,
            cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var retorno = Assert.IsType<QuestionarioSondagemRelatorioDto>(okResult.Value);

        Assert.Equal(relatorioEsperado.TituloTabelaRespostas, retorno.TituloTabelaRespostas);
        Assert.NotNull(retorno.Estudantes);
        Assert.Single(retorno.Estudantes);

        var estudante = retorno.Estudantes.First();
        Assert.Equal(1001, estudante.Codigo);
        Assert.Equal("João Silva", estudante.Nome);

        _obterSondagemRelatorioPorTurmaUseCaseMock.Verify(
            x => x.ObterSondagemRelatorio(filtro, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ObterRelatorioSondagemPorTurma_DeveRetornarOkComListaVazia_QuandoNaoHouverEstudantes()
    {
        var filtro = new FiltroQuestionario
        {
            TurmaId = 2,
            ProficienciaId = 1
        };
        var cancellationToken = CancellationToken.None;

        var relatorioEsperado = new QuestionarioSondagemRelatorioDto
        {
            TituloTabelaRespostas = "Relatório Vazio",
            Estudantes = new List<EstudanteQuestionarioDto>()
        };

        _obterSondagemRelatorioPorTurmaUseCaseMock
            .Setup(x => x.ObterSondagemRelatorio(filtro, cancellationToken))
            .ReturnsAsync(relatorioEsperado);

        var result = await _controller.ObterRelatorioSondagemPorTurma(
            filtro,
            _obterSondagemRelatorioPorTurmaUseCaseMock.Object,
            cancellationToken);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var retorno = Assert.IsType<QuestionarioSondagemRelatorioDto>(okResult.Value);

        Assert.NotNull(retorno.Estudantes);
        Assert.Empty(retorno.Estudantes);

        _obterSondagemRelatorioPorTurmaUseCaseMock.Verify(
            x => x.ObterSondagemRelatorio(filtro, cancellationToken),
            Times.Once);
    }
}