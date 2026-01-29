using SME.Sondagem.Infrastructure.Interfaces;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Interfaces
{
    public class IServicoUsuarioTeste
    {
        private class ServicoUsuarioMock : IServicoUsuario
        {
            public string ObterUsuarioLogado()
            {
                return "usuario.teste";
            }

            public string ObterRFUsuarioLogado()
            {
                return "1234567";
            }
        }

        [Fact]
        public void Deve_ObterUsuarioLogado()
        {
            var servico = new ServicoUsuarioMock();
            var usuario = servico.ObterUsuarioLogado();

            Assert.Equal("usuario.teste", usuario);
        }

        [Fact]
        public void Deve_ObterRFUsuarioLogado()
        {
            var servico = new ServicoUsuarioMock();
            var rf = servico.ObterRFUsuarioLogado();

            Assert.Equal("1234567", rf);
        }
    }
}
