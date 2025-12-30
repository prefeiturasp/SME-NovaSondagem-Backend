using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questao;

public class ObterQuestoesUseCaseTeste
{
    private readonly Mock<IRepositorioQuestao> _repositorioQuestaoMock;
    private readonly ObterQuestoesUseCase _useCase;
    private readonly CancellationToken _cancellationToken;

    public ObterQuestoesUseCaseTeste()
    {
        _repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        _useCase = new ObterQuestoesUseCase(_repositorioQuestaoMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarListaDeQuestoesDto()
    { 
        var questoes = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao>
        {
            new(1, null, 1, "Questao 1", "Observacao 1", true, TipoQuestao.Texto, "", false, 1, null, null, null, null)
            {
                Id = 1,
                CriadoEm = DateTime.Now,
                CriadoPor = "Usuario1",
                CriadoRF = "RF001"
            },
            new(2, null, 2, "Questao 2", "Observacao 2", false, TipoQuestao.Radio, "", false, 1, null, null, null, null)
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

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(questoes);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        var resultadoList = resultado.ToList();
        Assert.Equal(2, resultadoList.Count);

        var primeira = resultadoList.First(x => x.Id == 1);
        Assert.Equal("Questao 1", primeira.Nome);
        Assert.Equal(1, primeira.QuestionarioId);
        Assert.Equal("Usuario1", primeira.CriadoPor);
        Assert.Equal("RF001", primeira.CriadoRF);
        Assert.Null(primeira.AlteradoEm);
        Assert.Null(primeira.AlteradoPor);
        Assert.Null(primeira.AlteradoRF);

        var segunda = resultadoList.First(x => x.Id == 2);
        Assert.Equal("Questao 2", segunda.Nome);
        Assert.Equal(2, segunda.QuestionarioId);
        Assert.Equal("Usuario2", segunda.CriadoPor);
        Assert.Equal("RF002", segunda.CriadoRF);
        Assert.NotNull(segunda.AlteradoEm);
        Assert.Equal("Usuario3", segunda.AlteradoPor);
        Assert.Equal("RF003", segunda.AlteradoRF);

        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoNaoHaQuestoes_DeveRetornarListaVazia()
    {
        var questoesVazias = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao>();

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(questoesVazias);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        Assert.NotNull(resultado);
        Assert.Empty(resultado);

        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationTokenCancelado_DevePropararExcecao()
    {
        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(cancellationTokenCancelado));

        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(cancellationTokenCancelado), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveChamarRepositorioComCancellationTokenCorreto()
    {
        var cancellationTokenCustom = new CancellationTokenSource().Token;
        var questoes = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao>();

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(cancellationTokenCustom))
            .ReturnsAsync(questoes);

        await _useCase.ExecutarAsync(cancellationTokenCustom);

        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(cancellationTokenCustom), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(_cancellationToken));

        Assert.Equal("Erro do repositório", exception.Message);
        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveMaperarTodasAsPropriedadesCorretamente()
    {
        var dataEspecifica = new DateTime(2023, 10, 15, 14, 30, 0);
        var dataAlteracao = new DateTime(2023, 10, 16, 10, 15, 0);
        
        var questoes = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao>
        {
            new(100, 1, 3, "Matemática Básica", "Observação detalhada", true, TipoQuestao.Numerico, "opcionais", false, 2, 50, "###.###", "Digite aqui", "ComponenteX")
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

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(questoes);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var dto = resultado.Single();
        Assert.Equal(100, dto.Id);
        Assert.Equal("Matemática Básica", dto.Nome);
        Assert.Equal(100, dto.QuestionarioId);
        Assert.Equal(1, dto.GrupoQuestoesId);
        Assert.Equal(3, dto.Ordem);
        Assert.Equal("Observação detalhada", dto.Observacao);
        Assert.True(dto.Obrigatorio);
        Assert.Equal(TipoQuestao.Numerico, dto.Tipo);
        Assert.Equal("opcionais", dto.Opcionais);
        Assert.False(dto.SomenteLeitura);
        Assert.Equal(2, dto.Dimensao);
        Assert.Equal(50, dto.Tamanho);
        Assert.Equal("###.###", dto.Mascara);
        Assert.Equal("Digite aqui", dto.PlaceHolder);
        Assert.Equal("ComponenteX", dto.NomeComponente);
        Assert.Equal(dataEspecifica, dto.CriadoEm);
        Assert.Equal("Sistema", dto.CriadoPor);
        Assert.Equal("RF999", dto.CriadoRF);
        Assert.Equal(dataAlteracao, dto.AlteradoEm);
        Assert.Equal("Admin", dto.AlteradoPor);
        Assert.Equal("RF999", dto.AlteradoRF);
    }

    [Fact]
    public async Task ExecutarAsync_ComMuitasQuestoes_DeveMantarPerformance()
    {
        var questoes = new List<SME.Sondagem.Dominio.Entidades.Questionario.Questao>();
        
        for (int i = 1; i <= 1000; i++)
        {
            questoes.Add(new SME.Sondagem.Dominio.Entidades.Questionario.Questao(
                i % 5 + 1, 
                null, 
                i, 
                $"Questao {i}", 
                $"Observacao {i}", 
                i % 2 == 0, 
                TipoQuestao.Texto, 
                "", 
                false, 
                1, 
                null, 
                null, 
                null, 
                null)
            {
                Id = i,
                CriadoEm = DateTime.Now.AddDays(-i),
                CriadoPor = $"Usuario{i}",
                CriadoRF = $"RF{i:000}"
            });
        }

        _repositorioQuestaoMock
            .Setup(x => x.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(questoes);

        var resultado = await _useCase.ExecutarAsync(_cancellationToken);

        var resultadoList = resultado.ToList();
        Assert.Equal(1000, resultadoList.Count);
        Assert.Equal("Questao 1", resultadoList.First().Nome);
        Assert.Equal("Questao 1000", resultadoList.Last().Nome);

        _repositorioQuestaoMock.Verify(x => x.ObterTodosAsync(_cancellationToken), Times.Once);
    }
}