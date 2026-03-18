using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Services
{
    public class ControleAcessoServiceTeste
    {
        private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
        private readonly Mock<IRepositorioCache> repositorioCache;
        private readonly Mock<IRepositorioElasticTurma> repositorioElasticTurma;

        private const string TURMA_ID = "123456";
        private const string TURMA_ID_STRING = "TURMA-TESTE";
        private const string ESCOLA_CODIGO = "ESCOLA-TESTE";
        private const string RF_USUARIO = "123456";
        private const string CODIGO_ESCOLA_PERMITIDA = "111111";
        private const string CODIGO_ESCOLA_NAO_PERMITIDA = "999999";

        private static readonly string[] UES_PERMITIDAS = { "111111", "222222" };

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

        #region Testes Básicos de Validação

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_TurmaIdVazio_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(string.Empty, string.Empty);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_TurmaIdNull_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(null!, null!);

            Assert.False(result);
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

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

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

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_PerfilAusente_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_PerfilInvalido_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: "PERFIL-INVALIDO");

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.False(result);
        }

        #endregion

        #region Testes ADM_SME

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_AdmSme_TurmaExiste_DeveRetornarTrue()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_ADM_SME.ToString());

            repositorioCache
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(
                    It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            repositorioElasticTurma
                .Setup(r => r.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    CodigoTurma = int.Parse(TURMA_ID),
                    CodigoEscola = CODIGO_ESCOLA_PERMITIDA
                });

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.True(result);

            repositorioCache.Verify(
                r => r.SalvarRedisAsync(
                    It.IsAny<string>(),
                    It.IsAny<TurmaElasticDto>(),
                    30),
                Times.Once);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_AdmSme_TurmaNaoExiste_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_ADM_SME.ToString());

            repositorioCache
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(
                    It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            repositorioElasticTurma
                .Setup(r => r.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.False(result);

            repositorioCache.Verify(
                r => r.SalvarRedisAsync(
                    It.IsAny<string>(),
                    It.IsAny<TurmaElasticDto>(),
                    It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_AdmSme_TurmaIdNaoNumerico_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_ADM_SME.ToString());

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync("TURMA-ABC", "ESCOLA-ABC");

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_AdmSme_TurmaNoCache_DeveUsarCache()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_ADM_SME.ToString());

            var turmaCached = new TurmaElasticDto
            {
                CodigoTurma = int.Parse(TURMA_ID),
                CodigoEscola = CODIGO_ESCOLA_PERMITIDA
            };

            repositorioCache
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(
                    It.IsAny<string>()))
                .ReturnsAsync(turmaCached);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.True(result);

            repositorioElasticTurma.Verify(
                r => r.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        #endregion

        #region Testes PROFESSOR

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Professor_ComRegenciaETurmaPermitida_DeveRetornarTrue()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var cacheJsonEol = JsonConvert.SerializeObject(new[]
            {
                new
                {
                    regencia = true,
                    turmaCodigo = TURMA_ID_STRING
                }
            });

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.True(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Professor_SemRegencia_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var cacheJsonEol = JsonConvert.SerializeObject(new[]
            {
                new
                {
                    regencia = false,
                    turmaCodigo = TURMA_ID_STRING
                }
            });

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Professor_TurmaDiferente_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var cacheJsonEol = JsonConvert.SerializeObject(new[]
            {
                new
                {
                    regencia = true,
                    turmaCodigo = "OUTRA-TURMA"
                }
            });

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Professor_ComAcessosVazios_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var cacheJsonEol = JsonConvert.SerializeObject(Array.Empty<object>());

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Professor_MultiplaTurmas_ComTurmaCorreta_DeveRetornarTrue()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var cacheJsonEol = JsonConvert.SerializeObject(new[]
            {
                new { regencia = true, turmaCodigo = "TURMA-1" },
                new { regencia = true, turmaCodigo = TURMA_ID_STRING },
                new { regencia = true, turmaCodigo = "TURMA-3" }
            });

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.True(result);
        }

        #endregion

        #region Testes CP/AD/DIRETOR

        [Theory]
        [InlineData("44e1e074-37d6-e911-abd6-f81654fe895d")]
        [InlineData("45e1e074-37d6-e911-abd6-f81654fe895d")]
        [InlineData("46e1e074-37d6-e911-abd6-f81654fe895d")]
        public async Task ValidarPermissaoAcessoAsync_PerfilGestao_UePermitida_DeveRetornarTrue(
            string perfilGuid)
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: perfilGuid);

            var cacheJsonEol = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            repositorioCache
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(
                    It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            repositorioElasticTurma
                .Setup(r => r.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    CodigoTurma = int.Parse(TURMA_ID),
                    CodigoEscola = CODIGO_ESCOLA_PERMITIDA
                });

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.True(result);
        }

        [Theory]
        [InlineData("44e1e074-37d6-e911-abd6-f81654fe895d")]
        [InlineData("45e1e074-37d6-e911-abd6-f81654fe895d")]
        [InlineData("46e1e074-37d6-e911-abd6-f81654fe895d")]
        public async Task ValidarPermissaoAcessoAsync_PerfilGestao_UeNaoPermitida_DeveRetornarFalse(
            string perfilGuid)
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: perfilGuid);

            var cacheJsonEol = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            repositorioCache
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(
                    It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            repositorioElasticTurma
                .Setup(r => r.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    CodigoTurma = int.Parse(TURMA_ID),
                    CodigoEscola = CODIGO_ESCOLA_NAO_PERMITIDA
                });

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_CP_TurmaNaoExiste_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_CP.ToString());

            var cacheJsonEol = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            repositorioCache
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(
                    It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            repositorioElasticTurma
                .Setup(r => r.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Diretor_TurmaIdNaoNumerico_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_DIRETOR.ToString());

            var cacheJsonEol = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync("TURMA-ABC", "ESCOLA-ABC");

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_CP_TurmaNoCache_DeveUsarCache()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_CP.ToString());

            var cacheJsonEol = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            var turmaCached = new TurmaElasticDto
            {
                CodigoTurma = int.Parse(TURMA_ID),
                CodigoEscola = CODIGO_ESCOLA_PERMITIDA
            };

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            repositorioCache
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(
                    It.IsAny<string>()))
                .ReturnsAsync(turmaCached);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.True(result);

            repositorioElasticTurma.Verify(
                r => r.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        #endregion

        #region Testes de Cache

        [Fact]
        public async Task ObterTurmaComCache_PrimeiraConsulta_DeveBuscarElasticESalvarCache()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_ADM_SME.ToString());

            var turmaElastic = new TurmaElasticDto
            {
                CodigoTurma = int.Parse(TURMA_ID),
                CodigoEscola = CODIGO_ESCOLA_PERMITIDA
            };

            repositorioCache
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(
                    It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            repositorioElasticTurma
                .Setup(r => r.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmaElastic);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.True(result);

            repositorioElasticTurma.Verify(
                r => r.ObterTurmaPorId(
                    It.Is<FiltroQuestionario>(f => f.TurmaId == int.Parse(TURMA_ID)),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            repositorioCache.Verify(
                r => r.SalvarRedisAsync(
                    $"turma-elastic:{TURMA_ID}",
                    turmaElastic,
                    30),
                Times.Once);
        }

        [Fact]
        public async Task ObterTurmaComCache_SegundaConsulta_DeveUsarApenaCache()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_ADM_SME.ToString());

            var turmaCached = new TurmaElasticDto
            {
                CodigoTurma = int.Parse(TURMA_ID),
                CodigoEscola = CODIGO_ESCOLA_PERMITIDA
            };

            repositorioCache
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(
                    It.IsAny<string>()))
                .ReturnsAsync(turmaCached);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.True(result);

            repositorioElasticTurma.Verify(
                r => r.ObterTurmaPorId(
                    It.IsAny<FiltroQuestionario>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            repositorioCache.Verify(
                r => r.SalvarRedisAsync(
                    It.IsAny<string>(),
                    It.IsAny<TurmaElasticDto>(),
                    It.IsAny<int>()),
                Times.Never);
        }

        #endregion

        #region Novos Testes – ValidarPermissaoAcessoAsync e ObterControleAcessoUsuarioAutenticadoAsync

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_CodigoEscolaVazio_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, string.Empty);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_CodigoEscolaNull_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, null!);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Professor_SemAcessos_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            // Forçar ObterControleAcessoUsuarioAutenticadoAsync a retornar vazio:
            // Perfil é válido, mas cache vazio e HTTP retornando NoContent
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            var service = new ControleAcessoService(
                httpClientFactory.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_PerfilGestao_SemAcessos_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_CP.ToString());

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(string.Empty)
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            var service = new ControleAcessoService(
                httpClientFactory.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.False(result);
        }

        [Fact]
        public async Task ObterControleAcessoUsuarioAutenticadoAsync_UsaCache_Professor_DeveEvitarChamadaHttp()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            var cacheJsonEol = JsonConvert.SerializeObject(new[]
            {
                new { regencia = true, turmaCodigo = TURMA_ID_STRING }
            });

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJsonEol);

            var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);

            var service = new ControleAcessoService(
                httpClientFactory.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var resultado = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.True(resultado);
            httpClientFactory.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ObterControleAcessoUsuarioAutenticadoAsync_SemCache_Professor_ChamaHttpESalvaCache()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_PROFESSOR.ToString());

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            var jsonRetorno = JsonConvert.SerializeObject(new[]
            {
                new { regencia = true, turmaCodigo = TURMA_ID_STRING }
            });

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonRetorno)
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var resultado = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING, ESCOLA_CODIGO);

            Assert.True(resultado);

            repositorioCache.Verify(
        r => r.SalvarRedisToJsonAsync(
            It.IsAny<string>(),
            jsonRetorno,
            It.IsAny<int>()),
        Times.Once);
        }

        [Fact]
        public async Task ObterControleAcessoUsuarioAutenticadoAsync_SemCache_Gestao_ChamaHttpESalvaCache()
        {
            var accessor = CriarHttpContextAccessor(
                true,
                rf: RF_USUARIO,
                perfil: ControleAcessoService.PERFIL_CP.ToString());

            repositorioCache
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            var jsonRetorno = JsonConvert.SerializeObject(new
            {
                idUes = UES_PERMITIDAS
            });

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonRetorno)
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var turmaElastic = new TurmaElasticDto
            {
                CodigoTurma = int.Parse(TURMA_ID),
                CodigoEscola = CODIGO_ESCOLA_PERMITIDA
            };

            repositorioCache
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(It.IsAny<string>()))
                .ReturnsAsync(turmaElastic);

            var service = new ControleAcessoService(
                httpClientFactoryMock.Object,
                accessor,
                repositorioCache.Object,
                repositorioElasticTurma.Object);

            var resultado = await service.ValidarPermissaoAcessoAsync(TURMA_ID, ESCOLA_CODIGO);

            Assert.True(resultado);

            repositorioCache.Verify(
              r => r.SalvarRedisToJsonAsync(
                  It.IsAny<string>(),
                  jsonRetorno,
                  It.IsAny<int>()),
              Times.Once);
        }

        #endregion
    }
}