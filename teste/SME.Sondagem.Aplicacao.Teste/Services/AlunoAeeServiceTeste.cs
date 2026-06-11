using Moq;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Services.SGP;
using SME.Sondagem.Dominio;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Services;
using System.Net;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Services;

public class AlunoAeeServiceTeste
{
    private readonly Mock<IHttpClientFactory> httpClientFactoryMock;

    public AlunoAeeServiceTeste()
    {
        httpClientFactoryMock = new Mock<IHttpClientFactory>();
    }

    [Fact]
    public async Task VerificarAlunosPossuemPlanoAeeAsync_CodigosNulos_DeveRetornarVazio()
    {
        var service = new AlunoAeeService(httpClientFactoryMock.Object);

        var result = await service.VerificarAlunosPossuemPlanoAeeAsync(null!, 1, "123456");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task VerificarAlunosPossuemPlanoAeeAsync_ResponseOk_ComConteudo_DeveMapearCorretamente()
    {
        var codigos = new[] { 1, 2, 3 };
        var planosAee = new[]
        {
            new PlanoAEEResumoIntegracaoDto { CodigoAluno = "1" },
            new PlanoAEEResumoIntegracaoDto { CodigoAluno = "3" }
        };

        var json = JsonConvert.SerializeObject(planosAee);
        var httpClient = HttpClientMockHelper.Create(HttpStatusCode.OK, json);

        httpClientFactoryMock
            .Setup(x => x.CreateClient(ServicoSgpConstants.SERVICO))
            .Returns(httpClient);

        var service = new AlunoAeeService(httpClientFactoryMock.Object);

        var result = await service.VerificarAlunosPossuemPlanoAeeAsync(codigos, 10, "123456");

        Assert.True(result[1]);
        Assert.False(result[2]);
        Assert.True(result[3]);
    }

    [Fact]
    public async Task VerificarAlunosPossuemPlanoAeeAsync_ResponseNoContent_DeveRetornarTodosFalse()
    {
        var codigos = new[] { 10, 20 };
        var httpClient = HttpClientMockHelper.Create(HttpStatusCode.NoContent);

        httpClientFactoryMock
            .Setup(x => x.CreateClient(ServicoSgpConstants.SERVICO))
            .Returns(httpClient);

        var service = new AlunoAeeService(httpClientFactoryMock.Object);

        var result = await service.VerificarAlunosPossuemPlanoAeeAsync(codigos, 10, "123456");

        Assert.All(result.Values, Assert.False);
    }

    [Fact]
    public async Task VerificarAlunosPossuemPlanoAeeAsync_ResponseUnauthorized_DeveLancarRegraNegocioException()
    {
        var codigos = new[] { 5, 6 };
        var httpClient = HttpClientMockHelper.Create(HttpStatusCode.Unauthorized);

        httpClientFactoryMock
            .Setup(x => x.CreateClient(ServicoSgpConstants.SERVICO))
            .Returns(httpClient);

        var service = new AlunoAeeService(httpClientFactoryMock.Object);

        var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            service.VerificarAlunosPossuemPlanoAeeAsync(codigos, 10, "123456"));

        Assert.Equal((int)HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task VerificarAlunosPossuemPlanoAeeAsync_ResponseForbidden_DeveLancarRegraNegocioException()
    {
        var codigos = new[] { 5, 6 };
        var httpClient = HttpClientMockHelper.Create(HttpStatusCode.Forbidden);

        httpClientFactoryMock
            .Setup(x => x.CreateClient(ServicoSgpConstants.SERVICO))
            .Returns(httpClient);

        var service = new AlunoAeeService(httpClientFactoryMock.Object);

        var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            service.VerificarAlunosPossuemPlanoAeeAsync(codigos, 10, "123456"));

        Assert.Equal((int)HttpStatusCode.Forbidden, exception.StatusCode);
    }

    [Fact]
    public async Task VerificarAlunosPossuemPlanoAeeAsync_ResponseErro_DeveLancarRegraNegocioException()
    {
        var codigos = new[] { 5, 6 };
        var httpClient = HttpClientMockHelper.Create(HttpStatusCode.InternalServerError);

        httpClientFactoryMock
            .Setup(x => x.CreateClient(ServicoSgpConstants.SERVICO))
            .Returns(httpClient);

        var service = new AlunoAeeService(httpClientFactoryMock.Object);

        var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            service.VerificarAlunosPossuemPlanoAeeAsync(codigos, 10, "123456"));

        Assert.Equal((int)HttpStatusCode.BadGateway, exception.StatusCode);
    }

    [Fact]
    public async Task VerificarAlunosPossuemPlanoAeeAsync_ResponseOk_ConteudoVazio_DeveRetornarTodosFalse()
    {
        var codigos = new[] { 7, 8 };
        var httpClient = HttpClientMockHelper.Create(HttpStatusCode.OK, string.Empty);

        httpClientFactoryMock
            .Setup(x => x.CreateClient(ServicoSgpConstants.SERVICO))
            .Returns(httpClient);

        var service = new AlunoAeeService(httpClientFactoryMock.Object);

        var result = await service.VerificarAlunosPossuemPlanoAeeAsync(codigos, 10, "123456");

        Assert.All(result.Values, Assert.False);
    }
}
