using System.Net;
using Xunit;

namespace SME.Sondagem.Dominio.Teste.Excecoes;

public class RegraNegocioExceptionTeste
{
    [Fact]
    public void DeveCriarExcecaoComMensagemEStatusCodePadrao()
    {
        const string mensagemEsperada = "Erro de regra de negócio";

        var excecao = new RegraNegocioException(mensagemEsperada);

        Assert.Equal(mensagemEsperada, excecao.Message);
        Assert.Equal(601, excecao.StatusCode);
    }

    [Fact]
    public void DeveCriarExcecaoComMensagemEStatusCodeCustomizado()
    {
        const string mensagemEsperada = "Erro de regra de negócio";
        const int statusCodeEsperado = 400;

        var excecao = new RegraNegocioException(mensagemEsperada, statusCodeEsperado);

        Assert.Equal(mensagemEsperada, excecao.Message);
        Assert.Equal(statusCodeEsperado, excecao.StatusCode);
    }

    [Fact]
    public void DeveCriarExcecaoComMensagemEHttpStatusCode()
    {
        const string mensagemEsperada = "Erro de regra de negócio";
        const HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;

        var excecao = new RegraNegocioException(mensagemEsperada, httpStatusCode);

        Assert.Equal(mensagemEsperada, excecao.Message);
        Assert.Equal((int)httpStatusCode, excecao.StatusCode);
        Assert.Equal(400, excecao.StatusCode);
    }

    [Fact]
    public void DeveCriarExcecaoComMensagemEInnerException()
    {
        const string mensagemEsperada = "Erro de regra de negócio";
        var innerException = new InvalidOperationException("Erro interno");

        var excecao = new RegraNegocioException(mensagemEsperada, innerException);

        Assert.Equal(mensagemEsperada, excecao.Message);
        Assert.Equal(innerException, excecao.InnerException);
        Assert.Equal("Erro interno", excecao?.InnerException?.Message ?? string.Empty);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound, 404)]
    [InlineData(HttpStatusCode.Unauthorized, 401)]
    [InlineData(HttpStatusCode.Forbidden, 403)]
    [InlineData(HttpStatusCode.InternalServerError, 500)]
    public void DeveConverterHttpStatusCodeCorretamente(HttpStatusCode httpStatusCode, int statusCodeEsperado)
    {
        const string mensagem = "Erro";

        var excecao = new RegraNegocioException(mensagem, httpStatusCode);

        Assert.Equal(statusCodeEsperado, excecao.StatusCode);
    }

    [Fact]
    public void DeveSerDoTipoException()
    {
        var excecao = new RegraNegocioException("Erro");

        Assert.IsAssignableFrom<Exception>(excecao);
    }

    [Fact]
    public void DevePermitirMensagemVazia()
    {
        const string mensagemVazia = "";

        var excecao = new RegraNegocioException(mensagemVazia);

        Assert.Equal(mensagemVazia, excecao.Message);
        Assert.Equal(601, excecao.StatusCode);
    }
}