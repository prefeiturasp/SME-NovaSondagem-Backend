using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Services.SGP;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos;
using System.Net;
using System.Security.Claims;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.Services;

public class AbrangenciaServiceTeste
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IRepositorioCache> _mockCache;
    private readonly Mock<IPerfilService> _mockPerfilService;
    private readonly AbrangenciaService _service;

    public AbrangenciaServiceTeste()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockCache = new Mock<IRepositorioCache>();
        _mockPerfilService = new Mock<IPerfilService>();

        _service = new AbrangenciaService(
            _mockHttpClientFactory.Object,
            _mockHttpContextAccessor.Object,
            _mockCache.Object,
            _mockPerfilService.Object);
    }

    private void ConfigurarHttpContextComClaims(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);
        var mockContext = new Mock<HttpContext>();
        mockContext.Setup(c => c.User).Returns(principal);
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);
    }

    #region Construtor

    [Fact]
    public void Construtor_NullHttpClientFactory_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AbrangenciaService(null!, _mockHttpContextAccessor.Object, _mockCache.Object, _mockPerfilService.Object));
    }

    [Fact]
    public void Construtor_NullHttpContextAccessor_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AbrangenciaService(_mockHttpClientFactory.Object, null!, _mockCache.Object, _mockPerfilService.Object));
    }

    [Fact]
    public void Construtor_NullRepositorioCache_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AbrangenciaService(_mockHttpClientFactory.Object, _mockHttpContextAccessor.Object, null!, _mockPerfilService.Object));
    }

    [Fact]
    public void Construtor_NullPerfilService_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AbrangenciaService(_mockHttpClientFactory.Object, _mockHttpContextAccessor.Object, _mockCache.Object, null!));
    }

    #endregion

    #region DeveIgnorarAbrangenciaAsync

    [Fact]
    public async Task DeveIgnorarAbrangenciaAsync_SemHttpContext_RetornaFalse()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        // Act
        var resultado = await _service.DeveIgnorarAbrangenciaAsync();

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task DeveIgnorarAbrangenciaAsync_SemClaimPerfil_RetornaFalse()
    {
        // Arrange
        ConfigurarHttpContextComClaims();

        // Act
        var resultado = await _service.DeveIgnorarAbrangenciaAsync();

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task DeveIgnorarAbrangenciaAsync_PerfilGuidInvalido_RetornaFalse()
    {
        // Arrange
        ConfigurarHttpContextComClaims(new Claim("perfil", "nao-um-guid"));

        // Act
        var resultado = await _service.DeveIgnorarAbrangenciaAsync();

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task DeveIgnorarAbrangenciaAsync_PerfilNaoEncontrado_RetornaFalse()
    {
        // Arrange
        ConfigurarHttpContextComClaims(new Claim("perfil", Guid.NewGuid().ToString()));
        _mockPerfilService
            .Setup(p => p.ObterPerfilPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PerfilInfoSondagemDto)null!);

        // Act
        var resultado = await _service.DeveIgnorarAbrangenciaAsync();

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public async Task DeveIgnorarAbrangenciaAsync_AcessoIrrestrito_RetornaTrue()
    {
        // Arrange
        ConfigurarHttpContextComClaims(new Claim("perfil", Guid.NewGuid().ToString()));
        _mockPerfilService
            .Setup(p => p.ObterPerfilPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PerfilInfoSondagemDto { AcessoIrrestrito = true });

        // Act
        var resultado = await _service.DeveIgnorarAbrangenciaAsync();

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public async Task DeveIgnorarAbrangenciaAsync_TipoValidacaoAcessoTotal_RetornaTrue()
    {
        // Arrange
        ConfigurarHttpContextComClaims(new Claim("perfil", Guid.NewGuid().ToString()));
        _mockPerfilService
            .Setup(p => p.ObterPerfilPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PerfilInfoSondagemDto { TipoValidacao = "AcessoTotal" });

        // Act
        var resultado = await _service.DeveIgnorarAbrangenciaAsync();

        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public async Task DeveIgnorarAbrangenciaAsync_SemAcessoEspecial_RetornaFalse()
    {
        // Arrange
        ConfigurarHttpContextComClaims(new Claim("perfil", Guid.NewGuid().ToString()));
        _mockPerfilService
            .Setup(p => p.ObterPerfilPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PerfilInfoSondagemDto { AcessoIrrestrito = false, TipoValidacao = "Normal" });

        // Act
        var resultado = await _service.DeveIgnorarAbrangenciaAsync();

        // Assert
        Assert.False(resultado);
    }

    #endregion

    #region ObterAbrangenciaCompletaAsync

    [Fact]
    public async Task ObterAbrangenciaCompletaAsync_CacheHit_RetornaDadosDoCache()
    {
        // Arrange
        var json = JsonConvert.SerializeObject(new
        {
            dres = new[] { new { codigo = "DRE-1" } },
            ues = new[] { new { codigo = "UE-1" } },
            turmas = new[] { new { codigo = "TURMA-1" } }
        });

        ConfigurarHttpContextComClaims(new Claim("rf", "12345"), new Claim("perfil", Guid.NewGuid().ToString()));
        _mockCache.Setup(c => c.ObterRedisToJsonAsync(It.IsAny<string>())).ReturnsAsync(json);

        // Act
        var (dres, ues, turmas) = await _service.ObterAbrangenciaCompletaAsync(new AbrangenciaFiltroQuery(2025, 5, null));

        // Assert
        Assert.Contains("DRE-1", dres);
        Assert.Contains("UE-1", ues);
        Assert.Contains("TURMA-1", turmas);
    }

    [Fact]
    public async Task ObterAbrangenciaCompletaAsync_CacheHit_NaoFazChamadaHttp()
    {
        // Arrange
        var json = JsonConvert.SerializeObject(new
        {
            dres = Array.Empty<object>(),
            ues = Array.Empty<object>(),
            turmas = Array.Empty<object>()
        });

        ConfigurarHttpContextComClaims(new Claim("rf", "12345"), new Claim("perfil", Guid.NewGuid().ToString()));
        _mockCache.Setup(c => c.ObterRedisToJsonAsync(It.IsAny<string>())).ReturnsAsync(json);

        // Act
        await _service.ObterAbrangenciaCompletaAsync(new AbrangenciaFiltroQuery(2025, 5, null));

        // Assert
        _mockHttpClientFactory.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ObterAbrangenciaCompletaAsync_CacheMiss_HttpFalha_LancaExcecao()
    {
        // Arrange
        ConfigurarHttpContextComClaims(new Claim("rf", "12345"), new Claim("perfil", Guid.NewGuid().ToString()));
        _mockCache.Setup(c => c.ObterRedisToJsonAsync(It.IsAny<string>())).ReturnsAsync(string.Empty);

        var httpClient = HttpClientMockHelper.Create(HttpStatusCode.InternalServerError);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ObterAbrangenciaCompletaAsync(new AbrangenciaFiltroQuery(2025, 5, null)));

        Assert.Equal("Falha ao obter abrangência do SGP. Tente novamente.", ex.Message);
    }

    [Fact]
    public async Task ObterAbrangenciaCompletaAsync_CacheMiss_HttpNoContent_RetornaVazios()
    {
        // Arrange
        ConfigurarHttpContextComClaims(new Claim("rf", "12345"), new Claim("perfil", Guid.NewGuid().ToString()));
        _mockCache.Setup(c => c.ObterRedisToJsonAsync(It.IsAny<string>())).ReturnsAsync(string.Empty);

        var httpClient = HttpClientMockHelper.Create(HttpStatusCode.NoContent);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var (dres, ues, turmas) = await _service.ObterAbrangenciaCompletaAsync(new AbrangenciaFiltroQuery(2025, 5, null));

        // Assert
        Assert.Empty(dres);
        Assert.Empty(ues);
        Assert.Empty(turmas);
    }

    [Fact]
    public async Task ObterAbrangenciaCompletaAsync_CacheMiss_HttpSucesso_RetornaDados()
    {
        // Arrange
        var json = JsonConvert.SerializeObject(new
        {
            dres = new[] { new { codigo = "DRE-1" }, new { codigo = "DRE-2" } },
            ues = new[] { new { codigo = "UE-1" } },
            turmas = Array.Empty<object>()
        });

        ConfigurarHttpContextComClaims(new Claim("rf", "12345"), new Claim("perfil", Guid.NewGuid().ToString()));
        _mockCache.Setup(c => c.ObterRedisToJsonAsync(It.IsAny<string>())).ReturnsAsync(string.Empty);
        _mockCache.Setup(c => c.SalvarRedisToJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.CompletedTask);

        var httpClient = HttpClientMockHelper.Create(HttpStatusCode.OK, json);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var (dres, ues, turmas) = await _service.ObterAbrangenciaCompletaAsync(new AbrangenciaFiltroQuery(2025, 5, null));

        // Assert
        Assert.Equal(2, dres.Count);
        Assert.Single(ues);
        Assert.Empty(turmas);
    }

    [Fact]
    public async Task ObterAbrangenciaCompletaAsync_CacheMiss_HttpSucesso_SalvaNoCache()
    {
        // Arrange
        var json = JsonConvert.SerializeObject(new
        {
            dres = Array.Empty<object>(),
            ues = Array.Empty<object>(),
            turmas = Array.Empty<object>()
        });

        ConfigurarHttpContextComClaims(new Claim("rf", "12345"), new Claim("perfil", Guid.NewGuid().ToString()));
        _mockCache.Setup(c => c.ObterRedisToJsonAsync(It.IsAny<string>())).ReturnsAsync(string.Empty);
        _mockCache.Setup(c => c.SalvarRedisToJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.CompletedTask);

        var httpClient = HttpClientMockHelper.Create(HttpStatusCode.OK, json);
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        await _service.ObterAbrangenciaCompletaAsync(new AbrangenciaFiltroQuery(2025, 5, null));

        // Assert
        _mockCache.Verify(c => c.SalvarRedisToJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
    }

    #endregion
}
