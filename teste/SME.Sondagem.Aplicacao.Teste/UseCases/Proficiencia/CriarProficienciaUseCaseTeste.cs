using Moq;
using SME.Sondagem.Aplicacao.UseCases.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Proficiencia;

public class CriarProficienciaUseCaseTeste
{
    private readonly Mock<IRepositorioProficiencia> _repositorioProficienciaMock;
    private readonly CriarProficienciaUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public CriarProficienciaUseCaseTeste()
    {
        _repositorioProficienciaMock = new Mock<IRepositorioProficiencia>();
        _useCase = new CriarProficienciaUseCase(_repositorioProficienciaMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosValidos_DeveCriarProficienciaERetornarId()
    {
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Nova Proficiência",
            ComponenteCurricularId = 1,
            CriadoPor = "Usuario1",
            CriadoRF = "RF001"
        };

        const long expectedId = 123;

        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), _cancellationToken))
            .ReturnsAsync(expectedId);

        var resultado = await _useCase.ExecutarAsync(proficienciaDto, _cancellationToken);

        Assert.Equal(expectedId, resultado);

        _repositorioProficienciaMock.Verify(x => x.SalvarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Proficiencia>(p => 
                p.Nome == proficienciaDto.Nome && 
                p.ComponenteCurricularId == proficienciaDto.ComponenteCurricularId),
            _cancellationToken), Times.Once);
    }    

    [Fact]
    public async Task ExecutarAsync_ComCancellationToken_DevePropararExcecao()
    {
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Teste",
            ComponenteCurricularId = 1
        };

        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(proficienciaDto, cancellationTokenCancelado));
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Proficiência Teste",
            ComponenteCurricularId = 1
        };

        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(proficienciaDto, _cancellationToken));

        Assert.Equal("Erro do repositório", exception.Message);
    }

    [Fact]
    public async Task ExecutarAsync_DeveCriarEntidadeComParametrosCorretos()
    {
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Proficiência Específica",
            ComponenteCurricularId = 5,
            CriadoPor = "Testador",
            CriadoRF = "RF999"
        };

        SME.Sondagem.Dominio.Entidades.Proficiencia? proficienciaCapturada = null;
        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), _cancellationToken))
            .Callback<SME.Sondagem.Dominio.Entidades.Proficiencia, CancellationToken>((p, ct) => proficienciaCapturada = p)
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(proficienciaDto, _cancellationToken);

        Assert.NotNull(proficienciaCapturada);
        Assert.Equal("Proficiência Específica", proficienciaCapturada.Nome);
        Assert.Equal(5, proficienciaCapturada.ComponenteCurricularId);
    }

    [Fact]
    public async Task ExecutarAsync_ComDiferentesCancellationTokens_DevePropararParaRepositorio()
    {
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Teste",
            ComponenteCurricularId = 1
        };

        var customCancellationToken = new CancellationTokenSource().Token;

        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), customCancellationToken))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(proficienciaDto, customCancellationToken);

        _repositorioProficienciaMock.Verify(x => x.SalvarAsync(
            It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), 
            customCancellationToken), Times.Once);
    }
}