using Moq;
using SME.Sondagem.Aplicacao.UseCases.Ciclo;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Ciclo;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Ciclo;

public class AtualizarCicloUseCaseTeste
{
    private readonly Mock<IRepositorioCiclo> _repositorioCicloMock;
    private readonly AtualizarCicloUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public AtualizarCicloUseCaseTeste()
    {
        _repositorioCicloMock = new Mock<IRepositorioCiclo>();
        _useCase = new AtualizarCicloUseCase(_repositorioCicloMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_CicloExiste_DeveAtualizarERetornarCicloDto()
    {
        const int id = 1;
        var cicloDto = new CicloDto
        {
            Id = 1,
            DescCiclo = "Ciclo Atualizado",
            CodCicloEnsinoEol = 3,
            AlteradoPor = "Usuario Alterador",
            AlteradoRF = "RF999"
        };

        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Nome Original")
        {
            Id = id,
            CriadoEm = DateTime.Now.AddDays(-1),
            CriadoPor = "Usuario Original",
            CriadoRF = "RF001"
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, cicloDto, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Ciclo Atualizado", resultado.DescCiclo);
        Assert.Equal(3, resultado.CodCicloEnsinoEol);
        Assert.Equal("Usuario Alterador", resultado.AlteradoPor);
        Assert.Equal("RF999", resultado.AlteradoRF);
        Assert.NotNull(resultado.AlteradoEm);

        Assert.Equal("Ciclo Atualizado", cicloExistente.DescCiclo);
        Assert.Equal(3, cicloExistente.CodCicloEnsinoEol);
        Assert.Equal("Usuario Alterador", cicloExistente.AlteradoPor);
        Assert.Equal("RF999", cicloExistente.AlteradoRF);
        Assert.NotNull(cicloExistente.AlteradoEm);

        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.Verify(x => x.AtualizarAsync(cicloExistente, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_CicloNaoExiste_DeveRetornarNull()
    {
        const int id = 999;
        var cicloDto = new CicloDto
        {
            DescCiclo = "Qualquer Nome",
            CodCicloEnsinoEol = 1
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Ciclo?)null);

        var resultado = await _useCase.ExecutarAsync(id, cicloDto, _cancellationToken);

        Assert.Null(resultado);

        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.Verify(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_RepositorioRetornaFalse_DeveRetornarNull()
    {
        const int id = 1;
        var cicloDto = new CicloDto
        {
            DescCiclo = "Novo Ciclo",
            CodCicloEnsinoEol = 2,
            AlteradoPor = "Usuario",
            AlteradoRF = "RF001"
        };

        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Nome Original")
        {
            Id = id
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), _cancellationToken))
            .ReturnsAsync(false);

        var resultado = await _useCase.ExecutarAsync(id, cicloDto, _cancellationToken);

        Assert.Null(resultado);

        _repositorioCicloMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioCicloMock.Verify(x => x.AtualizarAsync(cicloExistente, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveDefinirDataAlteracaoComoAgora()
    {
        const int id = 1;
        var cicloDto = new CicloDto
        {
            DescCiclo = "Ciclo Atualizado",
            CodCicloEnsinoEol = 2,
            AlteradoPor = "Usuario",
            AlteradoRF = "RF001"
        };

        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Nome Original")
        {
            Id = id,
            AlteradoEm = null
        };

        var dataAntesExecucao = DateTime.Now;

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), _cancellationToken))
            .ReturnsAsync(true);

        await _useCase.ExecutarAsync(id, cicloDto, _cancellationToken);

        var dataAposExecucao = DateTime.Now;

        Assert.NotNull(cicloExistente.AlteradoEm);
        Assert.True(cicloExistente.AlteradoEm >= dataAntesExecucao);
        Assert.True(cicloExistente.AlteradoEm <= dataAposExecucao);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationToken_DevePropararExcecao()
    {
        const int id = 1;
        var cicloDto = new CicloDto { DescCiclo = "Teste", CodCicloEnsinoEol = 1 };
        var cancellationTokenCancelado = new CancellationToken(true);

        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Nome") { Id = id };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(id, cicloDto, cancellationTokenCancelado));
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarMetodoAtualizarDaEntidade()
    {
        const int id = 1;
        var cicloDto = new CicloDto
        {
            DescCiclo = "Nome Novo",
            CodCicloEnsinoEol = 5
        };

        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Nome Antigo")
        {
            Id = (int)id
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), _cancellationToken))
            .ReturnsAsync(true);

        await _useCase.ExecutarAsync(id, cicloDto, _cancellationToken);

        Assert.Equal("Nome Novo", cicloExistente.DescCiclo);
        Assert.Equal(5, cicloExistente.CodCicloEnsinoEol);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarCicloDtoCompleto()
    {
        const int id = 1;
        var cicloDto = new CicloDto
        {
            DescCiclo = "Ciclo Teste",
            CodCicloEnsinoEol = 2,
            AlteradoPor = "Usuario Teste",
            AlteradoRF = "RF123"
        };

        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Nome Original")
        {
            Id = id,
            CriadoEm = DateTime.Now.AddDays(-5),
            CriadoPor = "Usuario Criador",
            CriadoRF = "RF456"
        };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        _repositorioCicloMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Ciclo>(), _cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, cicloDto, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Ciclo Teste", resultado.DescCiclo);
        Assert.Equal(2, resultado.CodCicloEnsinoEol);
        Assert.Equal("Usuario Teste", resultado.AlteradoPor);
        Assert.Equal("RF123", resultado.AlteradoRF);
        Assert.NotNull(resultado.AlteradoEm);

        Assert.Equal(cicloExistente.CriadoEm, resultado.CriadoEm);
        Assert.Equal("Usuario Criador", resultado.CriadoPor);
        Assert.Equal("RF456", resultado.CriadoRF);
    }

    [Fact]
    public async Task ExecutarAsync_ComNomeVazio_DeveDispararExcecao()
    {
        const int id = 1;
        var cicloDto = new CicloDto
        {
            DescCiclo = "",
            CodCicloEnsinoEol = 1
        };

        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Nome Original") { Id = id };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecutarAsync(id, cicloDto, _cancellationToken));
    }

    [Fact]
    public async Task ExecutarAsync_ComComponenteCurricularIdInvalido_DeveDispararExcecao()
    {
        const int id = 1;
        var cicloDto = new CicloDto
        {
            DescCiclo = "Nome Válido",
            CodCicloEnsinoEol = 0
        };

        var cicloExistente = new SME.Sondagem.Dominio.Entidades.Ciclo(1, "Nome Original") { Id = id };

        _repositorioCicloMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(cicloExistente);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecutarAsync(id, cicloDto, _cancellationToken));
    }
}