using Moq;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Aplicacao.UseCases.Proficiencia;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Proficiencia;

public class ObterProficienciasUseCaseTeste
{
    private readonly Mock<IRepositorioProficiencia> _repositorioProficienciaMock;
    private readonly ObterProficienciasUseCase _useCase;
    private readonly CancellationToken _cancellationToken;
    private const int ModalidadeId = (int)Modalidade.Fundamental;

    public ObterProficienciasUseCaseTeste()
    {
        _repositorioProficienciaMock = new Mock<IRepositorioProficiencia>();
        _useCase = new ObterProficienciasUseCase(_repositorioProficienciaMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarListaDeProficienciasDto()
    { 
        var proficiencias = new List<SME.Sondagem.Dominio.Entidades.Proficiencia>
        {
            new("Proficiência 1", 1,ModalidadeId)
            {
                Id = 1,
                CriadoEm = DateTime.Now,
                CriadoPor = "Usuario1",
                CriadoRF = "RF001"
            },
            new("Proficiência 2", 2,ModalidadeId)
            {
                Id = 2,
                CriadoEm = DateTime.Now.AddDays(-1),
                CriadoPor = "Usuario2",
                CriadoRF = "RF002",
                AlteradoEm = DateTime.Now,
                AlteradoPor = "Usuario3",
                AlteradoRF = "RF003"
            }
        };

        _repositorioProficienciaMock
            .Setup(x => x.ListarAsync(_cancellationToken))
            .ReturnsAsync(proficiencias);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        var resultadoList = resultado.ToList();
        Assert.Equal(2, resultadoList.Count);

        var primeira = resultadoList.First(x => x.Id == 1);
        Assert.Equal("Proficiência 1", primeira.Nome);
        Assert.Equal(1, primeira.ComponenteCurricularId);
        Assert.Equal("Usuario1", primeira.CriadoPor);
        Assert.Equal("RF001", primeira.CriadoRF);
        Assert.Null(primeira.AlteradoEm);
        Assert.Null(primeira.AlteradoPor);
        Assert.Null(primeira.AlteradoRF);

        var segunda = resultadoList.First(x => x.Id == 2);
        Assert.Equal("Proficiência 2", segunda.Nome);
        Assert.Equal(2, segunda.ComponenteCurricularId);
        Assert.Equal("Usuario2", segunda.CriadoPor);
        Assert.Equal("RF002", segunda.CriadoRF);
        Assert.NotNull(segunda.AlteradoEm);
        Assert.Equal("Usuario3", segunda.AlteradoPor);
        Assert.Equal("RF003", segunda.AlteradoRF);

        _repositorioProficienciaMock.Verify(x => x.ListarAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoNaoHaProficiencias_DeveRetornarListaVazia()
    {
        var proficienciasVazias = new List<SME.Sondagem.Dominio.Entidades.Proficiencia>();

        _repositorioProficienciaMock
            .Setup(x => x.ListarAsync(_cancellationToken))
            .ReturnsAsync(proficienciasVazias);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        Assert.Empty(resultado);

        _repositorioProficienciaMock.Verify(x => x.ListarAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioProficienciaMock
            .Setup(x => x.ListarAsync(cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(cancellationTokenCancelado));

        _repositorioProficienciaMock.Verify(x => x.ListarAsync(cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComCancellationTokenCorreto()
    {
        var cancellationTokenCustom = new CancellationTokenSource().Token;
        var proficiencias = new List<SME.Sondagem.Dominio.Entidades.Proficiencia>();

        _repositorioProficienciaMock
            .Setup(x => x.ListarAsync(cancellationTokenCustom))
            .ReturnsAsync(proficiencias);

        await _useCase.ExecutarAsync(cancellationTokenCustom);

        _repositorioProficienciaMock.Verify(x => x.ListarAsync(cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        _repositorioProficienciaMock
            .Setup(x => x.ListarAsync(_cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(_cancellationToken));

        Assert.Equal("Erro do repositório", exception.Message);
        _repositorioProficienciaMock.Verify(x => x.ListarAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveMaperarTodasAsPropriedadesCorretamente()
    {
        var dataEspecifica = new DateTime(2023, 10, 15, 14, 30, 0);
        var dataAlteracao = new DateTime(2023, 10, 16, 10, 15, 0);
        
        var proficiencias = new List<SME.Sondagem.Dominio.Entidades.Proficiencia>
        {
            new("Matemática Básica", 3,ModalidadeId)
            {
                Id = 100,
                CriadoEm = dataEspecifica,
                CriadoPor = "Sistema",
                CriadoRF = "RF999",
                AlteradoEm = dataAlteracao,
                AlteradoPor = "Admin",
                AlteradoRF = "RF888"
            }
        };

        _repositorioProficienciaMock
            .Setup(x => x.ListarAsync(_cancellationToken))
            .ReturnsAsync(proficiencias);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var dto = resultado.Single();
        Assert.Equal(100, dto.Id);
        Assert.Equal("Matemática Básica", dto.Nome);
        Assert.Equal(3, dto.ComponenteCurricularId);
        Assert.Equal(dataEspecifica, dto.CriadoEm);
        Assert.Equal("Sistema", dto.CriadoPor);
        Assert.Equal("RF999", dto.CriadoRF);
        Assert.Equal(dataAlteracao, dto.AlteradoEm);
        Assert.Equal("Admin", dto.AlteradoPor);
        Assert.Equal("RF888", dto.AlteradoRF);
    }

    [Fact]
    public async Task ExecutarAsync_ComMuitasProficiencias_DeveMantarPerformance()
    {
        var proficiencias = new List<SME.Sondagem.Dominio.Entidades.Proficiencia>();
        
        for (int i = 1; i <= 1000; i++)
        {
            proficiencias.Add(new SME.Sondagem.Dominio.Entidades.Proficiencia($"Proficiência {i}", i % 5 + 1,ModalidadeId)
            {
                Id = i,
                CriadoEm = DateTime.Now.AddDays(-i),
                CriadoPor = $"Usuario{i}",
                CriadoRF = $"RF{i:000}"
            });
        }

        _repositorioProficienciaMock
            .Setup(x => x.ListarAsync(_cancellationToken))
            .ReturnsAsync(proficiencias);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var resultadoList = resultado.ToList();
        Assert.Equal(1000, resultadoList.Count);
        Assert.Equal("Proficiência 1", resultadoList.First().Nome);
        Assert.Equal("Proficiência 1000", resultadoList.Last().Nome);

        _repositorioProficienciaMock.Verify(x => x.ListarAsync(_cancellationToken), Times.Once);
    }
}