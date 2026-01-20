using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.API.Controllers.Integracao;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.API.Teste.Controller;

public class QuestionarioIntegracaoControllerTeste
{
    private readonly Mock<IObterQuestionariosUseCase> obterMock = new();
    private readonly Mock<IObterQuestionarioPorIdUseCase> obterPorIdMock = new();
    private readonly Mock<ICriarQuestionarioUseCase> criarMock = new();
    private readonly Mock<IAtualizarQuestionarioUseCase> atualizarMock = new();
    private readonly Mock<IExcluirQuestionarioUseCase> excluirMock = new();

    private QuestionarioIntegracaoController CriarController()
        => new(
            obterMock.Object,
            obterPorIdMock.Object,
            criarMock.Object,
            atualizarMock.Object,
            excluirMock.Object
        );

    private static QuestionarioDto CriarDto() => new()
    {
        Nome = "Nome Teste",
        Tipo = TipoQuestionario.SondagemEscrita,
        AnoLetivo = 2024,
        ComponenteCurricularId = 1,
        ProficienciaId = 1,
        SondagemId = 1
    };

    [Fact]
    public async Task Get_DeveRetornarOk()
    {
        obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new[] { CriarDto() });

        var controller = CriarController();

        var result = await controller.Get(CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Get_DevePropagarException()
    {
        obterMock.Setup(x => x.ExecutarAsync(It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception());

        var controller = CriarController();

        await controller
            .Invoking(c => c.Get(CancellationToken.None))
            .Should()
            .ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetById_DeveRetornarOk()
    {
        obterPorIdMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(CriarDto());

        var controller = CriarController();

        var result = await controller.GetById(1, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_DeveLancarErroNaoEncontrado()
    {
        obterPorIdMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((QuestionarioDto?)null);

        var controller = CriarController();

        await controller
            .Invoking(c => c.GetById(1, CancellationToken.None))
            .Should()
            .ThrowAsync<ErroNaoEncontradoException>();
    }

    [Fact]
    public async Task Create_DeveRetornarCreated()
    {
        criarMock.Setup(x => x.ExecutarAsync(It.IsAny<QuestionarioDto>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(1);

        var controller = CriarController();

        var result = await controller.Create(CriarDto(), CancellationToken.None);

        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Create_DevePropagarValidationException()
    {
        criarMock.Setup(x => x.ExecutarAsync(It.IsAny<QuestionarioDto>(), It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new FluentValidation.ValidationException("erro"));

        var controller = CriarController();

        await controller
            .Invoking(c => c.Create(CriarDto(), CancellationToken.None))
            .Should()
            .ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task Create_DevePropagarRegraNegocioException()
    {
        criarMock.Setup(x => x.ExecutarAsync(It.IsAny<QuestionarioDto>(), It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new RegraNegocioException("erro", 409));

        var controller = CriarController();

        await controller
            .Invoking(c => c.Create(CriarDto(), CancellationToken.None))
            .Should()
            .ThrowAsync<RegraNegocioException>();
    }

    [Fact]
    public async Task Atualizar_DeveRetornarOk()
    {
        atualizarMock.Setup(x => x.ExecutarAsync(1, It.IsAny<QuestionarioDto>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(CriarDto());

        var controller = CriarController();

        var result = await controller.Atualizar(1, CriarDto(), CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Atualizar_DeveLancarErroNaoEncontrado()
    {
        atualizarMock.Setup(x => x.ExecutarAsync(1, It.IsAny<QuestionarioDto>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((QuestionarioDto?)null);

        var controller = CriarController();

        await controller
            .Invoking(c => c.Atualizar(1, CriarDto(), CancellationToken.None))
            .Should()
            .ThrowAsync<ErroNaoEncontradoException>();
    }

    [Fact]
    public async Task Excluir_DeveRetornarNoContent()
    {
        excluirMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(true);

        var controller = CriarController();

        var result = await controller.Excluir(1, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Excluir_DeveLancarErroNaoEncontrado()
    {
        excluirMock.Setup(x => x.ExecutarAsync(1, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(false);

        var controller = CriarController();

        await controller
            .Invoking(c => c.Excluir(1, CancellationToken.None))
            .Should()
            .ThrowAsync<ErroNaoEncontradoException>();
    }
}
