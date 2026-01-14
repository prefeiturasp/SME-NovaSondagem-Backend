using Xunit;

namespace SME.Sondagem.Dominio.Teste.Excecoes;

public class ErroNaoEncontradoExceptionTeste
{
    [Fact]
    public void Construtor_DeveCriarExcecaoComMensagem()
    {
        const string mensagemEsperada = "Recurso não encontrado";

        var exception = new ErroNaoEncontradoException(mensagemEsperada);

        Assert.Equal(mensagemEsperada, exception.Message);
    }

    [Fact]
    public void Construtor_DeveCriarExcecaoComMensagemVazia()
    {
        const string mensagemEsperada = "";

        var exception = new ErroNaoEncontradoException(mensagemEsperada);

        Assert.Equal(mensagemEsperada, exception.Message);
    }

    [Fact]
    public void Construtor_DeveCriarExcecaoComMensagemEInnerException()
    {
        const string mensagemEsperada = "Recurso não encontrado";
        var innerException = new InvalidOperationException("Erro interno");

        var exception = new ErroNaoEncontradoException(mensagemEsperada, innerException);

        Assert.Equal(mensagemEsperada, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void Construtor_DeveHerdarDeException()
    {
        var exception = new ErroNaoEncontradoException("Teste");

        Assert.IsType<ErroNaoEncontradoException>(exception);
        Assert.True(exception is Exception);
    }

    [Fact]
    public void Construtor_DeveCriarExcecaoComInnerExceptionNulo()
    {
        const string mensagemEsperada = "Recurso não encontrado";
        Exception? innerException = null;

        var exception = new ErroNaoEncontradoException(mensagemEsperada, innerException!);

        Assert.Equal(mensagemEsperada, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Theory]
    [InlineData("Turma não localizada")]
    [InlineData("Aluno não encontrado")]
    [InlineData("Questionário não existe")]
    public void Construtor_DeveCriarExcecaoComDiferentesMensagens(string mensagem)
    {
        var exception = new ErroNaoEncontradoException(mensagem);

        Assert.Equal(mensagem, exception.Message);
    }

    [Fact]
    public void Exception_DevePossuirMensagemOriginalQuandoLancada()
    {
        const string mensagemEsperada = "Sondagem não encontrada";

        try
        {
            throw new ErroNaoEncontradoException(mensagemEsperada);
        }
        catch (ErroNaoEncontradoException ex)
        {
            Assert.Equal(mensagemEsperada, ex.Message);
        }
    }

    [Fact]
    public async Task Exception_DevePreservarStackTraceQuandoLancada()
    {
        var exception = await Assert.ThrowsAsync<ErroNaoEncontradoException>(async () =>
        {
            throw new ErroNaoEncontradoException("Teste");
        });

        Assert.NotNull(exception.StackTrace);
    }

    [Fact]
    public void Exception_DeveSerCapturadaComoExceptionGenerica()
    {
        const string mensagemEsperada = "Teste de exceção";

        try
        {
            throw new ErroNaoEncontradoException(mensagemEsperada);
        }
        catch (Exception ex)
        {
            Assert.IsType<ErroNaoEncontradoException>(ex);
            Assert.Equal(mensagemEsperada, ex.Message);
        }
    }

    [Fact]
    public void Exception_DeveEncapsularInnerExceptionCorretamente()
    {
        const string mensagemExterna = "Erro ao buscar dados";
        const string mensagemInterna = "Conexão com banco falhou";
        var innerException = new InvalidOperationException(mensagemInterna);

        var exception = new ErroNaoEncontradoException(mensagemExterna, innerException);

        Assert.Equal(mensagemExterna, exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.Equal(mensagemInterna, exception.InnerException.Message);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }
}