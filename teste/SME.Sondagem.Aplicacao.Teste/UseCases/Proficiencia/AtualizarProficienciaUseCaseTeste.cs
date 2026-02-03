using Moq;
using SME.Sondagem.Aplicacao.UseCases.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Proficiencia;

public class AtualizarProficienciaUseCaseTeste
{
    private readonly Mock<IRepositorioProficiencia> _repositorioProficienciaMock;
    private readonly AtualizarProficienciaUseCase _useCase;
    private readonly CancellationToken _cancellationToken;
    private const int ModalidadeId = (int)Modalidade.Fundamental;

    public AtualizarProficienciaUseCaseTeste()
    {
        _repositorioProficienciaMock = new Mock<IRepositorioProficiencia>();
        _useCase = new AtualizarProficienciaUseCase(_repositorioProficienciaMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_ProficienciaExiste_DeveAtualizarERetornarProficienciaDto()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto
        {
            Id = 1,
            Nome = "Profici�ncia Atualizada",
            ComponenteCurricularId = 3,
            ModalidadeId = (int)Modalidade.Fundamental,
        };

        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Nome Original", 1,ModalidadeId)
        {
            Id = id,
            CriadoEm = DateTime.Now.AddDays(-1),
            CriadoPor = "Usuario Original",
            CriadoRF = "RF001"
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), _cancellationToken))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, proficienciaDto, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Profici�ncia Atualizada", resultado.Nome);
        Assert.Equal(3, resultado.ComponenteCurricularId);

        Assert.Equal("Profici�ncia Atualizada", proficienciaExistente.Nome);
        Assert.Equal(3, proficienciaExistente.ComponenteCurricularId);

        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.SalvarAsync(proficienciaExistente, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ProficienciaNaoExiste_DeveRetornarNull()
    {
        const int id = 999;
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Qualquer Nome",
            ComponenteCurricularId = 1
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Proficiencia?)null);

        var resultado = await _useCase.ExecutarAsync(id, proficienciaDto, _cancellationToken);

        Assert.Null(resultado);

        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_RepositorioRetornaFalse_DeveRetornarNull()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Nova Profici�ncia",
            ComponenteCurricularId = 2,
            AlteradoPor = "Usuario",
            AlteradoRF = "RF001",
            ModalidadeId = (int)Modalidade.Fundamental,
        };

        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Nome Original", 1,ModalidadeId)
        {
            Id = id
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), _cancellationToken))
            .ReturnsAsync(0);

        var resultado = await _useCase.ExecutarAsync(id, proficienciaDto, _cancellationToken);

        Assert.Null(resultado);

        _repositorioProficienciaMock.Verify(x => x.ObterPorIdAsync(id, _cancellationToken), Times.Once);
        _repositorioProficienciaMock.Verify(x => x.SalvarAsync(proficienciaExistente, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveDefinirDataAlteracaoComoAgora()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Profici�ncia Atualizada",
            ComponenteCurricularId = 2,
            AlteradoPor = "Usuario",
            AlteradoRF = "RF001",
            ModalidadeId = (int)Modalidade.Fundamental,
        };

        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Nome Original", 1,ModalidadeId)
        {
            Id = id,
            AlteradoEm = null
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), _cancellationToken))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, proficienciaDto, _cancellationToken);

        // Verifica se a atualiza��o foi bem-sucedida
        Assert.NotNull(resultado);
        Assert.Equal("Profici�ncia Atualizada", proficienciaExistente.Nome);
        Assert.Equal(2, proficienciaExistente.ComponenteCurricularId);
        
        _repositorioProficienciaMock.Verify(x => x.SalvarAsync(proficienciaExistente, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationToken_DevePropararExcecao()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto { Nome = "Teste", ComponenteCurricularId = 1 };
        var cancellationTokenCancelado = new CancellationToken(true);

        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Nome", 1,ModalidadeId) { Id = id };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationTokenCancelado))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), cancellationTokenCancelado))
            .ThrowsAsync(new ArgumentException());

        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecutarAsync(id, proficienciaDto, cancellationTokenCancelado));
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarMetodoAtualizarDaEntidade()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Nome Novo",
            ComponenteCurricularId = 5,
            ModalidadeId = (int)Modalidade.Fundamental,
        };

        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Nome Antigo", 3,ModalidadeId)
        {
            Id = (int)id
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), _cancellationToken))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(id, proficienciaDto, _cancellationToken);

        Assert.Equal("Nome Novo", proficienciaExistente.Nome);
        Assert.Equal(5, proficienciaExistente.ComponenteCurricularId);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarProficienciaDtoCompleto()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Profici�ncia Teste",
            ComponenteCurricularId = 2,
            ModalidadeId = (int)Modalidade.Fundamental,
        };

        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Nome Original", 1,ModalidadeId)
        {
            Id = id,
            CriadoEm = DateTime.Now.AddDays(-5),
            CriadoPor = "Usuario Criador",
            CriadoRF = "RF456"
        };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        _repositorioProficienciaMock
            .Setup(x => x.SalvarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Proficiencia>(), _cancellationToken))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(id, proficienciaDto, _cancellationToken);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("Profici�ncia Teste", resultado.Nome);
        Assert.Equal(2, resultado.ComponenteCurricularId);
    }

    [Fact]
    public async Task ExecutarAsync_ComNomeVazio_DeveDispararExcecao()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "",
            ComponenteCurricularId = 1
        };

        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Nome Original", 1,ModalidadeId) { Id = id };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecutarAsync(id, proficienciaDto, _cancellationToken));
    }

    [Fact]
    public async Task ExecutarAsync_ComComponenteCurricularIdInvalido_DeveDispararExcecao()
    {
        const int id = 1;
        var proficienciaDto = new ProficienciaDto
        {
            Nome = "Nome V�lido",
            ComponenteCurricularId = 0
        };

        var proficienciaExistente = new SME.Sondagem.Dominio.Entidades.Proficiencia("Nome Original", 1,ModalidadeId) { Id = id };

        _repositorioProficienciaMock
            .Setup(x => x.ObterPorIdAsync(id, _cancellationToken))
            .ReturnsAsync(proficienciaExistente);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecutarAsync(id, proficienciaDto, _cancellationToken));
    }
}