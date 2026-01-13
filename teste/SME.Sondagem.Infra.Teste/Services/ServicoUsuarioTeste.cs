using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SME.Sondagem.Infrastructure.Services;
using System.Security.Claims;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Services
{
    public class ServicoUsuarioTeste
    {
        private static IHttpContextAccessor CriarContextoComUsuario(ClaimsPrincipal usuario)
        {
            var httpContext = new DefaultHttpContext
            {
                User = usuario
            };

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns(httpContext);

            return httpContextAccessor.Object;
        }

        #region ObterUsuarioLogado

        [Fact]
        public void ObterUsuarioLogado_deve_retornar_identity_name_quando_existir()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "usuario.teste")
            };

            var identity = new ClaimsIdentity(claims, "Teste");
            var usuario = new ClaimsPrincipal(identity);

            var httpContextAccessor = CriarContextoComUsuario(usuario);
            var servico = new ServicoUsuario(httpContextAccessor);

            var resultado = servico.ObterUsuarioLogado();

            resultado.Should().Be("usuario.teste");
        }

        [Fact]
        public void ObterUsuarioLogado_deve_retornar_claim_name_quando_identity_name_for_nulo()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "usuario.claim")
            };

            var identity = new ClaimsIdentity(claims);
            identity.AddClaim(new Claim(ClaimTypes.Name, "usuario.claim"));

            var usuario = new ClaimsPrincipal(identity);

            var httpContextAccessor = CriarContextoComUsuario(usuario);
            var servico = new ServicoUsuario(httpContextAccessor);

            var resultado = servico.ObterUsuarioLogado();

            resultado.Should().Be("usuario.claim");
        }

        [Fact]
        public void ObterUsuarioLogado_deve_retornar_sistema_quando_nao_houver_usuario()
        {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns((HttpContext?)null);

            var servico = new ServicoUsuario(httpContextAccessor.Object);

            var resultado = servico.ObterUsuarioLogado();

            resultado.Should().Be("Sistema");
        }

        #endregion

        #region ObterRFUsuarioLogado

        [Fact]
        public void ObterRFUsuarioLogado_deve_retornar_claim_RF_maiusculo()
        {
            var claims = new[]
            {
                new Claim("RF", "123456")
            };

            var identity = new ClaimsIdentity(claims, "Teste");
            var usuario = new ClaimsPrincipal(identity);

            var httpContextAccessor = CriarContextoComUsuario(usuario);
            var servico = new ServicoUsuario(httpContextAccessor);

            var resultado = servico.ObterRFUsuarioLogado();

            resultado.Should().Be("123456");
        }

        [Fact]
        public void ObterRFUsuarioLogado_deve_retornar_claim_rf_minusculo()
        {
            var claims = new[]
            {
                new Claim("rf", "654321")
            };

            var identity = new ClaimsIdentity(claims, "Teste");
            var usuario = new ClaimsPrincipal(identity);

            var httpContextAccessor = CriarContextoComUsuario(usuario);
            var servico = new ServicoUsuario(httpContextAccessor);

            var resultado = servico.ObterRFUsuarioLogado();

            resultado.Should().Be("654321");
        }

        [Fact]
        public void ObterRFUsuarioLogado_deve_retornar_zero_quando_nao_existir_claim()
        {
            var identity = new ClaimsIdentity();
            var usuario = new ClaimsPrincipal(identity);

            var httpContextAccessor = CriarContextoComUsuario(usuario);
            var servico = new ServicoUsuario(httpContextAccessor);

            var resultado = servico.ObterRFUsuarioLogado();

            resultado.Should().Be("0");
        }

        [Fact]
        public void ObterRFUsuarioLogado_deve_retornar_zero_quando_httpcontext_for_nulo()
        {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns((HttpContext?)null);

            var servico = new ServicoUsuario(httpContextAccessor.Object);

            var resultado = servico.ObterRFUsuarioLogado();

            resultado.Should().Be("0");
        }

        #endregion
    }
}
