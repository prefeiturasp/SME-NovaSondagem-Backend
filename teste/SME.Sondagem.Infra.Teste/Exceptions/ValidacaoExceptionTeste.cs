using FluentValidation.Results;
using SME.Sondagem.Infra.Exceptions;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Exceptions
{
    public class ValidacaoExceptionTeste
    {
        [Fact]
        public void Deve_Criar_ValidacaoException_Com_Erros()
        {
            var erros = new List<ValidationFailure>
            {
                new ValidationFailure("Campo1", "Mensagem de erro 1"),
                new ValidationFailure("Campo2", "Mensagem de erro 2")
            };

            var ex = new ValidacaoException(erros);

            Assert.Equal(erros, ex.Erros);
        }

        [Fact]
        public void Deve_Retornar_Mensagens_De_Erro()
        {
            var erros = new List<ValidationFailure>
            {
                new ValidationFailure("Campo1", "Mensagem de erro 1"),
                new ValidationFailure("Campo2", "Mensagem de erro 2")
            };

            var ex = new ValidacaoException(erros);
            var mensagens = ex.Mensagens();

            Assert.NotNull(mensagens);
            Assert.Equal(2, mensagens.Count);
            Assert.Contains("Mensagem de erro 1", mensagens);
            Assert.Contains("Mensagem de erro 2", mensagens);
        }

        [Fact]
        public void Deve_Retornar_Lista_Vazia_Quando_Nao_Ha_Erros()
        {
            var erros = new List<ValidationFailure>();
            var ex = new ValidacaoException(erros);

            var mensagens = ex.Mensagens();

            Assert.NotNull(mensagens);
            Assert.Empty(mensagens);
        }
    }
}
