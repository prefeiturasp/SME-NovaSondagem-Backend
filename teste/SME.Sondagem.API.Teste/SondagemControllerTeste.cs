using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.Sondagem.API.Controllers;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Infra.Exceptions;
using SME.Sondagem.Infra.Teste.DTO;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;
using Xunit;

namespace SME.Sondagem.API.Teste;

public class SondagemControllerTeste
{
    private readonly Mock<ISondagemUseCase> _sondagemUseCase;
    private readonly Mock<ISondagemSalvarRespostasUseCase> _sondagemSalvarRespostasUseCase;
    private readonly SondagemController _controller;

    public SondagemControllerTeste()
    {
        _sondagemUseCase = new Mock<ISondagemUseCase>();
        _sondagemSalvarRespostasUseCase = new Mock<ISondagemSalvarRespostasUseCase>();
        _controller = new SondagemController(_sondagemUseCase.Object, _sondagemSalvarRespostasUseCase.Object);
    }
    
    [Fact(DisplayName = "Deve salvar com sucesso as respostas da sondagem")]
    public async Task Deve_salvar_resposta_com_sucesso()
    {
        // Arrange
        var dto = SondagemMockData.ObterSondagemMock();
        var resultadoEsperado = true;

        _sondagemSalvarRespostasUseCase
            .Setup(s => s.SalvarOuAtualizarSondagemAsync(It.Is<SondagemSalvarDto>(d => 
                d.SondagemId == dto.SondagemId && 
                d.Alunos.Any())))
            .ReturnsAsync(resultadoEsperado);

        // Act
        var resultado = await _controller.Post(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.NotNull(okResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.NotNull(okResult.Value);
        Assert.Equal(resultadoEsperado, okResult.Value);
        Assert.IsType<bool>(okResult.Value);

        _sondagemSalvarRespostasUseCase.Verify(
            s => s.SalvarOuAtualizarSondagemAsync(It.Is<SondagemSalvarDto>(d => 
                d.SondagemId == dto.SondagemId)), 
            Times.Once);

        _sondagemSalvarRespostasUseCase.VerifyNoOtherCalls();
    }
    
    [Fact(DisplayName = "Deve retornar erro quando sondagem ativa for null")]
    public async Task Deve_retornar_erro_quando_sondagem_ativa_null()
    {
        // Arrange
        var dto = SondagemMockData.ObterSondagemMock();
        var mensagemEsperada = MensagemNegocioComuns.NENHUM_SONDAGEM_ATIVA_ENCONRADA;
        var negocioException = new NegocioException(mensagemEsperada);

        _sondagemSalvarRespostasUseCase.Setup(s => s.SalvarOuAtualizarSondagemAsync(dto))
            .ThrowsAsync(negocioException);

        // Act
        var resultado = await _controller.Post(dto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.NotNull(objectResult);
        Assert.Equal(negocioException.StatusCode, objectResult.StatusCode);

        var resultValue = objectResult.Value;
        var mensagemProperty = resultValue?.GetType().GetProperty("mensagem");
        var mensagemRetornada = mensagemProperty?.GetValue(resultValue)?.ToString();

        Assert.Equal(mensagemEsperada, mensagemRetornada);

        _sondagemSalvarRespostasUseCase.Verify(s => s.SalvarOuAtualizarSondagemAsync(dto), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar erro quando SondagemId não corresponder à sondagem ativa")]
    public async Task Deve_retornar_erro_quando_sondagem_id_diferente_da_ativa()
    {
        // Arrange
        var dto = SondagemMockData.ObterSondagemMock();
        var mensagemEsperada = MensagemNegocioComuns.SALVAR_SOMENTE_PARA_SONDAGEM_ATIVA;

        var negocioException = new NegocioException(mensagemEsperada);

        _sondagemSalvarRespostasUseCase.Setup(s => s.SalvarOuAtualizarSondagemAsync(dto))
            .ThrowsAsync(negocioException);

        // Act
        var resultado = await _controller.Post(dto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(resultado);
        Assert.NotNull(objectResult);
        Assert.Equal(negocioException.StatusCode, objectResult.StatusCode);

        var resultValue = objectResult.Value;
        var mensagemProperty = resultValue?.GetType().GetProperty("mensagem");
        var mensagemRetornada = mensagemProperty?.GetValue(resultValue)?.ToString();

        Assert.Equal(mensagemEsperada, mensagemRetornada);

        _sondagemSalvarRespostasUseCase.Verify(s => s.SalvarOuAtualizarSondagemAsync(dto), Times.Once);
    }
}
