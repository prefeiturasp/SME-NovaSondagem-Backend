using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos.Questao;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questao;

public class CriarQuestaoUseCaseTeste
{
    private readonly Mock<IRepositorioQuestao> _repositorioQuestaoMock;
    private readonly CriarQuestaoUseCase _useCase;

    public CriarQuestaoUseCaseTeste()
    {
        _repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        _useCase = new CriarQuestaoUseCase(_repositorioQuestaoMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosValidos_DeveCriarQuestaoERetornarId()
    {
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            GrupoQuestoesId = 1,
            Ordem = 1,
            Nome = "Qual é a sua idade?",
            Observacao = "Informar idade completa",
            Obrigatorio = true,
            Tipo = TipoQuestao.Numerico,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100,
            Tamanho = 3,
            Mascara = null,
            PlaceHolder = "Ex: 25",
            NomeComponente = "input-idade"
        };

        const long expectedId = 123;

        _repositorioQuestaoMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var resultado = await _useCase.ExecutarAsync(questaoDto);

        Assert.Equal(expectedId, resultado);

        _repositorioQuestaoMock.Verify(x => x.CriarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(q => 
                q.QuestionarioId == questaoDto.QuestionarioId && 
                q.GrupoQuestoesId == questaoDto.GrupoQuestoesId &&
                q.Ordem == questaoDto.Ordem &&
                q.Nome == questaoDto.Nome &&
                q.Observacao == questaoDto.Observacao &&
                q.Obrigatorio == questaoDto.Obrigatorio &&
                q.Tipo == questaoDto.Tipo &&
                q.Opcionais == questaoDto.Opcionais &&
                q.SomenteLeitura == questaoDto.SomenteLeitura &&
                q.Dimensao == questaoDto.Dimensao &&
                q.Tamanho == questaoDto.Tamanho &&
                q.Mascara == questaoDto.Mascara &&
                q.PlaceHolder == questaoDto.PlaceHolder &&
                q.NomeComponente == questaoDto.NomeComponente),
            cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComGrupoQuestoesIdNulo_DeveCriarQuestaoCorretamente()
    {
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 2,
            GrupoQuestoesId = null,
            Ordem = 1,
            Nome = "Questao sem grupo",
            Observacao = "Observacao",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 200,
            Tamanho = null,
            Mascara = null,
            PlaceHolder = null,
            NomeComponente = null
        };

        const long expectedId = 456;

        _repositorioQuestaoMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var resultado = await _useCase.ExecutarAsync(questaoDto);

        Assert.Equal(expectedId, resultado);

        _repositorioQuestaoMock.Verify(x => x.CriarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(q => 
                q.GrupoQuestoesId == null &&
                q.Tamanho == null &&
                q.Mascara == null &&
                q.PlaceHolder == null &&
                q.NomeComponente == null),
            cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComTodosOsTiposDeQuestao_DeveCriarCorretamente()
    {
        var tiposQuestao = new[]
        {
            TipoQuestao.Texto,
            TipoQuestao.Numerico,
            TipoQuestao.ComboMultiplaEscolha,
            TipoQuestao.Combo
        };

        long idEsperado = 100;

        foreach (var tipo in tiposQuestao)
        {
            var questaoDto = new QuestaoDto
            {
                QuestionarioId = 1,
                Ordem = 1,
                Nome = $"Questao tipo {tipo}",
                Observacao = "Obs",
                Obrigatorio = true,
                Tipo = tipo,
                Opcionais = "{}",
                SomenteLeitura = false,
                Dimensao = 100
            };

            _repositorioQuestaoMock
                .Setup(x => x.CriarAsync(
                    It.Is<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(q => q.Tipo == tipo), 
                    cancellationToken: It.IsAny<CancellationToken>()))
                .ReturnsAsync(idEsperado++);

            var resultado = await _useCase.ExecutarAsync(questaoDto);

            Assert.True(resultado > 0);
        }

        _repositorioQuestaoMock.Verify(x => x.CriarAsync(
            It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), 
            cancellationToken: It.IsAny<CancellationToken>()), Times.Exactly(tiposQuestao.Length));
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationToken_DevePropararExcecao()
    {
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            Ordem = 1,
            Nome = "Teste",
            Observacao = "Obs",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100
        };

        var cancellationTokenCancelado = new CancellationToken(true);

        _repositorioQuestaoMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: cancellationTokenCancelado))
            .ThrowsAsync(new OperationCanceledException());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecutarAsync(questaoDto, cancellationTokenCancelado));
    }

    [Fact]
    public async Task ExecutarAsync_QuandoRepositorioFalha_DevePropararExcecao()
    {
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            Ordem = 1,
            Nome = "Questao Teste",
            Observacao = "Obs",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100
        };

