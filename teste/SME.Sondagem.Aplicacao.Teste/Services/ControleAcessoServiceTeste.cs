using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos;
using System.Security.Claims;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Services
{
    public class ControleAcessoServiceTeste
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IRepositorioCache> _repositorioCacheMock;
        private readonly Mock<IRepositorioElasticTurma> _repositorioElasticTurmaMock;
        private readonly Mock<IPerfilService> _perfilServiceMock;

        private const string TURMA_ID = "123456";
        private const string TURMA_ID_STRING = "TURMA-TESTE";
        private const string RF_USUARIO = "123456";
        private const string CODIGO_ESCOLA_PERMITIDA = "111111";
        private const string CODIGO_ESCOLA_NAO_PERMITIDA = "999999";

        private static readonly string[] UES_PERMITIDAS = { "111111", "222222" };

        // GUIDs fictícios para simular perfis distintos nos testes
        private static readonly Guid PERFIL_IRRESTRITO_ID = Guid.NewGuid();
        private static readonly Guid PERFIL_PROFESSOR_ID = Guid.NewGuid();
        private static readonly Guid PERFIL_GESTAO_ID = Guid.NewGuid();

        public ControleAcessoServiceTeste()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _repositorioCacheMock = new Mock<IRepositorioCache>();
            _repositorioElasticTurmaMock = new Mock<IRepositorioElasticTurma>();
            _perfilServiceMock = new Mock<IPerfilService>();
        }

        private ControleAcessoService CriarService(IHttpContextAccessor accessor)
        {
            return new ControleAcessoService(
                _httpClientFactoryMock.Object,
                accessor,
                _repositorioCacheMock.Object,
                _repositorioElasticTurmaMock.Object,
                _perfilServiceMock.Object);
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

        private static PerfilInfoSondagemDto CriarPerfilIrrestrito() => new()
        {
            Codigo = PERFIL_IRRESTRITO_ID,
            Nome = "Irrestrito",
            PermiteConsultar = true,
            AcessoIrrestrito = true,
            ConsultarAbrangencia = false,
            TipoValidacao = "Irrestrito"
        };

        private static PerfilInfoSondagemDto CriarPerfilRegencia() => new()
        {
            Codigo = PERFIL_PROFESSOR_ID,
            Nome = "Professor",          
            PermiteConsultar = true,
            AcessoIrrestrito = false,
            ConsultarAbrangencia = true,
            TipoValidacao = "Regencia"
        };

        private static PerfilInfoSondagemDto CriarPerfilUE() => new()
        {
            Codigo = PERFIL_GESTAO_ID,
            PermiteConsultar = true,
            Nome = "Gestao UE",
            AcessoIrrestrito = false,
            ConsultarAbrangencia = true,
            TipoValidacao = "UE"
        };

        private static PerfilInfoSondagemDto CriarPerfilSemPermissao() => new()
        {
            Codigo = Guid.NewGuid(),
            PermiteConsultar = false,
            AcessoIrrestrito = false,
            ConsultarAbrangencia = false,
            TipoValidacao = "Nenhum"
        };

        #region Testes Básicos de Validação

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_TurmaIdVazio_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_PROFESSOR_ID.ToString());
            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(string.Empty);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_TurmaIdNull_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_PROFESSOR_ID.ToString());
            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(null!);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_ClaimPerfilAusente_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO);
            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING);

            Assert.False(result);
            _perfilServiceMock.Verify(
                p => p.ObterPerfilPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_ClaimPerfilInvalido_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: "NAO-E-UM-GUID");
            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING);

            Assert.False(result);
            _perfilServiceMock.Verify(
                p => p.ObterPerfilPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_PerfilNaoEncontradoNoServico_DeveRetornarFalse()
        {
            var perfilId = Guid.NewGuid();
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: perfilId.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(perfilId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PerfilInfoSondagemDto?)null);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_PerfilSemPermissaoConsultar_DeveRetornarFalse()
        {
            var perfilId = Guid.NewGuid();
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: perfilId.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(perfilId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilSemPermissao());

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING);

            Assert.False(result);
        }

        #endregion

        #region Testes Perfil com Acesso Irrestrito

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_AcessoIrrestrito_TurmaExiste_DeveRetornarTrue()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_IRRESTRITO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_IRRESTRITO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilIrrestrito());

            _repositorioCacheMock
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            _repositorioElasticTurmaMock
                .Setup(r => r.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    CodigoTurma = int.Parse(TURMA_ID),
                    CodigoEscola = CODIGO_ESCOLA_PERMITIDA
                });

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.True(result);

            _repositorioCacheMock.Verify(
                r => r.SalvarRedisAsync(It.IsAny<string>(), It.IsAny<TurmaElasticDto>(), 30),
                Times.Once);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_AcessoIrrestrito_TurmaNaoExiste_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_IRRESTRITO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_IRRESTRITO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilIrrestrito());

            _repositorioCacheMock
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            _repositorioElasticTurmaMock
                .Setup(r => r.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);

            _repositorioCacheMock.Verify(
                r => r.SalvarRedisAsync(It.IsAny<string>(), It.IsAny<TurmaElasticDto>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_AcessoIrrestrito_TurmaIdNaoNumerico_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_IRRESTRITO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_IRRESTRITO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilIrrestrito());

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync("TURMA-ABC");

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_AcessoIrrestrito_TurmaNoCache_NaoDeveConsultarElastic()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_IRRESTRITO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_IRRESTRITO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilIrrestrito());

            _repositorioCacheMock
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(It.IsAny<string>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    CodigoTurma = int.Parse(TURMA_ID),
                    CodigoEscola = CODIGO_ESCOLA_PERMITIDA
                });

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.True(result);

            _repositorioElasticTurmaMock.Verify(
                r => r.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        #endregion

        #region Testes Perfil com TipoValidacao = "Regencia"

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Regencia_ComTurmaPermitida_DeveRetornarTrue()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_PROFESSOR_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_PROFESSOR_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilRegencia());

            var cacheJson = JsonConvert.SerializeObject(new[]
            {
                new { regencia = true, turmaCodigo = TURMA_ID_STRING }
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING);

            Assert.True(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Regencia_SemFlagRegencia_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_PROFESSOR_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_PROFESSOR_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilRegencia());

            var cacheJson = JsonConvert.SerializeObject(new[]
            {
                new { regencia = false, turmaCodigo = TURMA_ID_STRING }
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Regencia_TurmaDiferente_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_PROFESSOR_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_PROFESSOR_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilRegencia());

            var cacheJson = JsonConvert.SerializeObject(new[]
            {
                new { regencia = true, turmaCodigo = "OUTRA-TURMA" }
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Regencia_AcessosVazios_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_PROFESSOR_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_PROFESSOR_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilRegencia());

            var cacheJson = JsonConvert.SerializeObject(Array.Empty<object>());

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Regencia_MultiplaTurmas_ComTurmaCorreta_DeveRetornarTrue()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_PROFESSOR_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_PROFESSOR_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilRegencia());

            var cacheJson = JsonConvert.SerializeObject(new[]
            {
                new { regencia = true, turmaCodigo = "TURMA-1" },
                new { regencia = true, turmaCodigo = TURMA_ID_STRING },
                new { regencia = true, turmaCodigo = "TURMA-3" }
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING);

            Assert.True(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_Regencia_UsuarioSemRfNoClaim_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, perfil: PERFIL_PROFESSOR_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_PROFESSOR_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilRegencia());

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID_STRING);

            Assert.False(result);
        }

        #endregion

        #region Testes Perfil com TipoValidacao = "UE"

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_UE_UePermitida_DeveRetornarTrue()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_GESTAO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_GESTAO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilUE());

            var cacheJson = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            _repositorioCacheMock
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            _repositorioElasticTurmaMock
                .Setup(r => r.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    CodigoTurma = int.Parse(TURMA_ID),
                    CodigoEscola = CODIGO_ESCOLA_PERMITIDA
                });

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.True(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_UE_UeNaoPermitida_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_GESTAO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_GESTAO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilUE());

            var cacheJson = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            _repositorioCacheMock
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            _repositorioElasticTurmaMock
                .Setup(r => r.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    CodigoTurma = int.Parse(TURMA_ID),
                    CodigoEscola = CODIGO_ESCOLA_NAO_PERMITIDA
                });

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_UE_TurmaNaoExisteNoElastic_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_GESTAO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_GESTAO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilUE());

            var cacheJson = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            _repositorioCacheMock
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            _repositorioElasticTurmaMock
                .Setup(r => r.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_UE_TurmaIdNaoNumerico_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_GESTAO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_GESTAO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilUE());

            var cacheJson = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync("TURMA-ABC");

            Assert.False(result);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_UE_TurmaNoCache_NaoDeveConsultarElastic()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_GESTAO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_GESTAO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilUE());

            var cacheJson = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            _repositorioCacheMock
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(It.IsAny<string>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    CodigoTurma = int.Parse(TURMA_ID),
                    CodigoEscola = CODIGO_ESCOLA_PERMITIDA
                });

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.True(result);

            _repositorioElasticTurmaMock.Verify(
                r => r.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_UE_AcessosVazios_DeveRetornarFalse()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_GESTAO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_GESTAO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilUE());

            // Cache retorna json com lista de UEs vazia
            var cacheJson = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = Array.Empty<string>()
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        #endregion

        #region Testes de TipoValidacao desconhecido

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_TipoValidacaoDesconhecido_DeveRetornarFalse()
        {
            var perfilId = Guid.NewGuid();
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: perfilId.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(perfilId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PerfilInfoSondagemDto
                {
                    Codigo = perfilId,
                    Nome = "Tipo Desconhecido",
                    PermiteConsultar = true,
                    AcessoIrrestrito = false,
                    ConsultarAbrangencia = true,
                    TipoValidacao = "TipoInexistente"
                });

            var cacheJson = JsonConvert.SerializeObject(new
            {
                login = RF_USUARIO,
                idUes = UES_PERMITIDAS
            });

            _repositorioCacheMock
                .Setup(r => r.ObterRedisToJsonAsync(It.IsAny<string>()))
                .ReturnsAsync(cacheJson);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.False(result);
        }

        #endregion

        #region Testes de Cache da Turma

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_PrimeiraConsultaTurma_DeveBuscarElasticESalvarCache()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_IRRESTRITO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_IRRESTRITO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilIrrestrito());

            var turmaElastic = new TurmaElasticDto
            {
                CodigoTurma = int.Parse(TURMA_ID),
                CodigoEscola = CODIGO_ESCOLA_PERMITIDA
            };

            _repositorioCacheMock
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(It.IsAny<string>()))
                .ReturnsAsync((TurmaElasticDto)null!);

            _repositorioElasticTurmaMock
                .Setup(r => r.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmaElastic);

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.True(result);

            _repositorioElasticTurmaMock.Verify(
                r => r.ObterTurmaPorId(
                    It.Is<FiltroQuestionario>(f => f.TurmaId == int.Parse(TURMA_ID)),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _repositorioCacheMock.Verify(
                r => r.SalvarRedisAsync($"turma-elastic:{TURMA_ID}", turmaElastic, 30),
                Times.Once);
        }

        [Fact]
        public async Task ValidarPermissaoAcessoAsync_SegundaConsultaTurma_DeveUsarApenasCache()
        {
            var accessor = CriarHttpContextAccessor(true, rf: RF_USUARIO, perfil: PERFIL_IRRESTRITO_ID.ToString());

            _perfilServiceMock
                .Setup(p => p.ObterPerfilPorIdAsync(PERFIL_IRRESTRITO_ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CriarPerfilIrrestrito());

            _repositorioCacheMock
                .Setup(r => r.ObterRedisAsync<TurmaElasticDto>(It.IsAny<string>()))
                .ReturnsAsync(new TurmaElasticDto
                {
                    CodigoTurma = int.Parse(TURMA_ID),
                    CodigoEscola = CODIGO_ESCOLA_PERMITIDA
                });

            var service = CriarService(accessor);

            var result = await service.ValidarPermissaoAcessoAsync(TURMA_ID);

            Assert.True(result);

            _repositorioElasticTurmaMock.Verify(
                r => r.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _repositorioCacheMock.Verify(
                r => r.SalvarRedisAsync(It.IsAny<string>(), It.IsAny<TurmaElasticDto>(), It.IsAny<int>()),
                Times.Never);
        }

        #endregion
    }
}
