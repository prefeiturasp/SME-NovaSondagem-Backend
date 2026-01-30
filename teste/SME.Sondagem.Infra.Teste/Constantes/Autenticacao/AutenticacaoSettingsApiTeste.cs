using SME.Sondagem.Infra.Constantes.Autenticacao;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Constantes.Autenticacao
{
    public class AutenticacaoSettingsApiTeste
    {
        [Fact]
        public void Deve_Retornar_Valor_Correto_BearerTokenSondagem()
        {
            Assert.Equal("SondagemTokenSettings", AutenticacaoSettingsApi.BearerTokenSondagem);
        }

        [Fact]
        public void Deve_Retornar_Valor_Correto_BearerTokenSGP()
        {
            Assert.Equal("SGPApiTokenSettings", AutenticacaoSettingsApi.BearerTokenSGP);
        }
    }
}
