using Moq;
using SME.Sondagem.Aplicacao.UseCases.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
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

    // Método auxiliar para criar instâncias de Questao nos testes
    private static SME.Sondagem.Dominio.Entidades.Questionario.Questao CriarQuestao(
        int questionarioId = 1,
        int ordem = 1,
        string nome = "Nome Teste",
        string observacao = "Observacao Teste",
        bool obrigatorio = false,
        TipoQuestao tipo = TipoQuestao.Texto,
        string opcionais = "{}",
        bool somenteLeitura = false,
        int dimensao = 100,
        int? grupoQuestoesId = null,
        int? tamanho = null,
        string? mascara = null,
        string? placeHolder = null,
        string? nomeComponente = null,
        int? id = null,
        DateTime? criadoEm = null,
        string? criadoPor = null,
        string? criadoRF = null,
        DateTime? alteradoEm = null,
        string? alteradoPor = null,
        string? alteradoRF = null)
    {
        var questao = new SME.Sondagem.Dominio.Entidades.Questionario.Questao(
            questionarioId: questionarioId,
            ordem: ordem,
            nome: nome,
            observacao: observacao,
            obrigatorio: obrigatorio,
            tipo: tipo,
            opcionais: opcionais,
            somenteLeitura: somenteLeitura,
            dimensao: dimensao,
            grupoQuestoesId: grupoQuestoesId,
            tamanho: tamanho,
            mascara: mascara,
            placeHolder: placeHolder,
            nomeComponente: nomeComponente
        );

        // Usa reflexão para definir propriedades da classe base
        if (id.HasValue)
        {
            var idProp = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("Id");
            idProp?.SetValue(questao, id.Value);
        }

        if (criadoEm.HasValue)
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("CriadoEm");
            prop?.SetValue(questao, criadoEm.Value);
        }

        if (!string.IsNullOrEmpty(criadoPor))
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("CriadoPor");
            prop?.SetValue(questao, criadoPor);
        }

        if (!string.IsNullOrEmpty(criadoRF))
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("CriadoRF");
            prop?.SetValue(questao, criadoRF);
        }

        if (alteradoEm.HasValue)
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("AlteradoEm");
            prop?.SetValue(questao, alteradoEm);
        }

        if (!string.IsNullOrEmpty(alteradoPor))
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("AlteradoPor");
            prop?.SetValue(questao, alteradoPor);
        }

        if (!string.IsNullOrEmpty(alteradoRF))
        {
            var prop = typeof(SME.Sondagem.Dominio.Entidades.Questionario.Questao).GetProperty("AlteradoRF");
            prop?.SetValue(questao, alteradoRF);
        }

        return questao;
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

        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            grupoQuestoesId: 1,
            ordem: 1,
            nome: "Nome Original",
            observacao: "Obs Original",
            obrigatorio: true,
            tipo: TipoQuestao.Texto,
            opcionais: "{}",
            somenteLeitura: false,
            dimensao: 100,
            tamanho: 50,
            id: (int)id,
            criadoEm: DateTime.Now.AddDays(-5),
            criadoPor: "Usuario Criador",
            criadoRF: "RF001"
        );

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

        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            grupoQuestoesId: 1,
            ordem: 1,
            nome: "Nome Original",
            observacao: "Obs Original",
            obrigatorio: false,
            tipo: TipoQuestao.Texto,
            opcionais: "{}",
            somenteLeitura: true,
            dimensao: 100,
            tamanho: 50,
            placeHolder: "Place Original",
            nomeComponente: "componente-original",
            id: (int)id,
            criadoEm: new DateTime(2024, 1, 1, 10, 0, 0),
            criadoPor: "Usuario Criador",
            criadoRF: "RF001"
        );

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, questaoDto);

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

        // As propriedades de alteração permanecem null pois o use case atual não as define
        Assert.Null(resultado.AlteradoEm);
        Assert.Null(resultado.AlteradoPor);
        Assert.Null(resultado.AlteradoRF);

        // Verifica se os dados foram atualizados na entidade
        Assert.Equal(3, questaoExistente.QuestionarioId);
        Assert.Equal(3, questaoExistente.GrupoQuestoesId);
        Assert.Equal(5, questaoExistente.Ordem);
        Assert.Equal("Questao Atualizada", questaoExistente.Nome);

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

        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            ordem: 1,
            nome: "Nome",
            observacao: "Obs",
            obrigatorio: false,
            tipo: TipoQuestao.Texto,
            opcionais: "{}",
            somenteLeitura: false,
            dimensao: 100,
            id: (int)id
        );

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

        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            grupoQuestoesId: 2,
            ordem: 1,
            nome: "Nome",
            observacao: "Obs",
            obrigatorio: false,
            tipo: TipoQuestao.Texto,
            opcionais: "{}",
            somenteLeitura: false,
            dimensao: 100,
            tamanho: 50,
            mascara: "mask",
            placeHolder: "place",
            nomeComponente: "comp",
            id: (int)id,
            criadoEm: DateTime.Now,
            criadoPor: "Usuario",
            criadoRF: "RF001"
        );

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
    public async Task ExecutarAsync_DeveAtualizarPropriedadesDaQuestao()
    {
        const long id = 1;

        var questaoDto = new QuestaoDto
        {
            QuestionarioId = 1,
            Ordem = 1,
            Nome = "Questao Atualizada",
            Observacao = "Observacao Atualizada",
            Obrigatorio = false,
            Tipo = TipoQuestao.Texto,
            Opcionais = "{}",
            SomenteLeitura = false,
            Dimensao = 100,
            AlteradoPor = "Usuario",
            AlteradoRF = "RF123"
        };

        var questaoExistente = CriarQuestao(
            questionarioId: 1,
            ordem: 1,
            nome: "Nome Original",
            observacao: "Obs Original",
            obrigatorio: false,
            tipo: TipoQuestao.Texto,
            opcionais: "{}",
            somenteLeitura: false,
            dimensao: 100,
            id: (int)id
        );

        _repositorioQuestaoMock
            .Setup(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(questaoExistente);

        _repositorioQuestaoMock
            .Setup(x => x.AtualizarAsync(It.IsAny<SME.Sondagem.Dominio.Entidades.Questionario.Questao>(), cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(id, questaoDto);

        // Verifica se a atualização foi bem-sucedida
        Assert.NotNull(resultado);
        Assert.Equal("Questao Atualizada", questaoExistente.Nome);
        Assert.Equal("Observacao Atualizada", questaoExistente.Observacao);

        // Verifica que as propriedades de auditoria permanecem como estavam
        // pois o use case atual não as define automaticamente
        Assert.Null(questaoExistente.AlteradoEm);

        _repositorioQuestaoMock.Verify(x => x.ObterPorIdAsync(id, cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
        _repositorioQuestaoMock.Verify(x => x.AtualizarAsync(questaoExistente, cancellationToken: It.IsAny<CancellationToken>()), Times.Once);
    }
}