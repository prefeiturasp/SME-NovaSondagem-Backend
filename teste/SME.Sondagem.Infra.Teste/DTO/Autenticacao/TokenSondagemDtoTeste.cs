using SME.Sondagem.Infrastructure.Dtos.Autenticacao;
using Xunit;

namespace SME.Sondagem.Infra.Teste.DTO.Autenticacao
{
    public class TokenSondagemDtoTeste
    {
        [Fact]
        public void Deve_Criar_TokenSondagemDto_Com_Propriedades_Padrao()
        {
            var dto = new TokenSondagemDto();

            Assert.Null(dto.ApiAToken);
        }

        [Fact]
        public void Deve_Definir_E_Obter_ApiAToken()
        {
            var dto = new TokenSondagemDto();
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";

            dto.ApiAToken = token;

            Assert.Equal(token, dto.ApiAToken);
        }

        [Fact]
        public void Deve_Aceitar_ApiAToken_Nulo()
        {
            var dto = new TokenSondagemDto
            {
                ApiAToken = null
            };

            Assert.Null(dto.ApiAToken);
        }

        [Fact]
        public void Deve_Aceitar_ApiAToken_Vazio()
        {
            var dto = new TokenSondagemDto
            {
                ApiAToken = string.Empty
            };

            Assert.Equal(string.Empty, dto.ApiAToken);
        }

        [Fact]
        public void Deve_Aceitar_ApiAToken_Com_Espacos_Em_Branco()
        {
            var dto = new TokenSondagemDto();
            var tokenComEspacos = "   ";

            dto.ApiAToken = tokenComEspacos;

            Assert.Equal(tokenComEspacos, dto.ApiAToken);
        }

        [Fact]
        public void Deve_Permitir_Alteracao_De_ApiAToken()
        {
            var dto = new TokenSondagemDto
            {
                ApiAToken = "token-inicial"
            };

            var novoToken = "token-alterado";
            dto.ApiAToken = novoToken;

            Assert.Equal(novoToken, dto.ApiAToken);
        }

        [Fact]
        public void Deve_Aceitar_ApiAToken_Com_Caracteres_Especiais()
        {
            var dto = new TokenSondagemDto();
            var tokenComCaracteresEspeciais = "Bearer eyJ@#$%&*()_+-=[]{}|;:',.<>?/";

            dto.ApiAToken = tokenComCaracteresEspeciais;

            Assert.Equal(tokenComCaracteresEspeciais, dto.ApiAToken);
        }
    }
}