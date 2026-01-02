using Moq;
using SME.Sondagem.Aplicacao.UseCases.Ciclo;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Ciclo;

public class ObterCiclosUseCaseTeste
{
    private readonly Mock<IRepositorioCiclo> _repositorioCicloMock;
    private readonly ObterCiclosUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ObterCiclosUseCaseTeste()
    {
        _repositorioCicloMock = new Mock<IRepositorioCiclo>();
        _useCase = new ObterCiclosUseCase(_repositorioCicloMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarListaDeCiclosDto()
    { 
        var ciclos = new List<SME.Sondagem.Dominio.Entidades.Ciclo>
        {
            new(1, "Ciclo 1")
            {
                Id = 1,
                CriadoEm = DateTime.Now,
                CriadoPor = "Usuario1",
                CriadoRF = "RF001"
            },
            new(2, "Ciclo 2")
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

        _repositorioCicloMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(ciclos);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        var resultadoList = resultado.ToList();
        Assert.Equal(2, resultadoList.Count);

        var primeira = resultadoList.First(x => x.Id == 1);
        Assert.Equal("Ciclo 1", primeira.DescCiclo);
        Assert.Equal(1, primeira.CodCicloEnsinoEol);
        Assert.Equal("Usuario1", primeira.CriadoPor);
        Assert.Equal("RF001", primeira.CriadoRF);
        Assert.Null(primeira.AlteradoEm);
        Assert.Null(primeira.AlteradoPor);
        Assert.Null(primeira.AlteradoRF);

        var segunda = resultadoList.First(x => x.Id == 2);
        Assert.Equal("Ciclo 2", segunda.DescCiclo);
        Assert.Equal(2, segunda.CodCicloEnsinoEol);
        Assert.Equal("Usuario2", segunda.CriadoPor);
        Assert.Equal("RF002", segunda.CriadoRF);
        Assert.NotNull(segunda.AlteradoEm);
        Assert.Equal("Usuario3", segunda.AlteradoPor);
        Assert.Equal("RF003", segunda.AlteradoRF);

        _repositorioCicloMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoNaoHaCiclos_DeveRetornarListaVazia()
    {
        var ciclosVazias = new List<SME.Sondagem.Dominio.Entidades.Ciclo>();

        _repositorioCicloMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(ciclosVazias);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        Assert.Empty(resultado);

        _repositorioCicloMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioCicloMock
            .Setup(x => x.ObterTodosAsync(cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(cancellationTokenCancelado));

        _repositorioCicloMock.Verify(x => x.ObterTodosAsync(cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComCancellationTokenCorreto()
    {
        var cancellationTokenCustom = new CancellationTokenSource().Token;
        var ciclos = new List<SME.Sondagem.Dominio.Entidades.Ciclo>();

        _repositorioCicloMock
            .Setup(x => x.ObterTodosAsync(cancellationTokenCustom))
            .ReturnsAsync(ciclos);

        await _useCase.ExecutarAsync(cancellationTokenCustom);

        _repositorioCicloMock.Verify(x => x.ObterTodosAsync(cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        _repositorioCicloMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(_cancellationToken));

        Assert.Equal("Erro do repositório", exception.Message);
        _repositorioCicloMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveMaperarTodasAsPropriedadesCorretamente()
    {
        var dataEspecifica = new DateTime(2023, 10, 15, 14, 30, 0);
        var dataAlteracao = new DateTime(2023, 10, 16, 10, 15, 0);
        
        var ciclos = new List<SME.Sondagem.Dominio.Entidades.Ciclo>
        {
            new(3, "Matemática Básica")
            {
                Id = 100,
                CriadoEm = dataEspecifica,
                CriadoPor = "Sistema",
                CriadoRF = "RF999",
                AlteradoEm = dataAlteracao,
                AlteradoPor = "Admin",
                AlteradoRF = "RF999"
            }
        };

        _repositorioCicloMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(ciclos);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var dto = resultado.Single();
        Assert.Equal(100, dto.Id);
        Assert.Equal("Matemática Básica", dto.DescCiclo);
        Assert.Equal(3, dto.CodCicloEnsinoEol);
        Assert.Equal(dataEspecifica, dto.CriadoEm);
        Assert.Equal("Sistema", dto.CriadoPor);
        Assert.Equal("RF999", dto.CriadoRF);
        Assert.Equal(dataAlteracao, dto.AlteradoEm);
        Assert.Equal("Admin", dto.AlteradoPor);
        Assert.Equal("RF999", dto.AlteradoRF);
    }

    [Fact]
    public async Task ExecutarAsync_ComMuitasCiclos_DeveMantarPerformance()
    {
        var ciclos = new List<SME.Sondagem.Dominio.Entidades.Ciclo>();
        
        for (int i = 1; i <= 1000; i++)
        {
            ciclos.Add(new SME.Sondagem.Dominio.Entidades.Ciclo(i % 5 + 1, $"Ciclo {i}")
            {
                Id = i,
                CriadoEm = DateTime.Now.AddDays(-i),
                CriadoPor = $"Usuario{i}",
                CriadoRF = $"RF{i:000}"
            });
        }

        _repositorioCicloMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(ciclos);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var resultadoList = resultado.ToList();
        Assert.Equal(1000, resultadoList.Count);
        Assert.Equal("Ciclo 1", resultadoList.First().DescCiclo);
        Assert.Equal("Ciclo 1000", resultadoList.Last().DescCiclo);

        _repositorioCicloMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }
}