using Moq;
using SME.Sondagem.Aplicacao.UseCases.Bimestre;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Bimestre;

public class ObterBimestresUseCaseTeste
{
    private readonly Mock<IRepositorioBimestre> _repositorioBimestreMock;
    private readonly ObterBimestresUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ObterBimestresUseCaseTeste()
    {
        _repositorioBimestreMock = new Mock<IRepositorioBimestre>();
        _useCase = new ObterBimestresUseCase(_repositorioBimestreMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarListaDeBimestreDto()
    { 
        var bimestres = new List<Dominio.Entidades.Bimestre>
        {
            new(1, "Bimestre 1")
            {
                Id = 1,
                CriadoEm = DateTime.Now,
                CriadoPor = "Usuario1",
                CriadoRF = "RF001"
            },
            new(2, "Bimestre 2")
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

        _repositorioBimestreMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(bimestres);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        var resultadoList = resultado.ToList();
        Assert.Equal(2, resultadoList.Count);

        var primeira = resultadoList.First(x => x.Id == 1);
        Assert.Equal("Bimestre 1", primeira.Descricao);
        Assert.Equal(1, primeira.CodBimestreEnsinoEol);
        Assert.Equal("Usuario1", primeira.CriadoPor);
        Assert.Equal("RF001", primeira.CriadoRF);
        Assert.Null(primeira.AlteradoEm);
        Assert.Null(primeira.AlteradoPor);
        Assert.Null(primeira.AlteradoRF);

        var segunda = resultadoList.First(x => x.Id == 2);
        Assert.Equal("Bimestre 2", segunda.Descricao);
        Assert.Equal(2, segunda.CodBimestreEnsinoEol);
        Assert.Equal("Usuario2", segunda.CriadoPor);
        Assert.Equal("RF002", segunda.CriadoRF);
        Assert.NotNull(segunda.AlteradoEm);
        Assert.Equal("Usuario3", segunda.AlteradoPor);
        Assert.Equal("RF003", segunda.AlteradoRF);

        _repositorioBimestreMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoNaoHaBimestres_DeveRetornarListaVazia()
    {
        var bimestresVazias = new List<Dominio.Entidades.Bimestre>();

        _repositorioBimestreMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(bimestresVazias);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        Assert.Empty(resultado);

        _repositorioBimestreMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioBimestreMock
            .Setup(x => x.ObterTodosAsync(cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(cancellationTokenCancelado));

        _repositorioBimestreMock.Verify(x => x.ObterTodosAsync(cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComCancellationTokenCorreto()
    {
        var cancellationTokenCustom = new CancellationTokenSource().Token;
        var bimestres = new List<Dominio.Entidades.Bimestre>();

        _repositorioBimestreMock
            .Setup(x => x.ObterTodosAsync(cancellationTokenCustom))
            .ReturnsAsync(bimestres);

        await _useCase.ExecutarAsync(cancellationTokenCustom);

        _repositorioBimestreMock.Verify(x => x.ObterTodosAsync(cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        _repositorioBimestreMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(_cancellationToken));

        Assert.Equal("Erro do repositório", exception.Message);
        _repositorioBimestreMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveMaperarTodasAsPropriedadesCorretamente()
    {
        var dataEspecifica = new DateTime(2023, 10, 15, 14, 30, 0);
        var dataAlteracao = new DateTime(2023, 10, 16, 10, 15, 0);
        
        var bimestres = new List<Dominio.Entidades.Bimestre>
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

        _repositorioBimestreMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(bimestres);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var dto = resultado.Single();
        Assert.Equal(100, dto.Id);
        Assert.Equal("Matemática Básica", dto.Descricao);
        Assert.Equal(3, dto.CodBimestreEnsinoEol);
        Assert.Equal(dataEspecifica, dto.CriadoEm);
        Assert.Equal("Sistema", dto.CriadoPor);
        Assert.Equal("RF999", dto.CriadoRF);
        Assert.Equal(dataAlteracao, dto.AlteradoEm);
        Assert.Equal("Admin", dto.AlteradoPor);
        Assert.Equal("RF999", dto.AlteradoRF);
    }

    [Fact]
    public async Task ExecutarAsync_ComMuitasBimestres_DeveMantarPerformance()
    {
        var bimestres = new List<Dominio.Entidades.Bimestre>();
        
        for (int i = 1; i <= 1000; i++)
        {
            bimestres.Add(new Dominio.Entidades.Bimestre(i % 5 + 1, $"Bimestre {i}")
            {
                Id = i,
                CriadoEm = DateTime.Now.AddDays(-i),
                CriadoPor = $"Usuario{i}",
                CriadoRF = $"RF{i:000}"
            });
        }

        _repositorioBimestreMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(bimestres);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var resultadoList = resultado.ToList();
        Assert.Equal(1000, resultadoList.Count);
        Assert.Equal("Bimestre 1", resultadoList.First().Descricao);
        Assert.Equal("Bimestre 1000", resultadoList.Last().Descricao);

        _repositorioBimestreMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }
}