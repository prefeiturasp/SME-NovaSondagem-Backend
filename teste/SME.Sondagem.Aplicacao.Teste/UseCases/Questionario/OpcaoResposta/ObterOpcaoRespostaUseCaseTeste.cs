using Moq;
using SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.OpcaoResposta;

public class ObterOpcoesRespostaUseCaseTeste
{
    private readonly Mock<IRepositorioOpcaoResposta> _repositorioOpcaoRespostaMock;
    private readonly ObterOpcaoRespostasUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ObterOpcoesRespostaUseCaseTeste()
    {
        _repositorioOpcaoRespostaMock = new Mock<IRepositorioOpcaoResposta>();
        _useCase = new ObterOpcaoRespostasUseCase(_repositorioOpcaoRespostaMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarListaDeOpcoesRespostaDto()
    {
        var opcoesResposta = new List<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>
        {
            new("Opção A", "Legenda A", "#FF0000", "#FFFFFF")
            {
                Id = 1,
                CriadoEm = DateTime.Now,
                CriadoPor = "Usuario1",
                CriadoRF = "RF001"
            },
            new("Opção B", "Legenda B", "#00FF00", "#000000")
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

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(opcoesResposta);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        var resultadoList = resultado.ToList();
        Assert.Equal(2, resultadoList.Count);

        var primeira = resultadoList.First(x => x.DescricaoOpcaoResposta == "Opção A");
        Assert.Equal("Opção A", primeira.DescricaoOpcaoResposta);
        Assert.Equal("Legenda A", primeira.Legenda);
        Assert.Equal("#FF0000", primeira.CorFundo);
        Assert.Equal("#FFFFFF", primeira.CorTexto);
        Assert.Equal("Usuario1", primeira.CriadoPor);
        Assert.Equal("RF001", primeira.CriadoRF);
        Assert.Null(primeira.AlteradoEm);
        Assert.Null(primeira.AlteradoPor);
        Assert.Null(primeira.AlteradoRF);

        var segunda = resultadoList.First(x => x.DescricaoOpcaoResposta == "Opção B");
        Assert.Equal("Opção B", segunda.DescricaoOpcaoResposta);
        Assert.Equal("Legenda B", segunda.Legenda);
        Assert.Equal("#00FF00", segunda.CorFundo);
        Assert.Equal("#000000", segunda.CorTexto);
        Assert.Equal("Usuario2", segunda.CriadoPor);
        Assert.Equal("RF002", segunda.CriadoRF);
        Assert.NotNull(segunda.AlteradoEm);
        Assert.Equal("Usuario3", segunda.AlteradoPor);
        Assert.Equal("RF003", segunda.AlteradoRF);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoNaoHaOpcoesResposta_DeveRetornarListaVazia()
    {
        var opcoesRespostaVazias = new List<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>();

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(opcoesRespostaVazias);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        Assert.Empty(resultado);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropagar_Excecao()
    {
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterTodosAsync(cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(cancellationTokenCancelado));

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterTodosAsync(cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComCancellationTokenCorreto()
    {
        var cancellationTokenCustom = new CancellationTokenSource().Token;
        var opcoesResposta = new List<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>();

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterTodosAsync(cancellationTokenCustom))
            .ReturnsAsync(opcoesResposta);

        await _useCase.ExecutarAsync(cancellationTokenCustom);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterTodosAsync(cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropagar_Excecao()
    {
        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(_cancellationToken));

        Assert.Equal("Erro do repositório", exception.Message);
        _repositorioOpcaoRespostaMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveMaperarTodasAsPropriedadesCorretamente()
    {
        var dataEspecifica = new DateTime(2023, 10, 15, 14, 30, 0);
        var dataAlteracao = new DateTime(2023, 10, 16, 10, 15, 0);

        var opcoesResposta = new List<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>
        {
            new("Opção Completa", "Legenda detalhada", "#0000FF", "#FFFF00")
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

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(opcoesResposta);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var dto = resultado.Single();
        Assert.Equal("Opção Completa", dto.DescricaoOpcaoResposta);
        Assert.Equal("Legenda detalhada", dto.Legenda);
        Assert.Equal("#0000FF", dto.CorFundo);
        Assert.Equal("#FFFF00", dto.CorTexto);
        Assert.Equal(dataEspecifica, dto.CriadoEm);
        Assert.Equal("Sistema", dto.CriadoPor);
        Assert.Equal("RF999", dto.CriadoRF);
        Assert.Equal(dataAlteracao, dto.AlteradoEm);
        Assert.Equal("Admin", dto.AlteradoPor);
        Assert.Equal("RF999", dto.AlteradoRF);
    }

    [Fact]
    public async Task ExecutarAsync_ComMuitasOpcoesResposta_DeveManterPerformance()
    {
        var opcoesResposta = new List<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>();

        for (int i = 1; i <= 1000; i++)
        {
            opcoesResposta.Add(new SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta(
                $"Opção {i}",
                $"Legenda {i}",
                $"#FF{i:X4}",
                $"#00{i:X4}")
            {
                Id = i,
                CriadoEm = DateTime.Now.AddDays(-i),
                CriadoPor = $"Usuario{i}",
                CriadoRF = $"RF{i:000}"
            });
        }

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(opcoesResposta);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var resultadoList = resultado.ToList();
        Assert.Equal(1000, resultadoList.Count);
        Assert.Equal("Opção 1", resultadoList.First().DescricaoOpcaoResposta);
        Assert.Equal("Opção 1000", resultadoList.Last().DescricaoOpcaoResposta);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCamposOpcionaisNulos_DeveMaperarCorretamente()
    {
        var opcoesResposta = new List<SME.Sondagem.Dominio.Entidades.Questionario.OpcaoResposta>
        {
            new("Opção sem campos opcionais", null, null, null)
            {
                Id = 1,
                CriadoEm = DateTime.Now,
                CriadoPor = "Usuario1",
                CriadoRF = "RF001"
            }
        };

        _repositorioOpcaoRespostaMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(opcoesResposta);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var dto = resultado.Single();
        Assert.Equal("Opção sem campos opcionais", dto.DescricaoOpcaoResposta);
        Assert.Null(dto.Legenda);
        Assert.Null(dto.CorFundo);
        Assert.Null(dto.CorTexto);

        _repositorioOpcaoRespostaMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }
}