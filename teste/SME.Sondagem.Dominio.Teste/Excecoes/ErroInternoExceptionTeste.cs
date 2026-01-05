using Xunit;

namespace SME.Sondagem.Dominio.Teste.Excecoes
{
    public class ErroInternoExceptionTeste
    {
        [Fact]
        public void Construtor_DeveAtribuirMensagemCorretamente()
        {
            var mensagemEsperada = "Erro interno ocorreu no sistema";

            var exception = new ErroInternoException(mensagemEsperada);

            Assert.Equal(mensagemEsperada, exception.Message);
        }

        [Fact]
        public void Construtor_ComInnerException_DeveAtribuirMensagemEInnerExceptionCorretamente()
        {
            var mensagemEsperada = "Erro interno no processamento";
            var innerException = new InvalidOperationException("Operação inválida");

            var exception = new ErroInternoException(mensagemEsperada, innerException);

            Assert.Equal(mensagemEsperada, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
            Assert.Equal("Operação inválida", exception.InnerException.Message);
        }

        [Fact]
        public void Construtor_ComInnerException_DeveManter_StackTrace()
        {
            var innerException = new InvalidOperationException("Erro original");
            var mensagem = "Erro ao processar requisição";

            var exception = new ErroInternoException(mensagem, innerException);

            Assert.NotNull(exception.InnerException);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public void ErroInternoException_DeveHerdarDeException()
        {
            var exception = new ErroInternoException("Teste");

            Assert.IsAssignableFrom<Exception>(exception);
        }

        [Fact]
        public void Construtor_ComMensagemVazia_DeveCriarExcecao()
        {
            var mensagemVazia = string.Empty;

            var exception = new ErroInternoException(mensagemVazia);

            Assert.Equal(mensagemVazia, exception.Message);
        }

        [Fact]
        public void Construtor_ComMensagemNula_DeveCriarExcecaoComMensagemPadrao()
        {
            string? mensagemNula = null;

            var exception = new ErroInternoException(mensagemNula!);

            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
        }
    }
}