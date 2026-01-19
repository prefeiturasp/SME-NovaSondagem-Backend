using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Autenticacao;
using SME.Sondagem.Infrastructure.Dtos.Autenticacao;
using Xunit;

namespace SME.Sondagem.API.Teste.Controller;

public class AutenticacaoControllerTeste
{
    private readonly Mock<IAutenticacaoUseCase> mockAuthUseCase;
    private readonly AutenticacaoController controller;
    private string token = "12345678901234567890123456789012";

    public AutenticacaoControllerTeste()
    {
        mockAuthUseCase = new Mock<IAutenticacaoUseCase>();
        controller = new AutenticacaoController(mockAuthUseCase.Object);
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoAuthUseCaseForNulo()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new AutenticacaoController(null!));
        Assert.Equal("authUseCase", exception.ParamName);
    }

    [Fact]
    public async Task Autenticar_DeveRetornarBadRequest_QuandoTokenForNulo()
    {
        string? tokenNulo = null;

        var resultado = await controller.Autenticar(tokenNulo!);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Token da API A é obrigatório.", badRequestResult.Value);
        mockAuthUseCase.Verify(x => x.Autenticar(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Autenticar_DeveRetornarBadRequest_QuandoTokenForVazio()
    {
        string tokenVazio = string.Empty;

        var resultado = await controller.Autenticar(tokenVazio);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Token da API A é obrigatório.", badRequestResult.Value);
        mockAuthUseCase.Verify(x => x.Autenticar(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Autenticar_DeveRetornarBadRequest_QuandoTokenForApenasEspacos()
    {
        string tokenEspacos = "   ";

        var resultado = await controller.Autenticar(tokenEspacos);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Token da API A é obrigatório.", badRequestResult.Value);
        mockAuthUseCase.Verify(x => x.Autenticar(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Autenticar_DeveRetornarOk_QuandoAutenticacaoForBemSucedida()
    {
        var resultadoEsperado = new TokenSondagemDto
        {
            ApiAToken = token
        };
        mockAuthUseCase.Setup(x => x.Autenticar(token))
            .ReturnsAsync(resultadoEsperado);

        var resultado = await controller.Autenticar(token);

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal(resultadoEsperado, okResult.Value);
        mockAuthUseCase.Verify(x => x.Autenticar(token), Times.Once);
    }

    [Fact]
    public async Task Autenticar_DeveRetornarUnauthorized_QuandoLancarUnauthorizedAccessException()
    {
        var tokenInvalido = "token-invalido";
        var mensagemErro = "Token inválido ou expirado.";
        mockAuthUseCase.Setup(x => x.Autenticar(tokenInvalido))
            .ThrowsAsync(new UnauthorizedAccessException(mensagemErro));

        var resultado = await controller.Autenticar(tokenInvalido);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(resultado);
        Assert.Equal(mensagemErro, unauthorizedResult.Value);
        mockAuthUseCase.Verify(x => x.Autenticar(tokenInvalido), Times.Once);
    }

    [Fact]
    public async Task Autenticar_DeveRetornarStatusCode500_QuandoLancarInvalidOperationException()
    {
        var mensagemErro = "Serviço de autenticação indisponível.";
        mockAuthUseCase.Setup(x => x.Autenticar(token))
            .ThrowsAsync(new InvalidOperationException(mensagemErro));

        var resultado = await controller.Autenticar(token);

        var statusCodeResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal($"Erro interno ao processar a autenticação: {mensagemErro}", statusCodeResult.Value);
        mockAuthUseCase.Verify(x => x.Autenticar(token), Times.Once);
    }

    [Fact]
    public async Task Autenticar_DeveRetornarStatusCode500_QuandoLancarExcecaoGenerica()
    {
        mockAuthUseCase.Setup(x => x.Autenticar(token))
            .ThrowsAsync(new Exception("Erro inesperado"));

        var resultado = await controller.Autenticar(token);

        var statusCodeResult = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Ocorreu um erro inesperado durante a autenticação.", statusCodeResult.Value);
        mockAuthUseCase.Verify(x => x.Autenticar(token), Times.Once);
    }
}
