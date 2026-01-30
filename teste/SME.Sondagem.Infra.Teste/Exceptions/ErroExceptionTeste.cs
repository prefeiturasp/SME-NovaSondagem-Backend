using System.Net;
using SME.Sondagem.Infra.Exceptions;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Exceptions
{
    public class ErroExceptionTeste
    {
        [Fact]
        public void Deve_Criar_ErroException_Com_StatusCode_Default()
        {
            var ex = new ErroException("mensagem de erro");

            Assert.Equal("mensagem de erro", ex.Message);
            Assert.Equal(500, ex.StatusCode);
        }

        [Fact]
        public void Deve_Criar_ErroException_Com_StatusCode_Int()
        {
            var ex = new ErroException("erro customizado", 404);

            Assert.Equal("erro customizado", ex.Message);
            Assert.Equal(404, ex.StatusCode);
        }

        [Fact]
        public void Deve_Criar_ErroException_Com_StatusCode_HttpStatusCode()
        {
            var ex = new ErroException("erro http", HttpStatusCode.BadRequest);

            Assert.Equal("erro http", ex.Message);
            Assert.Equal((int)HttpStatusCode.BadRequest, ex.StatusCode);
        }
    }
}
