using System.Net;
using SME.Sondagem.Infra.Exceptions;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Exceptions
{
    public class NegocioExceptionTeste
    {
        [Fact]
        public void Deve_Criar_NegocioException_Com_StatusCode_Default()
        {
            var ex = new NegocioException("mensagem de negócio");

            Assert.Equal("mensagem de negócio", ex.Message);
            Assert.Equal(409, ex.StatusCode);
        }

        [Fact]
        public void Deve_Criar_NegocioException_Com_StatusCode_Int()
        {
            var ex = new NegocioException("erro customizado", 422);

            Assert.Equal("erro customizado", ex.Message);
            Assert.Equal(422, ex.StatusCode);
        }

        [Fact]
        public void Deve_Criar_NegocioException_Com_StatusCode_HttpStatusCode()
        {
            var ex = new NegocioException("erro http", HttpStatusCode.BadRequest);

            Assert.Equal("erro http", ex.Message);
            Assert.Equal((int)HttpStatusCode.BadRequest, ex.StatusCode);
        }
    }
}
