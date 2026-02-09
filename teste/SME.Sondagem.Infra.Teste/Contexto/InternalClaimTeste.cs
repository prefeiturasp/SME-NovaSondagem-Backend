using SME.Sondagem.Infra.Contexto;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Contexto
{
    public class InternalClaimTeste
    {
        [Fact]
        public void Deve_Permitir_Setar_Propriedades()
        {
            var claim = new InternalClaim
            {
                Value = "valor",
                Type = "tipo"
            };

            Assert.Equal("valor", claim.Value);
            Assert.Equal("tipo", claim.Type);
        }

        [Fact]
        public void Propriedades_Devem_Ser_Nulas_Por_Padrao()
        {
            var claim = new InternalClaim();

            Assert.Null(claim.Value);
            Assert.Null(claim.Type);
        }
    }
}
