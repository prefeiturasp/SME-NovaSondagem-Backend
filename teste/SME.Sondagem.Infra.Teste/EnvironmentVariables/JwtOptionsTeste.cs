using SME.Sondagem.Infra.EnvironmentVariables;
using Xunit;

namespace SME.Sondagem.Infra.Teste.EnvironmentVariables
{
    public class JwtOptionsTeste
    {
        [Fact]
        public void Deve_Permitir_Setar_Propriedades()
        {
            var options = new JwtOptions
            {
                Issuer = "issuer",
                Audience = "audience",
                ExpiresInMinutes = "60",
                IssuerSigningKey = "chave"
            };

            Assert.Equal("issuer", options.Issuer);
            Assert.Equal("audience", options.Audience);
            Assert.Equal("60", options.ExpiresInMinutes);
            Assert.Equal("chave", options.IssuerSigningKey);
        }

        [Fact]
        public void Propriedades_Devem_Ser_Nulas_Por_Padrao()
        {
            var options = new JwtOptions();

            Assert.Null(options.Issuer);
            Assert.Null(options.Audience);
            Assert.Null(options.ExpiresInMinutes);
            Assert.Null(options.IssuerSigningKey);
        }

        [Fact]
        public void Deve_Retornar_Secao_Correta()
        {
            Assert.Equal("Jwt", JwtOptions.Secao);
        }
    }
}
