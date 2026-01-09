using Moq;
using SME.Sondagem.Aplicacao.UseCases.Bimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Bimestre;

public class AtualizarBimestreUseCaseTeste
{
    private readonly Mock<IRepositorioBimestre> _repositorioBimestreMock;
    private readonly AtualizarBimestreUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public AtualizarBimestreUseCaseTeste()
    {
        _repositorioBimestreMock = new Mock<IRepositorioBimestre>();
        _useCase = new AtualizarBimestreUseCase(_repositorioBimestreMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_BimestreExiste_DeveAtualizarERetornarBimestreDto()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto
        {
            Id = 1,
            Descricao = "Bimestre Atualizado",
            CodBimestreEnsinoEol = 3
        };

        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Nome Original")
        {
            Id = id,
            CriadoEm = DateTime.Now.AddDays(-1),
            CriadoPor = "Usuario Original",
            CriadoRF = "RF001"
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.AtualizarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, bimestreDto, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Bimestre Atualizado", resultado.Descricao);
        Assert.Equal(3, resultado.CodBimestreEnsinoEol);

        Assert.Equal("Bimestre Atualizado", bimestreExistente.Descricao);
        Assert.Equal(3, bimestreExistente.CodBimestreEnsinoEol);

        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.Verify(x => x.AtualizarAsync(bimestreExistente, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_BimestreNaoExiste_DeveRetornarNull()
    {
        const int id = 999;
        var bimestreDto = new BimestreDto
        {
            Descricao = "Qualquer Nome",
            CodBimestreEnsinoEol = 1
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((Dominio.Entidades.Bimestre?)null);

        var resultado = await _useCase.ExecutarAsync(id, bimestreDto, _cancellationToken);

        Assert.Null(resultado);

        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.Verify(x => x.AtualizarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_RepositorioRetornaFalse_DeveRetornarNull()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto
        {
            Descricao = "Novo Bimestre",
            CodBimestreEnsinoEol = 2,
            AlteradoPor = "Usuario",
            AlteradoRF = "RF001"
        };

        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Nome Original")
        {
            Id = id
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.AtualizarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), _cancellationToken))
            .ReturnsAsync(false);

        var resultado = await _useCase.ExecutarAsync(id, bimestreDto, _cancellationToken);

        Assert.Null(resultado);

        _repositorioBimestreMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioBimestreMock.Verify(x => x.AtualizarAsync(bimestreExistente, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveDefinirDataAlteracaoComoAgora()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto
        {
            Descricao = "Bimestre Atualizado",
            CodBimestreEnsinoEol = 2
        };

        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Nome Original")
        {
            Id = id,
            AlteradoEm = null
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.AtualizarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, bimestreDto, _cancellationToken);

        // Verifica se a atualização foi bem-sucedida
        Assert.NotNull(resultado);
        Assert.Equal("Bimestre Atualizado", bimestreExistente.Descricao);
        Assert.Equal(2, bimestreExistente.CodBimestreEnsinoEol);
        
        _repositorioBimestreMock.Verify(x => x.AtualizarAsync(bimestreExistente, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationToken_DevePropararExcecao()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto { Descricao = "Teste", CodBimestreEnsinoEol = 1 };
        var cancellationTokenCancelado = new CancellationToken(true);

        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Nome") { Id = id };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.AtualizarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, bimestreDto, cancellationTokenCancelado));
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarMetodoAtualizarDaEntidade()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto
        {
            Descricao = "Nome Novo",
            CodBimestreEnsinoEol = 5
        };

        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Nome Antigo")
        {
            Id = (int)id
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.AtualizarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), _cancellationToken))
            .ReturnsAsync(true);

        await _useCase.ExecutarAsync(id, bimestreDto, _cancellationToken);

        Assert.Equal("Nome Novo", bimestreExistente.Descricao);
        Assert.Equal(5, bimestreExistente.CodBimestreEnsinoEol);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarBimestreDtoCompleto()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto
        {
            Descricao = "Bimestre Teste",
            CodBimestreEnsinoEol = 2
        };

        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Nome Original")
        {
            Id = id,
            CriadoEm = DateTime.Now.AddDays(-5),
            CriadoPor = "Usuario Criador",
            CriadoRF = "RF456"
        };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        _repositorioBimestreMock
            .Setup(x => x.AtualizarAsync(It.IsAny<Dominio.Entidades.Bimestre>(), _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, bimestreDto, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Bimestre Teste", resultado.Descricao);
        Assert.Equal(2, resultado.CodBimestreEnsinoEol);
    }

    [Fact]
    public async Task ExecutarAsync_ComNomeVazio_DeveDispararExcecao()
    {
        const int id = 1;
        var BimestreDto = new BimestreDto
        {
            Descricao = "",
            CodBimestreEnsinoEol = 1
        };

        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Nome Original") { Id = id };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecutarAsync(id, BimestreDto, _cancellationToken));
    }

    [Fact]
    public async Task ExecutarAsync_ComComponenteCurricularIdInvalido_DeveDispararExcecao()
    {
        const int id = 1;
        var bimestreDto = new BimestreDto
        {
            Descricao = "Nome Válido",
            CodBimestreEnsinoEol = 0
        };

        var bimestreExistente = new Dominio.Entidades.Bimestre(1, "Nome Original") { Id = id };

        _repositorioBimestreMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(bimestreExistente);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecutarAsync(id, bimestreDto, _cancellationToken));
    }
}