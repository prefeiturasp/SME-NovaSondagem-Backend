using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos.Questao;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questao;

public class AtualizarQuestaoUseCaseTeste
{
    private readonly Mock<IRepositorioQuestao> _repositorioQuestaoMock;
    private readonly AtualizarQuestaoUseCase _useCase;

    public AtualizarQuestaoUseCaseTeste()
    {
        _repositorioQuestaoMock = new Mock<IRepositorioQuestao>();
        _useCase = new AtualizarQuestaoUseCase(_repositorioQuestaoMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_QuestaoNaoExiste_DeveRetornarNull()
    {
        const long id = 999;
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            GrupoQuestoesId = 1,
            Ordem = 1,
            Nome = "Questao Teste",
            Observacao = "Observacao",
            Obrigatorio = true,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100,
            Tamanho = 50,
            Mascara = null,
            PlaceHolder = "Digite aqui",
            NomeComponente = "input-text",
            AlteradoPor = "Usuario Teste",
            AlteradoRF = "RF123"
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync((SME.Sondagem.Dominio.Entidades.Questionario.Questao?)null);

        var resultado = await _useCase.ExecutarAsync(id, questaoDto);

        Assert.Null(resultado);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_AtualizacaoFalha_DeveRetornarNull()
    {
        const long id = 1;
        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 2,
            GrupoQuestoesId = 2,
            Ordem = 2,
            Nome = "Questao Atualizada",
            Observacao = "Nova Observacao",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = true,
            Dimensao = 200,
            Tamanho = 100,
            Mascara = "###.###.###-##",
            PlaceHolder = "CPF",
            NomeComponente = "input-cpf",
            AlteradoPor = "Usuario",
            AlteradoRF = "RF456"
        };

        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            GrupoQuestoesId = 1,
            Ordem = 1,
            Nome = "Nome Original",
            Observacao = "Obs Original",
            Obrigatorio = true,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100,
            Tamanho = 50,
            Id = (int)id,
            CriadoEm = DateTime.Now.AddDays(-5),
            CriadoPor = "Usuario Criador",
            CriadoRF = "RF001"
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var resultado = await _useCase.ExecutarAsync(id, questaoDto);

        Assert.Null(resultado);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.AtualizarAsync(questaoExistente, cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_AtualizacaoComSucesso_DeveRetornarQuestaoDtoCompleto()
    {
        const long id = 1;
        var dataAntes = DateTime.Now;

        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 3,
            GrupoQuestoesId = 3,
            Ordem = 5,
            Nome = "Questao Atualizada",
            Observacao = "Observacao Atualizada",
            Obrigatorio = true,
            Tipo = TipoQuestao.ComboMultiplaEscolha,
            Opcionais = "{\"opcoes\": [\"A\", \"B\", \"C\"]}",
            SomenteLeitura = false,
            Dimensao = 300,
            Tamanho = 150,
            Mascara = "(##) #####-####",
            PlaceHolder = "Telefone",
            NomeComponente = "input-telefone",
            AlteradoPor = "Usuario Alterador",
            AlteradoRF = "RF789"
        };

        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            GrupoQuestoesId = 1,
            Ordem = 1,
            Nome = "Nome Original",
            Observacao = "Obs Original",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = true,
            Dimensao = 100,
            Tamanho = 50,
            PlaceHolder = "Place Original",
            NomeComponente = "componente-original",
            Id = (int)id,
            CriadoEm = new DateTime(2024, 1, 1, 10, 0, 0),
            CriadoPor = "Usuario Criador",
            CriadoRF = "RF001",
            AlteradoEm = null,
            AlteradoPor = null,
            AlteradoRF = null
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, questaoDto);
        var dataDepois = DateTime.Now;

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal(3, resultado.QuestionarioId);
        Assert.Equal(3, resultado.GrupoQuestoesId);
        Assert.Equal(5, resultado.Ordem);
        Assert.Equal("Questao Atualizada", resultado.Nome);
        Assert.Equal("Observacao Atualizada", resultado.Observacao);
        Assert.True(resultado.Obrigatorio);
        Assert.Equal(TipoQuestao.ComboMultiplaEscolha, resultado.Tipo);
        Assert.Equal("{\"opcoes\": [\"A\", \"B\", \"C\"]}", resultado.Opcionais);
        Assert.False(resultado.SomenteLeitura);
        Assert.Equal(300, resultado.Dimensao);
        Assert.Equal(150, resultado.Tamanho);
        Assert.Equal("(##) #####-####", resultado.Mascara);
        Assert.Equal("Telefone", resultado.PlaceHolder);
        Assert.Equal("input-telefone", resultado.NomeComponente);

        Assert.Equal(new DateTime(2024, 1, 1, 10, 0, 0), resultado.CriadoEm);
        Assert.Equal("Usuario Criador", resultado.CriadoPor);
        Assert.Equal("RF001", resultado.CriadoRF);

        Assert.NotNull(resultado.AlteradoEm);
        Assert.True(resultado.AlteradoEm >= dataAntes && resultado.AlteradoEm <= dataDepois);
        Assert.Equal("Usuario Alterador", resultado.AlteradoPor);
        Assert.Equal("RF789", resultado.AlteradoRF);

        Assert.Equal(3, questaoExistente.QuestionarioId);
        Assert.Equal(3, questaoExistente.GrupoQuestoesId);
        Assert.Equal(5, questaoExistente.Ordem);
        Assert.Equal("Questao Atualizada", questaoExistente.Nome);
        Assert.NotNull(questaoExistente.AlteradoEm);
        Assert.Equal("Usuario Alterador", questaoExistente.AlteradoPor);
        Assert.Equal("RF789", questaoExistente.AlteradoRF);

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.AtualizarAsync(questaoExistente, cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCancellationToken_DevePropagaCancellationToken()
    {
        const long id = 1;
        var cancellationToken = new CancellationToken();

        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            Nome = "Questao",
            Observacao = "Obs",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100
        };

        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            Ordem = 1,
            Nome = "Nome",
            Observacao = "Obs",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100,
            Id = (int)id
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: cancellationToken))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: cancellationToken))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, questaoDto, cancellationToken);

        Assert.NotNull(resultado);
        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: cancellationToken), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.AtualizarAsync(questaoExistente, cancellationToken: cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComGrupoQuestoesIdNulo_DeveAtualizarCorretamente()
    {
        const long id = 1;

        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            GrupoQuestoesId = null,
            Ordem = 1,
            Nome = "Questao Sem Grupo",
            Observacao = "Sem grupo",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100,
            Tamanho = null,
            Mascara = null,
            PlaceHolder = null,
            NomeComponente = null
        };

        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            GrupoQuestoesId = 2,
            Ordem = 1,
            Nome = "Nome",
            Observacao = "Obs",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100,
            Tamanho = 50,
            Mascara = "mask",
            PlaceHolder = "place",
            NomeComponente = "comp",
            Id = (int)id,
            CriadoEm = DateTime.Now,
            CriadoPor = "Usuario",
            CriadoRF = "RF001"
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, questaoDto);

        Assert.NotNull(resultado);
        Assert.Null(resultado.GrupoQuestoesId);
        Assert.Null(resultado.Tamanho);
        Assert.Null(resultado.Mascara);
        Assert.Null(resultado.PlaceHolder);
        Assert.Null(resultado.NomeComponente);
        Assert.Equal("Questao Sem Grupo", resultado.Nome);
    }

    [Fact]
    public async Task ExecutarAsync_DeveDefinirAlteradoEmComoDateTimeNow()
    {
        const long id = 1;
        var dataAntesExecucao = DateTime.Now;

        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            Ordem = 1,
            Nome = "Questao",
            Observacao = "Obs",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100,
            AlteradoPor = "Usuario",
            AlteradoRF = "RF123"
        };

        var questaoExistente = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = 1,
            Ordem = 1,
            Nome = "Nome",
            Observacao = "Obs",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100,
            Id = (int)id,
            AlteradoEm = null
        };

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _useCase.ExecutarAsync(id, questaoDto);
        var dataAposExecucao = DateTime.Now;

        Assert.NotNull(questaoExistente.AlteradoEm);
        Assert.True(questaoExistente.AlteradoEm >= dataAntesExecucao);
        Assert.True(questaoExistente.AlteradoEm <= dataAposExecucao);
    }
}