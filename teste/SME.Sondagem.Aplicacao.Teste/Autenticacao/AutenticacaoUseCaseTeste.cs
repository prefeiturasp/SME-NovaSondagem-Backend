using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SME.Sondagem.Aplicacao.UseCases.Autenticacao;
using System.Security.Claims;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Autenticacao
{
    public class AutenticacaoUseCaseTeste
    {
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<IAuthenticationService> authenticationServiceMock;
        private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;

        private readonly AutenticacaoUseCase useCase;

        public AutenticacaoUseCaseTeste()
        {
            configurationMock = new Mock<IConfiguration>();
            authenticationServiceMock = new Mock<IAuthenticationService>();
            httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new ServiceCollection()
                .AddSingleton(authenticationServiceMock.Object)
                .BuildServiceProvider();

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            configurationMock.Setup(c => c["SondagemTokenSettings:Issuer"]).Returns("issuer");
            configurationMock.Setup(c => c["SondagemTokenSettings:Audience"]).Returns("audience");
            configurationMock.Setup(c => c["SondagemTokenSettings:IssuerSigningKey"])
                             .Returns("12345678901234567890123456789012");

            useCase = new AutenticacaoUseCase(
                configurationMock.Object,
                authenticationServiceMock.Object,
                httpContextAccessorMock.Object);
        }

        private static ClaimsPrincipal CriarPrincipalValido(bool comRoles = true)
        {
            var claims = new List<Claim>
            {
                new Claim("login", "usuario.teste"),
                new Claim("nome", "Usuário Teste"),
                new Claim("rf", "123456"),
                new Claim("perfil", "Administrador")
            };

            if (comRoles)
                claims.Add(new Claim("roles", "ADMIN"));

            var identity = new ClaimsIdentity(claims, "Bearer");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task Autenticar_DeveGerarTokenComSucesso()
        {
            var principal = CriarPrincipalValido();

            authenticationServiceMock
                .Setup(a => a.AuthenticateAsync(
                    It.IsAny<HttpContext>(),
                    "SGPApiTokenSettings"))
                .ReturnsAsync(AuthenticateResult.Success(
                    new AuthenticationTicket(principal, "SGPApiTokenSettings")));

            var resultado = await useCase.Autenticar("token-valido");

            Assert.NotNull(resultado);
            Assert.False(string.IsNullOrEmpty(resultado.ApiAToken));
        }

        [Fact]
        public async Task Autenticar_DeveLancarUnauthorized_QuandoTokenInvalido()
        {
            authenticationServiceMock
                .Setup(a => a.AuthenticateAsync(
                    It.IsAny<HttpContext>(),
                    "SGPApiTokenSettings"))
                .ReturnsAsync(AuthenticateResult.Fail("Token inválido"));

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                useCase.Autenticar("token-invalido"));
        }

        [Fact]
        public async Task Autenticar_DeveLancarErro_QuandoHttpContextForNulo()
        {
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

            var novoUseCase = new AutenticacaoUseCase(
                configurationMock.Object,
                authenticationServiceMock.Object,
                httpContextAccessorMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                novoUseCase.Autenticar("token"));
        }

        [Fact]
        public async Task Autenticar_DeveLancarErro_QuandoClaimsObrigatoriasAusentes()
        {
            var claims = new List<Claim>
            {
                new Claim("rf", "123"),
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            authenticationServiceMock
                .Setup(a => a.AuthenticateAsync(
                    It.IsAny<HttpContext>(),
                    "SGPApiTokenSettings"))
                .ReturnsAsync(AuthenticateResult.Success(
                    new AuthenticationTicket(principal, "SGPApiTokenSettings")));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                useCase.Autenticar("token"));
        }

        [Fact]
        public async Task Autenticar_DeveFuncionar_SemRoles()
        {
            var principal = CriarPrincipalValido(comRoles: false);

            authenticationServiceMock
                .Setup(a => a.AuthenticateAsync(
                    It.IsAny<HttpContext>(),
                    "SGPApiTokenSettings"))
                .ReturnsAsync(AuthenticateResult.Success(
                    new AuthenticationTicket(principal, "SGPApiTokenSettings")));

            var resultado = await useCase.Autenticar("token");

            Assert.NotNull(resultado.ApiAToken);
        }

        [Fact]
        public async Task Autenticar_DeveLancarErro_QuandoSigningKeyNaoConfigurada()
        {
            configurationMock.Setup(c => c["SondagemTokenSettings:IssuerSigningKey"])
                             .Returns((string)null);

            var principal = CriarPrincipalValido();

            authenticationServiceMock
                .Setup(a => a.AuthenticateAsync(
                    It.IsAny<HttpContext>(),
                    "SGPApiTokenSettings"))
                .ReturnsAsync(AuthenticateResult.Success(
                    new AuthenticationTicket(principal, "SGPApiTokenSettings")));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                useCase.Autenticar("token"));
        }
    }
}