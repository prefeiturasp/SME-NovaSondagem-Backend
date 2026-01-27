using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Services
{
    public class ControleAcessoServiceTeste
    {
        private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
        private readonly Mock<IRepositorioCache> repositorioCache;
        private const string TURMA_ID = "TURMA-TESTE";

        public ControleAcessoServiceTeste()
        {
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            repositorioCache = new Mock<IRepositorioCache>();
        }

        private static HttpContextAccessor CriarHttpContextAccessor(
            bool autenticado,
            string? rf = null,
            string? perfil = null)
        {
            var context = new DefaultHttpContext();

            if (autenticado)
            {
                var claims = new List<Claim>();

                if (!string.IsNullOrEmpty(rf))
                    claims.Add(new Claim("rf", rf));

                if (!string.IsNullOrEmpty(perfil))
                    claims.Add(new Claim("perfil", perfil));

                context.User = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "TestAuth"));
            }

            return new HttpContextAccessor { HttpContext = context };
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_UsuarioNaoAutenticado_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(false);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,                
                accessor,
                repositorioCache.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_RfAusente_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_PerfilAusente_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: "123");

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_PerfilInvalido_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: "123",
                perfil: "INVALIDO");

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_PerfilDiferenteProfessor_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: "123",
                perfil: Guid.NewGuid().ToString());

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_ResponseErro_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: "123",
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.InternalServerError);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(httpClient);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_ResponseNoContent_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: "123",
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.NoContent);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(httpClient);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_ResponseOk_ConteudoVazio_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: "123",
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var httpClient = HttpClientMockHelper.Create(
                HttpStatusCode.OK,
                string.Empty);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(httpClient);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_SemComponentePermitido_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: "123",
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var json = JsonConvert.SerializeObject(new[]
            {
                new ControleAcessoDto { Codigo = 999 }
            });

            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.OK, json);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(httpClient);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_ComComponentePermitido_DeveRetornarTrue()
        {
            // Arrange
            var accessor = CriarHttpContextAccessor(
                autenticado: true,
                rf: "123",
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString() // 👈 GUID correto
            );

            var json = JsonConvert.SerializeObject(new[]
            {
                new ControleAcessoDto
                {
                    Regencia = true,           
                    TurmaCodigo = TURMA_ID
                }
            });

            var httpClient = HttpClientMockHelper.Create(HttpStatusCode.OK, json);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(ServicoEolConstants.SERVICO))
                .Returns(httpClient);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object);

            // Act
            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            // Assert
            Assert.True(result);
        }
    }
}
