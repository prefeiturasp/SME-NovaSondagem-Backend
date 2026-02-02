using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using System.Security.Claims;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Services
{
    public class ControleAcessoServiceTeste
    {
        private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
        private readonly Mock<IRepositorioCache> repositorioCache;
        private readonly Mock<IRepositorioElasticTurma> repositorioElasticTurma;

        private const string TURMA_ID = "TURMA-TESTE";

        public ControleAcessoServiceTeste()
        {
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            repositorioCache = new Mock<IRepositorioCache>();
            repositorioElasticTurma = new Mock<IRepositorioElasticTurma>();
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
                repositorioCache.Object,
                repositorioElasticTurma.Object);

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
                repositorioCache.Object,
                repositorioElasticTurma.Object);

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
                repositorioCache.Object,
                repositorioElasticTurma.Object);

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
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_PerfilNaoProfessor_SemUeNaAbrangencia_DeveRetornarFalse()
        {
            var perfil = ControleAcessoService.PERFIL_CP.ToString();
            var turmaId = "123456";

            var accessor = CriarHttpContextAccessor(
                true,
                rf: "123",
                perfil: perfil);

            repositorioElasticTurma
                .Setup(r => r.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    CodigoEscola = "999999"
                });

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(JsonConvert.SerializeObject(new[]
                {
                    new ControleAcessoDto
                    {
                        IdUes = new[] { "111111", "222222" }
                    }
                }));

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(turmaId);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Professor_SemTurmaPermitida_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: "123",
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(JsonConvert.SerializeObject(new[]
                {
                    new ControleAcessoDto
                    {
                        Regencia = true,
                        TurmaCodigos = new[] { "999" }
                    }
                }));

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Professor_ComTurmaPermitida_DeveRetornarTrue()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: "123",
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(JsonConvert.SerializeObject(new[]
                {
                    new ControleAcessoDto
                    {
                        Regencia = true,
                        TurmaCodigos = new[] { TURMA_ID }
                    }
                }));

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.True(result);
        }
    }
}
