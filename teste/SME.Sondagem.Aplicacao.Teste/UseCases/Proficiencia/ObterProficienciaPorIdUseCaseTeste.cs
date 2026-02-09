using Moq;
using SME.Sondagem.Aplicacao.UseCases.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Proficiencia;

public class ObterProficienciaPorIdUseCaseTeste
{
    private readonly Mock<IRepositorioProficiencia> _repositorioProficienciaMock;
    private readonly ObterProficienciaPorIdUseCase _useCase;
    private readonly CancellationToken _cancellationToken;
    private const int ModalidadeId = (int)Modalidade.Fundamental;

    public ObterProficienciaPorIdUseCaseTeste()
    {
        _repositorioProficienciaMock = new Mock<IRepositorioProficiencia>();
        _useCase = new ObterProficienciaPorIdUseCase(_repositorioProficienciaMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_ProficienciaExiste_DeveRetornarProficienciaDto()
    {
        const int id = 1;
        var proficiencia = new SME.Sondagem.Dominio.Entidades.Proficiencia("Proficiência Teste", 2,ModalidadeId)
        {
            Id = (int)id,
            CriadoEm = DateTime.Now,
            CriadoPor = "Usuario1",
            CriadoRF = "RF001",
            AlteradoEm = DateTime.Now.AddHours(1),
            AlteradoPor = "Usuario2",
            AlteradoRF = "RF002"
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficiencia);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Proficiência Teste", resultado.Nome);
        Assert.Equal(2, resultado.ComponenteCurricularId);
        Assert.Equal("Usuario1", resultado.CriadoPor);
        Assert.Equal("RF001", resultado.CriadoRF);
        Assert.Equal(proficiencia.CriadoEm, resultado.CriadoEm);
        Assert.Equal(proficiencia.AlteradoEm, resultado.AlteradoEm);
        Assert.Equal("Usuario2", resultado.AlteradoPor);
        Assert.Equal("RF002", resultado.AlteradoRF);

        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ProficienciaNaoExiste_DeveRetornarNull()
    {
        const long id = 999;

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Proficiencia?)null);

        var resultado = await _useCase.ExecutarAsync(id, _cancellationToken);

        Assert.Null(resultado);

        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComParametrosCorretos()
    {
        const long id = 42;
        var cancellationTokenCustom = new CancellationToken(false);

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCustom))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Proficiencia?)null);

        await _useCase.ExecutarAsync(id, cancellationTokenCustom);

        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        const long id = 1;
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cancellationTokenCancelado));

        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, cancellationTokenCancelado), Times.Once);
    }
}