        _repositorioQuestaoMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Erro do repositório"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecutarAsync(questaoDto));

        Assert.Equal("Erro do repositório", exception.Message);
    }

    [Fact]
    public async Task ExecutarAsync_DeveCriarEntidadeComTodosOsParametrosCorretos()
    {
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 5,
            GrupoQuestoesId = 3,
            Ordem = 10,
            Nome = "Nome da Questao Específica",
            Observacao = "Observacao detalhada",
            Obrigatorio = true,
            Tipo = TipoQuestao.ComboMultiplaEscolha,
            Opcionais = "{\"opcoes\": [\"Sim\", \"Não\", \"Talvez\"]}",
            SomenteLeitura = true,
            Dimensao = 500,
            Tamanho = 250,
            Mascara = "(##) #####-####",
            PlaceHolder = "Digite o telefone",
            NomeComponente = "input-telefone-custom"
        };

        SME.Sondagem.Dominio.Entidades.Questionario.Questao? questaoCapturada = null;
        
        _repositorioQuestaoMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .Callback<SME.Sondagem.Dominio.Entidades.Questionario.Questao, CancellationToken>((q, ct) => questaoCapturada = q)
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(questaoDto);

        Assert.NotNull(questaoCapturada);
        Assert.Equal(5, questaoCapturada.QuestionarioId);
        Assert.Equal(3, questaoCapturada.GrupoQuestoesId);
        Assert.Equal(10, questaoCapturada.Ordem);
        Assert.Equal("Nome da Questao Específica", questaoCapturada.Nome);
        Assert.Equal("Observacao detalhada", questaoCapturada.Observacao);
        Assert.True(questaoCapturada.Obrigatorio);
        Assert.Equal(TipoQuestao.ComboMultiplaEscolha, questaoCapturada.Tipo);
        Assert.Equal("{\"opcoes\": [\"Sim\", \"Não\", \"Talvez\"]}", questaoCapturada.Opcionais);
        Assert.True(questaoCapturada.SomenteLeitura);
        Assert.Equal(500, questaoCapturada.Dimensao);
        Assert.Equal(250, questaoCapturada.Tamanho);
        Assert.Equal("(##) #####-####", questaoCapturada.Mascara);
        Assert.Equal("Digite o telefone", questaoCapturada.PlaceHolder);
        Assert.Equal("input-telefone-custom", questaoCapturada.NomeComponente);
    }

    [Fact]
    public async Task ExecutarAsync_ComDiferentesCancellationTokens_DevePropararParaRepositorio()
    {
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            Ordem = 1,
            Nome = "Teste",
            Observacao = "Obs",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100
        };

        var customCancellationToken = new CancellationTokenSource().Token;

        _repositorioQuestaoMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: customCancellationToken))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(questaoDto, customCancellationToken);

        _repositorioQuestaoMock.Verify(x => x.CriarAsync(
            It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), 
            cancellationToken: customCancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComOpcionaisComplexo_DeveCriarCorretamente()
    {
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            Ordem = 1,
            Nome = "Questao com opcionais complexos",
            Observacao = "Obs",
            Obrigatorio = true,
            Tipo = TipoQuestao.ComboMultiplaEscolha,
            Opcionais = "{\"opcoes\": [{\"id\": 1, \"texto\": \"Opção A\", \"valor\": 10}, {\"id\": 2, \"texto\": \"Opção B\", \"valor\": 20}]}",
            SomenteLeitura = false,
            Dimensao = 300
        };

        _repositorioQuestaoMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(questaoDto);

        Assert.True(resultado > 0);
        _repositorioQuestaoMock.Verify(x => x.CriarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(q => 
                q.Opcionais == questaoDto.Opcionais),
            cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComQuestaoObrigatoria_DeveCriarComFlagCorreta()
    {
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            Ordem = 1,
            Nome = "Questao Obrigatoria",
            Observacao = "Campo obrigatório",
            Obrigatorio = true,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100
        };

        _repositorioQuestaoMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(questaoDto);

        _repositorioQuestaoMock.Verify(x => x.CriarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(q => q.Obrigatorio == true),
            cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComQuestaoSomenteLeitura_DeveCriarComFlagCorreta()
    {
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            Ordem = 1,
            Nome = "Questao Somente Leitura",
            Observacao = "Não editável",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = true,
            Dimensao = 100
        };

        _repositorioQuestaoMock
            .Setup(x => x.CriarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _useCase.ExecutarAsync(questaoDto);

        _repositorioQuestaoMock.Verify(x => x.CriarAsync(
            It.Is<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(q => q.SomenteLeitura == true),
            cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
    }
}