using MediatR;
using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Base;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Interfaces;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario.Base;

internal partial class QuestionarioSondagemUseCaseBaseConcreto : QuestionarioSondagemUseCaseBase
{
    private readonly DadosAlunosDto _dadosAlunos;

    public QuestionarioSondagemUseCaseBaseConcreto(
        RepositoriosElastic repositoriosElastic,
        RepositoriosSondagem repositoriosSondagem,
        IAlunoPapService alunoPapService,
        IControleAcessoService controleAcessoService,
        IServicoUsuario servicoUsuario,
        IMediator mediator,
        DadosAlunosDto? dadosAlunos = null)
        : base(repositoriosElastic, repositoriosSondagem, alunoPapService, controleAcessoService, servicoUsuario,mediator)
    {
        _dadosAlunos = dadosAlunos ?? new DadosAlunosDto
        {
            AlunosComPap = new Dictionary<int, bool>(),
            AlunosComLinguaPortuguesaSegundaLingua = new Dictionary<int, bool>(),
            DadosRacaGenero = null
        };
    }

    protected override Task<DadosAlunosDto> ObterDadosAlunos(
        int turmaId,
        int anoLetivo,
        ContextoProcessamentoDto contexto,
        CancellationToken cancellationToken)
        => Task.FromResult(_dadosAlunos);
}

public class QuestionarioSondagemUseCaseBaseTeste
{
    #region Mocks

    private readonly Mock<IRepositorioElasticTurma> _mockRepositorioElasticTurma;
    private readonly Mock<IRepositorioElasticAluno> _mockRepositorioElasticAluno;
    private readonly Mock<IRepositorioSondagem> _mockRepositorioSondagem;
    private readonly Mock<IRepositorioQuestao> _mockRepositorioQuestao;
    private readonly Mock<IRepositorioRespostaAluno> _mockRepositorioRespostaAluno;
    private readonly Mock<IRepositorioBimestre> _mockRepositorioBimestre;
    private readonly Mock<IAlunoPapService> _mockAlunoPapService;
    private readonly Mock<IControleAcessoService> _mockControleAcessoService;
    private readonly Mock<IServicoUsuario> _mockServicoUsuario;
    private readonly Mock<IMediator> _mediator;

    private readonly RepositoriosElastic _repositoriosElastic;
    private readonly RepositoriosSondagem _repositoriosSondagem;

    #endregion

    public QuestionarioSondagemUseCaseBaseTeste()
    {
        _mockRepositorioElasticTurma = new Mock<IRepositorioElasticTurma>();
        _mockRepositorioElasticAluno = new Mock<IRepositorioElasticAluno>();
        _mockRepositorioSondagem = new Mock<IRepositorioSondagem>();
        _mockRepositorioQuestao = new Mock<IRepositorioQuestao>();
        _mockRepositorioRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        _mockRepositorioBimestre = new Mock<IRepositorioBimestre>();
        _mockAlunoPapService = new Mock<IAlunoPapService>();
        _mockControleAcessoService = new Mock<IControleAcessoService>();
        _mockServicoUsuario = new Mock<IServicoUsuario>();
        _mediator = new Mock<IMediator>();

        _repositoriosElastic = new RepositoriosElastic(
            _mockRepositorioElasticTurma.Object,
            _mockRepositorioElasticAluno.Object);

        _repositoriosSondagem = new RepositoriosSondagem(
            _mockRepositorioSondagem.Object,
            _mockRepositorioQuestao.Object,
            _mockRepositorioRespostaAluno.Object,
            _mockRepositorioBimestre.Object);
    }

    private QuestionarioSondagemUseCaseBaseConcreto CriarUseCase(DadosAlunosDto? dadosAlunos = null)
        => new(
            _repositoriosElastic,
            _repositoriosSondagem,
            _mockAlunoPapService.Object,
            _mockControleAcessoService.Object,
            _mockServicoUsuario.Object,
            _mediator.Object,
            dadosAlunos);

    #region Construtor

    [Fact]
    public void Construtor_DeveLancarArgumentNullException_QuandoRepositoriosElasticForNulo()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new QuestionarioSondagemUseCaseBaseConcreto(
                null!,
                _repositoriosSondagem,
                _mockAlunoPapService.Object,
                _mockControleAcessoService.Object,
                _mockServicoUsuario.Object, _mediator.Object,
                null!
                ));
    }

    [Fact]
    public void Construtor_DeveLancarArgumentNullException_QuandoRepositoriosSondagemForNulo()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new QuestionarioSondagemUseCaseBaseConcreto(
                _repositoriosElastic,
                null!,
                _mockAlunoPapService.Object,
                _mockControleAcessoService.Object,
                _mockServicoUsuario.Object,
                _mediator.Object,
                null!
                ));
    }

    [Fact]
    public void Construtor_DeveLancarArgumentNullException_QuandoAlunoPapServiceForNulo()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new QuestionarioSondagemUseCaseBaseConcreto(
                _repositoriosElastic,
                _repositoriosSondagem,
                null!,
                _mockControleAcessoService.Object,
                _mockServicoUsuario.Object, _mediator.Object,
                null!
                ));
    }

    [Fact]
    public void Construtor_DeveLancarArgumentNullException_QuandoControleAcessoServiceForNulo()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new QuestionarioSondagemUseCaseBaseConcreto(
                _repositoriosElastic,
                _repositoriosSondagem,
                _mockAlunoPapService.Object,
                null!,
                _mockServicoUsuario.Object, _mediator.Object,
                null!
                ));
    }

    [Fact]
    public void Construtor_DeveLancarArgumentNullException_QuandoServicoUsuarioForNulo()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new QuestionarioSondagemUseCaseBaseConcreto(
                _repositoriosElastic,
                _repositoriosSondagem,
                _mockAlunoPapService.Object,
                _mockControleAcessoService.Object,
                 _mockServicoUsuario.Object,
                 null!,
                null
                ));
    }

    [Fact]
    public void Construtor_DeveInicializarCorretamente_ComDependenciasValidas()
    {
        var useCase = CriarUseCase();
        Assert.NotNull(useCase);
    }

    #endregion

    #region ExecutarProcessamentoQuestionario

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveLancarArgumentNullException_QuandoFiltroForNulo()
    {
        var useCase = CriarUseCase();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            useCase.ExecutarProcessamentoQuestionario(null!, false, CancellationToken.None));
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveLancarRegraNegocioException_QuandoTurmaNaoForLocalizada()
    {
        _mockRepositorioElasticTurma
            .Setup(x => x.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TurmaElasticDto?)null);

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var useCase = CriarUseCase();

        var ex = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None));

        Assert.Equal(MensagemNegocioComuns.TURMA_NAO_LOCALIZADA, ex.Message);
        Assert.Equal(400, ex.StatusCode);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveLancarRegraNegocioException_QuandoProficienciaIdForZero()
    {
        _mockRepositorioElasticTurma
            .Setup(x => x.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarTurmaElasticDto());

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 0 };
        var useCase = CriarUseCase();

        var ex = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None));

        Assert.Equal(MensagemNegocioComuns.PROFICIENCIA_OBRIGATORIA_NO_FILTRO, ex.Message);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveLancarErroInternoException_QuandoSondagemAtivaForNula()
    {
        _mockRepositorioElasticTurma
            .Setup(x => x.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarTurmaElasticDto());

        _mockRepositorioSondagem
            .Setup(x => x.ObterSondagemAtiva(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dominio.Entidades.Sondagem.Sondagem?)null!);

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var useCase = CriarUseCase();

        var ex = await Assert.ThrowsAsync<ErroInternoException>(() =>
            useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None));

        Assert.Equal(MensagemNegocioComuns.SONDAGEM_ATIVA_NAO_CADASTRADA, ex.Message);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveRetornarQuestionarioSondagemDto_QuandoEhRelatorioForFalse()
    {
        ConfigurarMocksCompletos();

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var useCase = CriarUseCase();

        var resultado = await useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None);

        Assert.IsType<SME.Sondagem.Infra.Dtos.Questionario.QuestionarioSondagemDto>(resultado);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveRetornarQuestionarioSondagemRelatorioDto_QuandoEhRelatorioForTrue()
    {
        ConfigurarMocksCompletos(comBimestreId: 1);

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, BimestreId = 1 };
        var useCase = CriarUseCase();

        var resultado = await useCase.ExecutarProcessamentoQuestionario(filtro, true, CancellationToken.None);

        Assert.IsType<Infrastructure.Dtos.Questionario.Relatorio.QuestionarioSondagemRelatorioDto>(resultado);
    }

    #endregion

    #region ValidarModalidadeEAno

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    public void ValidarModalidadeEAno_DeveLancarErroNaoEncontradoException_QuandoModalidadeForInvalida(int modalidade)
    {
        var ex = Assert.Throws<ErroNaoEncontradoException>(() =>
            QuestionarioSondagemUseCaseBaseConcreto.ValidarModalidadeEAnoPublico(modalidade, 1));

        Assert.Equal(MensagemNegocioComuns.MODALIDADE_SEM_QUESTIONARIO, ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(-1)]
    public void ValidarModalidadeEAno_DeveLancarErroNaoEncontradoException_QuandoAnoForInvalido(int ano)
    {
        var ex = Assert.Throws<ErroNaoEncontradoException>(() =>
            QuestionarioSondagemUseCaseBaseConcreto.ValidarModalidadeEAnoPublico((int)Modalidade.Fundamental, ano));

        Assert.Equal(MensagemNegocioComuns.SERIE_SEM_QUESTIONARIO, ex.Message);
    }

    [Theory]
    [InlineData((int)Modalidade.Fundamental, 1)]
    [InlineData((int)Modalidade.Fundamental, 2)]
    [InlineData((int)Modalidade.Fundamental, 3)]
    [InlineData((int)Modalidade.EJA, 1)]
    [InlineData((int)Modalidade.EJA, 2)]
    [InlineData((int)Modalidade.EJA, 3)]
    public void ValidarModalidadeEAno_NaoDeveLancarExcecao_QuandoModalidadeEAnoForemValidos(int modalidade, int ano)
    {
        var ex = Record.Exception(() =>
            QuestionarioSondagemUseCaseBaseConcreto.ValidarModalidadeEAnoPublico(modalidade, ano));

        Assert.Null(ex);
    }

    #endregion

    #region ObterQuestoesAtivasOuLancarExcecao

    [Fact]
    public async Task ObterQuestoesAtivasOuLancarExcecao_DeveLancarErroNaoEncontradoException_QuandoRetornarNulo()
    {
        _mockRepositorioQuestao
            .Setup(x => x.ObterQuestoesAtivasPorFiltroAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<Dominio.Entidades.Questionario.Questao>?)null!);

        var useCase = CriarUseCase();

        var ex = await Assert.ThrowsAsync<ErroNaoEncontradoException>(() =>
            useCase.ObterQuestoesAtivasOuLancarExcecaoPublico(
                (int)Modalidade.Fundamental, 1, 2024, 1, CancellationToken.None));

        Assert.Equal(MensagemNegocioComuns.QUESTOES_ATIVAS_NAO_ENCONTRADAS, ex.Message);
    }

    [Fact]
    public async Task ObterQuestoesAtivasOuLancarExcecao_DeveLancarErroNaoEncontradoException_QuandoRetornarListaVazia()
    {
        _mockRepositorioQuestao
            .Setup(x => x.ObterQuestoesAtivasPorFiltroAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<Dominio.Entidades.Questionario.Questao>());

        var useCase = CriarUseCase();

        var ex = await Assert.ThrowsAsync<ErroNaoEncontradoException>(() =>
            useCase.ObterQuestoesAtivasOuLancarExcecaoPublico(
                (int)Modalidade.Fundamental, 1, 2024, 1, CancellationToken.None));

        Assert.Equal(MensagemNegocioComuns.QUESTOES_ATIVAS_NAO_ENCONTRADAS, ex.Message);
    }

    [Fact]
    public async Task ObterQuestoesAtivasOuLancarExcecao_DeveRetornarQuestoes_QuandoExistirem()
    {
        var questoes = new List<Dominio.Entidades.Questionario.Questao> { CriarQuestao() };

        _mockRepositorioQuestao
            .Setup(x => x.ObterQuestoesAtivasPorFiltroAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(questoes);

        var useCase = CriarUseCase();

        var resultado = await useCase.ObterQuestoesAtivasOuLancarExcecaoPublico(
            (int)Modalidade.Fundamental, 1, 2024, 1, CancellationToken.None);

        Assert.Single(resultado);
    }

    #endregion

    #region ObterAlunosOuLancarExcecao

    [Fact]
    public async Task ObterAlunosOuLancarExcecao_DeveLancarErroNaoEncontradoException_QuandoRetornarNulo()
    {
        _mockRepositorioElasticAluno
            .Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<AlunoElasticDto>?)null!);

        var useCase = CriarUseCase();

        var ex = await Assert.ThrowsAsync<ErroNaoEncontradoException>(() =>
            useCase.ObterAlunosOuLancarExcecaoPublico(1, 2024, CancellationToken.None));

        Assert.Equal(MensagemNegocioComuns.ALUNOS_NAO_CADASTRADOS_TURMA, ex.Message);
    }

    [Fact]
    public async Task ObterAlunosOuLancarExcecao_DeveLancarErroNaoEncontradoException_QuandoRetornarListaVazia()
    {
        _mockRepositorioElasticAluno
            .Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<AlunoElasticDto>());

        var useCase = CriarUseCase();

        var ex = await Assert.ThrowsAsync<ErroNaoEncontradoException>(() =>
            useCase.ObterAlunosOuLancarExcecaoPublico(1, 2024, CancellationToken.None));

        Assert.Equal(MensagemNegocioComuns.ALUNOS_NAO_CADASTRADOS_TURMA, ex.Message);
    }

    [Fact]
    public async Task ObterAlunosOuLancarExcecao_DeveRetornarAlunos_QuandoExistirem()
    {
        var alunos = CriarAlunosElastic();

        _mockRepositorioElasticAluno
            .Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunos);

        var useCase = CriarUseCase();

        var resultado = await useCase.ObterAlunosOuLancarExcecaoPublico(1, 2024, CancellationToken.None);

        Assert.Equal(2, resultado.Count());
    }

    #endregion

    #region ObterQuestoesIdsPorTipo

    [Fact]
    public void ObterQuestoesIdsPorTipo_DeveRetornarListaDeIds_ComTodosOsIds()
    {
        var questoes = new List<Dominio.Entidades.Questionario.Questao>
        {
            CriarQuestaoComId(10),
            CriarQuestaoComId(20),
            CriarQuestaoComId(30)
        };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto.ObterQuestoesIdsPorTipoPublico(questoes);

        Assert.Equal(3, resultado.Count);
        Assert.Contains(10, resultado);
        Assert.Contains(20, resultado);
        Assert.Contains(30, resultado);
    }

    [Fact]
    public void ObterQuestoesIdsPorTipo_DeveRetornarListaVazia_QuandoNaoHouverQuestoes()
    {
        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ObterQuestoesIdsPorTipoPublico(Enumerable.Empty<Dominio.Entidades.Questionario.Questao>());

        Assert.Empty(resultado);
    }

    #endregion

    #region ObterIdQuestionario

    [Fact]
    public void ObterIdQuestionario_DeveRetornarId_DaPrimeiraQuestao()
    {
        var questao = CriarQuestaoComId(1);
        questao.GetType().GetProperty("QuestionarioId")!.SetValue(questao, 99);

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ObterIdQuestionarioPublico(new List<Dominio.Entidades.Questionario.Questao> { questao });

        Assert.Equal(99, resultado);
    }

    [Fact]
    public void ObterIdQuestionario_DeveRetornarZero_QuandoListaForVazia()
    {
        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ObterIdQuestionarioPublico(Enumerable.Empty<Dominio.Entidades.Questionario.Questao>());

        Assert.Equal(0, resultado);
    }

    #endregion

    #region PossuiQuestaoVinculo

    [Fact]
    public void PossuiQuestaoVinculo_DeveRetornarTrue_QuandoExistirQuestaoComSubpergunta()
    {
        var questao = CriarQuestaoComVinculo(TipoQuestao.QuestaoComSubpergunta);
        var questoes = new List<Dominio.Entidades.Questionario.Questao> { questao };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto.PossuiQuestaoVinculoPublico(questoes);

        Assert.True(resultado);
    }

    [Fact]
    public void PossuiQuestaoVinculo_DeveRetornarFalse_QuandoNaoExistirQuestaoComSubpergunta()
    {
        var questoes = new List<Dominio.Entidades.Questionario.Questao> { CriarQuestao() };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto.PossuiQuestaoVinculoPublico(questoes);

        Assert.False(resultado);
    }

    [Fact]
    public void PossuiQuestaoVinculo_DeveRetornarFalse_QuandoListaForVazia()
    {
        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .PossuiQuestaoVinculoPublico(Enumerable.Empty<Dominio.Entidades.Questionario.Questao>());

        Assert.False(resultado);
    }

    #endregion

    #region ObterOpcoesRespostasPorQuestao

    [Fact]
    public void ObterOpcoesRespostasPorQuestao_DeveRetornarOpcoes_QuandoQuestaoExistir()
    {
        var questao = CriarQuestaoComOpcoes(id: 5);
        var questoes = new List<Dominio.Entidades.Questionario.Questao> { questao };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ObterOpcoesRespostasPorQuestaoPublico(5, questoes);

        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public void ObterOpcoesRespostasPorQuestao_DeveRetornarListaVazia_QuandoQuestaoNaoExistir()
    {
        var questoes = new List<Dominio.Entidades.Questionario.Questao> { CriarQuestaoComId(1) };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ObterOpcoesRespostasPorQuestaoPublico(999, questoes);

        Assert.Empty(resultado);
    }

    [Fact]
    public void ObterOpcoesRespostasPorQuestao_DeveOrdenarPorOrdem()
    {
        var questao = CriarQuestaoComOpcoesDesordenadas(id: 1);
        var questoes = new List<Dominio.Entidades.Questionario.Questao> { questao };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ObterOpcoesRespostasPorQuestaoPublico(1, questoes);

        Assert.Equal(1, resultado[0].Ordem);
        Assert.Equal(2, resultado[1].Ordem);
    }

    #endregion

    #region ObterTituloTabelaRespostas

    [Fact]
    public void ObterTituloTabelaRespostas_DeveRetornarNomeDaQuestao_QuandoNaoHouverSubpergunta()
    {
        var questoes = new List<Dominio.Entidades.Questionario.Questao> { CriarQuestaoComId(1, nome: "Título da Questão") };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ObterTituloTabelaRespostasPublico(questoes);

        Assert.Equal("Título da Questão", resultado);
    }

    [Fact]
    public void ObterTituloTabelaRespostas_DeveRetornarNomeDaQuestaoVinculo_QuandoHouverSubpergunta()
    {
        var questao = CriarQuestaoComVinculo(TipoQuestao.QuestaoComSubpergunta, nomeVinculo: "Título Subpergunta");
        var questoes = new List<Dominio.Entidades.Questionario.Questao> { questao };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ObterTituloTabelaRespostasPublico(questoes);

        Assert.Equal("Título Subpergunta", resultado);
    }

    [Fact]
    public void ObterTituloTabelaRespostas_DeveRetornarStringVazia_QuandoListaForVazia()
    {
        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ObterTituloTabelaRespostasPublico(Enumerable.Empty<Dominio.Entidades.Questionario.Questao>());

        Assert.Equal(string.Empty, resultado);
    }

    #endregion

    #region ConstruirResposta

    [Fact]
    public void ConstruirResposta_DeveRetornarRespostaVazia_QuandoNaoPossuirResposta()
    {
        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ConstruirRespostaPublico(false, null);

        Assert.NotNull(resultado);
        Assert.Equal(0, resultado.Id);
        Assert.Null(resultado.OpcaoRespostaId);
    }

    [Fact]
    public void ConstruirResposta_DeveRetornarRespostaVazia_QuandoRespostaForNula()
    {
        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ConstruirRespostaPublico(true, null);

        Assert.NotNull(resultado);
        Assert.Equal(0, resultado.Id);
        Assert.Null(resultado.OpcaoRespostaId);
    }

    [Fact]
    public void ConstruirResposta_DeveRetornarResposta_QuandoPossuirRespostaComOpcao()
    {
        var resposta = CriarRespostaAluno(id: 10, opcaoRespostaId: 5);

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ConstruirRespostaPublico(true, resposta);

        Assert.Equal(10, resultado.Id);
        Assert.Equal(5, resultado.OpcaoRespostaId);
    }

    [Fact]
    public void ConstruirResposta_DeveRetornarOpcaoRespostaIdNulo_QuandoOpcaoRespostaIdForZero()
    {
        var resposta = CriarRespostaAluno(id: 10, opcaoRespostaId: 0);

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ConstruirRespostaPublico(true, resposta);

        Assert.Equal(10, resultado.Id);
        Assert.Null(resultado.OpcaoRespostaId);
    }

    #endregion

    #region ConstruirColunaAluno

    [Fact]
    public void ConstruirColunaAluno_DeveConstruirColunaCorreta_SemResposta()
    {
        var colunaBase = CriarColunaQuestionario(idCiclo: 1);
        var aluno = CriarAlunoElastic(codigo: 100);
        var sondagem = CriarSondagem();
        var respostas = new Dictionary<(int, int?, long), RespostaAluno>();
        var useCase = CriarUseCase();

        var resultado = QuestionarioSondagemUseCaseBaseConcreto.ConstruirColunaAlunoPublico(colunaBase, aluno, sondagem, 1L, respostas);

        Assert.Equal(1, resultado.IdCiclo);
        Assert.NotNull(resultado.Resposta);
        Assert.Equal(0, resultado.Resposta.Id);
    }

    [Fact]
    public void ConstruirColunaAluno_DeveSetarPeriodoBimestreAtivo_QuandoAlunoEstaAtivo()
    {
        var periodoAtivo = CriarPeriodoBimestre(dataInicio: DateTime.Now.AddDays(-1), dataFim: DateTime.Now.AddDays(1));
        var sondagem = CriarSondagemComPeriodo(periodoAtivo);
        var colunaBase = CriarColunaQuestionario(idCiclo: 1, periodoAtivo: false);
        var aluno = CriarAlunoElasticAtivo(dataSituacao: DateTime.Now, situacao: (int)SituacaoMatriculaAluno.Ativo);
        var respostas = new Dictionary<(int, int?, long), RespostaAluno>();
        var useCase = CriarUseCase();

        var resultado = QuestionarioSondagemUseCaseBaseConcreto.ConstruirColunaAlunoPublico(colunaBase, aluno, sondagem, 1L, respostas);

        Assert.True(resultado.PeriodoBimestreAtivo);
    }

    [Fact]
    public void ConstruirColunaAluno_DeveFiltrarOpcaoResposta_QuandoEhRelatorioForTrue()
    {
        var opcaoRespostaId = 7;
        var colunaBase = CriarColunaQuestionario(idCiclo: 1, opcaoRespostaId: opcaoRespostaId);
        var aluno = CriarAlunoElastic(codigo: 100);
        var sondagem = CriarSondagem();
        var resposta = CriarRespostaAluno(id: 1, opcaoRespostaId: opcaoRespostaId);
        var respostas = new Dictionary<(int, int?, long), RespostaAluno>
    {
        { (100, 1, 1L), resposta }
    };
        var useCase = CriarUseCase();

        var resultado = QuestionarioSondagemUseCaseBaseConcreto.ConstruirColunaAlunoPublico(colunaBase, aluno, sondagem, 1L, respostas, ehRelatorio: true);

        Assert.NotNull(resultado.OpcaoResposta);
        Assert.Single(resultado.OpcaoResposta!);
        Assert.Equal(opcaoRespostaId, resultado.OpcaoResposta!.First().Id);
    }

    [Fact]
    public void ConstruirColunaAluno_NaoDeveFiltrarOpcaoResposta_QuandoEhRelatorioForFalse()
    {
        var colunaBase = CriarColunaQuestionario(idCiclo: 1, incluirDuasOpcoes: true);
        var aluno = CriarAlunoElastic(codigo: 100);
        var sondagem = CriarSondagem();
        var respostas = new Dictionary<(int, int?, long), RespostaAluno>();
        var useCase = CriarUseCase();

        var resultado = QuestionarioSondagemUseCaseBaseConcreto.ConstruirColunaAlunoPublico(colunaBase, aluno, sondagem, 1L, respostas, ehRelatorio: false);

        Assert.Equal(2, resultado.OpcaoResposta!.Count());
    }

    [Fact]
    public void ConstruirColunaAluno_DeveUsarQuestaoSubrespostaId_QuandoDefinido()
    {
        var colunaBase = CriarColunaQuestionario(idCiclo: 1, questaoSubrespostaId: 55);
        var aluno = CriarAlunoElastic(codigo: 200);
        var sondagem = CriarSondagem();
        var resposta = CriarRespostaAluno(id: 99, opcaoRespostaId: 3);
        var respostas = new Dictionary<(int, int?, long), RespostaAluno>
        {
            { (200, 1, 55L), resposta }
        };
        var useCase = CriarUseCase();

        var resultado = QuestionarioSondagemUseCaseBaseConcreto.ConstruirColunaAlunoPublico(colunaBase, aluno, sondagem, 9L, respostas);

        Assert.Equal(99, resultado.Resposta.Id);
    }

    [Fact]
    public void ConstruirColunaAluno_DeveTratarIdCicloZeroComoBimestreIdNulo()
    {
        var colunaBase = CriarColunaQuestionario(idCiclo: 0);
        var aluno = CriarAlunoElastic(codigo: 300);
        var sondagem = CriarSondagem();
        var resposta = CriarRespostaAluno(id: 50, opcaoRespostaId: 2);
        var respostas = new Dictionary<(int, int?, long), RespostaAluno>
        {
            { (300, null, 1L), resposta }
        };
        var useCase = CriarUseCase();

        var resultado = QuestionarioSondagemUseCaseBaseConcreto.ConstruirColunaAlunoPublico(colunaBase, aluno, sondagem, 1L, respostas);

        Assert.Equal(50, resultado.Resposta.Id);
    }

    #endregion

    #region ProcessarRespostas

    [Fact]
    public void ProcessarRespostas_DeveRetornarRespostasVazias_QuandoNaoHouverRespostas()
    {
        var alunosAtivos = CriarAlunosElastic();
        var respostas = new Dictionary<(long, int?, long), RespostaAluno>();

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ProcessarRespostasPublico(respostas, null!, alunosAtivos, DateTime.Now.AddDays(-30));

        Assert.Null(resultado.InseridoPor);
        Assert.Null(resultado.AlteradoPor);
        Assert.Empty(resultado.RespostasConvertidas);
    }

    [Fact]
    public void ProcessarRespostas_DevePreencherInseridoPor_QuandoHouverRespostaCriada()
    {
        var aluno = CriarAlunoElastic(codigo: 1001, dataSituacao: DateTime.Now.AddDays(-60));
        var alunosAtivos = new List<AlunoElasticDto> { aluno };

        var resposta = CriarRespostaAluno(id: 1, opcaoRespostaId: 2, alunoId: 1001);
        DefinirAuditoria(resposta,
            criadoPor: "Professor Silva",
            criadoRf: "12345",
            criadoEm: new DateTime(2024, 3, 10, 9, 0, 0));

        var respostas = new Dictionary<(long, int?, long), RespostaAluno>
        {
            { (1001L, 1, 1L), resposta }
        };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ProcessarRespostasPublico(respostas, null!, alunosAtivos, DateTime.Now.AddDays(-30));

        Assert.NotNull(resultado.InseridoPor);
        Assert.Contains("Professor Silva", resultado.InseridoPor);
        Assert.Contains("12345", resultado.InseridoPor);
    }

    [Fact]
    public void ProcessarRespostas_DevePreencherAlteradoPor_QuandoHouverRespostaAlterada()
    {
        var aluno = CriarAlunoElastic(codigo: 1001, dataSituacao: DateTime.Now.AddDays(-60));
        var alunosAtivos = new List<AlunoElasticDto> { aluno };

        var resposta = CriarRespostaAluno(id: 1, opcaoRespostaId: 2, alunoId: 1001);
        DefinirAuditoria(resposta,
            criadoPor: "Professor",
            criadoRf: "111",
            criadoEm: new DateTime(2024, 3, 10, 9, 0, 0),
            alteradoPor: "Coordenador",
            alteradoRf: "999",
            alteradoEm: new DateTime(2024, 4, 1, 10, 0, 0));

        var respostas = new Dictionary<(long, int?, long), RespostaAluno>
        {
            { (1001L, 1, 1L), resposta }
        };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ProcessarRespostasPublico(respostas, null!, alunosAtivos, DateTime.Now.AddDays(-30));

        Assert.NotNull(resultado.AlteradoPor);
        Assert.Contains("Coordenador", resultado.AlteradoPor);
        Assert.Contains("999", resultado.AlteradoPor);
    }

    [Fact]
    public void ProcessarRespostas_DeveExcluirRespostasDeAlunosInativos()
    {
        var alunoAtivo = CriarAlunoElastic(codigo: 1001, dataSituacao: DateTime.Now.AddDays(-60));
        var alunoInativo = CriarAlunoElastic(codigo: 1002, dataSituacao: DateTime.Now.AddDays(1));
        var alunosAtivos = new List<AlunoElasticDto> { alunoAtivo, alunoInativo };

        var respostaAtivo = CriarRespostaAluno(id: 1, opcaoRespostaId: 2, alunoId: 1001);
        var respostaInativo = CriarRespostaAluno(id: 2, opcaoRespostaId: 3, alunoId: 1002);

        var respostas = new Dictionary<(long, int?, long), RespostaAluno>
        {
            { (1001L, 1, 1L), respostaAtivo },
            { (1002L, 1, 1L), respostaInativo }
        };

        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ProcessarRespostasPublico(respostas, null!, alunosAtivos, DateTime.Now.AddDays(-30));

        Assert.DoesNotContain(resultado.RespostasConvertidas.Keys, k => k.CodigoAluno == 1002);
    }

    [Fact]
    public void ProcessarRespostas_DeveExcluiRespostasDeQuestaoLinguaPortuguesa_SemOpcaoRespostaId()
    {
        var aluno = CriarAlunoElastic(codigo: 1001, dataSituacao: DateTime.Now.AddDays(-60));
        var alunosAtivos = new List<AlunoElasticDto> { aluno };

        var respostaSemOpcao = CriarRespostaAluno(id: 1, opcaoRespostaId: null, alunoId: 1001);
        var respostaComOpcao = CriarRespostaAluno(id: 2, opcaoRespostaId: 1, alunoId: 1001);

        var respostas = new Dictionary<(long, int?, long), RespostaAluno>
        {
            { (1001L, 1, 1L), respostaSemOpcao },
            { (1001L, 1, 2L), respostaComOpcao }
        };

        var questaoLingua = CriarQuestaoComId(99, tipo: TipoQuestao.LinguaPortuguesaSegundaLingua);
        var resultado = QuestionarioSondagemUseCaseBaseConcreto
            .ProcessarRespostasPublico(respostas, questaoLingua, alunosAtivos, DateTime.Now.AddDays(-30));

        Assert.Single(resultado.RespostasConvertidas);
        Assert.Contains(resultado.RespostasConvertidas.Keys, k => k.QuestaoId == 2);
    }

    #endregion

    #region ObterColunasOuLancarExcecao - por bimestres

    [Fact]
    public async Task ObterColunasOuLancarExcecao_DeveRetornarColunasPorBimestre_QuandoNaoHouverSubpergunta()
    {
        var periodos = new List<SondagemPeriodoBimestre>
        {
            CriarPeriodoBimestre(bimestreId: 1, descricao: "1º Bimestre")
        };
        var questoes = new List<Dominio.Entidades.Questionario.Questao> { CriarQuestaoComOpcoes(id: 1) };

        var resultado = await QuestionarioSondagemUseCaseBaseConcreto
            .ObterColunasOuLancarExcecaoPublico(periodos, questoes, null);

        Assert.Single(resultado);
        Assert.Equal("1º Bimestre", resultado[0].DescricaoColuna);
        Assert.Equal(1, resultado[0].IdCiclo);
    }

    [Fact]
    public async Task ObterColunasOuLancarExcecao_DeveLancarErroNaoEncontradoException_QuandoBimestresForemExcluidos()
    {
        var periodoExcluido = CriarPeriodoBimestre(bimestreId: 1, descricao: "1º Bimestre");
        periodoExcluido.Excluido = true;

        var periodos = new List<SondagemPeriodoBimestre> { periodoExcluido };
        var questoes = new List<Dominio.Entidades.Questionario.Questao>
        {
            CriarQuestaoComId(1, tipo: TipoQuestao.Combo)
        };

        await Assert.ThrowsAsync<ErroNaoEncontradoException>(() =>
            QuestionarioSondagemUseCaseBaseConcreto
                .ObterColunasOuLancarExcecaoPublico(periodos, questoes, null));
    }

    [Fact]
    public async Task ObterColunasOuLancarExcecao_DeveRetornarColunasPorSubperguntas_QuandoHouverVinculo()
    {
        var periodos = new List<SondagemPeriodoBimestre>
        {
            CriarPeriodoBimestre(bimestreId: 1)
        };
        var questoes = new List<Dominio.Entidades.Questionario.Questao>
        {
            CriarQuestaoComVinculo(TipoQuestao.QuestaoComSubpergunta, id: 10, nome: "Subquestão 1"),
        };

        var resultado = await QuestionarioSondagemUseCaseBaseConcreto
            .ObterColunasOuLancarExcecaoPublico(periodos, questoes, null);

        Assert.NotEmpty(resultado);
    }

    [Fact]
    public async Task ObterColunasOuLancarExcecao_DeveRetornarColunasPorSubperguntas_QuandoBimestreIdForDefinido()
    {
        var periodos = new List<SondagemPeriodoBimestre>
        {
            CriarPeriodoBimestre(bimestreId: 2)
        };
        var questoes = new List<Dominio.Entidades.Questionario.Questao>
        {
            CriarQuestaoComVinculo(TipoQuestao.QuestaoComSubpergunta, id: 20, nome: "Sub 1"),
        };

        var resultado = await QuestionarioSondagemUseCaseBaseConcreto
            .ObterColunasOuLancarExcecaoPublico(periodos, questoes, bimestreId: 2);

        Assert.NotEmpty(resultado);
    }

    [Fact]
    public async Task ObterColunasOuLancarExcecao_DeveLancarErroNaoEncontradoException_QuandoPeriodoBimestreNaoEncontrado()
    {
        var periodos = new List<SondagemPeriodoBimestre>
        {
            CriarPeriodoBimestre(bimestreId: 1)
        };
        var questoes = new List<Dominio.Entidades.Questionario.Questao>
        {
            CriarQuestaoComVinculo(TipoQuestao.QuestaoComSubpergunta, id: 30, nome: "Sub"),
        };

        await Assert.ThrowsAsync<ErroNaoEncontradoException>(() =>
            QuestionarioSondagemUseCaseBaseConcreto
                .ObterColunasOuLancarExcecaoPublico(periodos, questoes, bimestreId: 999));
    }

    #endregion

    #region ExecutarProcessamentoQuestionario - Fluxo completo

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveRetornarQuestionarioComEstudantesOrdenados()
    {
        ConfigurarMocksCompletos();
        _mockControleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var useCase = CriarUseCase();

        var resultado = await useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None);

        var dto = Assert.IsType<SME.Sondagem.Infra.Dtos.Questionario.QuestionarioSondagemDto>(resultado);
        Assert.NotNull(dto.Estudantes);
        Assert.True(dto.PodeSalvar);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DevePopularSemestreNoRelatorio()
    {
        ConfigurarMocksCompletos(comBimestreId: 1, semestre: 2);

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, BimestreId = 1 };
        var useCase = CriarUseCase();

        var resultado = await useCase.ExecutarProcessamentoQuestionario(filtro, true, CancellationToken.None);

        var dto = Assert.IsType<Infrastructure.Dtos.Questionario.Relatorio.QuestionarioSondagemRelatorioDto>(resultado);
        Assert.Contains("semestre", dto.Semestre, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveUsarAnoLetivoDeFiltro_QuandoDefinido()
    {
        ConfigurarMocksCompletos(anoLetivo: 2025);

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, AnoLetivo = 2025 };
        var useCase = CriarUseCase();

        await useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None);

        _mockRepositorioQuestao.Verify(x =>
            x.ObterQuestoesAtivasPorFiltroAsync(
                It.IsAny<int>(),
                2025,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveUsarAnoLetivoDaTurma_QuandoFiltroForZero()
    {
        ConfigurarMocksCompletos(anoLetivo: 2024);

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, AnoLetivo = 0 };
        var useCase = CriarUseCase();

        await useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None);

        _mockRepositorioQuestao.Verify(x =>
            x.ObterQuestoesAtivasPorFiltroAsync(
                It.IsAny<int>(),
                2024,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DevePreencherInseridoPorEAlteradoPor()
    {
        ConfigurarMocksCompletos(comRespostaAuditoria: true);

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var useCase = CriarUseCase();

        var resultado = await useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None);

        var dto = Assert.IsType<SME.Sondagem.Infra.Dtos.Questionario.QuestionarioSondagemDto>(resultado);
        Assert.NotNull(dto.InseridoPor);
        Assert.NotNull(dto.AlteradoPor);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveUsarBimestresDoRepositorio_QuandoExistirem()
    {
        ConfigurarMocksCompletos();

        var bimestreEspecial = CriarPeriodoBimestre(bimestreId: 77, descricao: "Bimestre Especial");
        _mockRepositorioBimestre
            .Setup(x => x.ObterBimestresPorQuestionarioIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SondagemPeriodoBimestre> { bimestreEspecial });

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var useCase = CriarUseCase();

        var resultado = await useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None);

        var dto = Assert.IsType<SME.Sondagem.Infra.Dtos.Questionario.QuestionarioSondagemDto>(resultado);
        var coluna = dto.Estudantes!.FirstOrDefault()?.Coluna?.FirstOrDefault();
        Assert.NotNull(coluna);
        Assert.Equal("Bimestre Especial", coluna!.DescricaoColuna);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveUsarBimestresDaSondagem_QuandoRepositorioRetornarVazio()
    {
        ConfigurarMocksCompletos();

        _mockRepositorioBimestre
            .Setup(x => x.ObterBimestresPorQuestionarioIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SondagemPeriodoBimestre>());

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var useCase = CriarUseCase();

        var resultado = await useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None);

        Assert.IsType<SME.Sondagem.Infra.Dtos.Questionario.QuestionarioSondagemDto>(resultado);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveRetornarLegenda_NoRelatorio()
    {
        ConfigurarMocksCompletos(comBimestreId: 1);

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1, BimestreId = 1 };
        var useCase = CriarUseCase();

        var resultado = await useCase.ExecutarProcessamentoQuestionario(filtro, true, CancellationToken.None);

        var dto = Assert.IsType<Infrastructure.Dtos.Questionario.Relatorio.QuestionarioSondagemRelatorioDto>(resultado);
        Assert.NotNull(dto.Legenda);
    }

    [Fact]
    public async Task ExecutarProcessamentoQuestionario_DeveRetornarQuestaoId_NoDto()
    {
        ConfigurarMocksCompletos();

        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var useCase = CriarUseCase();

        var resultado = await useCase.ExecutarProcessamentoQuestionario(filtro, false, CancellationToken.None);

        var dto = Assert.IsType<SME.Sondagem.Infra.Dtos.Questionario.QuestionarioSondagemDto>(resultado);
        Assert.True(dto.QuestaoId >= 0);
        Assert.True(dto.SondagemId > 0);
    }

    #endregion

    #region Helpers privados

    private void ConfigurarMocksCompletos(
        int? comBimestreId = null,
        int semestre = 1,
        int anoLetivo = 2024,
        bool comRespostaAuditoria = false)
    {
        var turma = CriarTurmaElasticDto(anoLetivo: anoLetivo, semestre: semestre);
        var sondagem = CriarSondagem();
        var questoes = new List<Dominio.Entidades.Questionario.Questao> { CriarQuestaoComOpcoes(id: 1) };
        var alunos = CriarAlunosElastic();

        _mockRepositorioElasticTurma
            .Setup(x => x.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(turma);

        _mockRepositorioSondagem
            .Setup(x => x.ObterSondagemAtiva(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sondagem);

        _mockRepositorioQuestao
            .Setup(x => x.ObterQuestoesAtivasPorFiltroAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(questoes);

        _mockRepositorioElasticAluno
            .Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunos);

        _mockRepositorioBimestre
            .Setup(x => x.ObterBimestresPorQuestionarioIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ICollection<SondagemPeriodoBimestre>?)null!);

        _mockControleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Dictionary<(long, long, int?), RespostaAluno> respostasDict;

        if (comRespostaAuditoria)
        {
            var r = CriarRespostaAluno(id: 1, opcaoRespostaId: 2, alunoId: 1001);
            DefinirAuditoria(r, "Prof", "111", DateTime.Now.AddDays(-5),
                alteradoPor: "Coord", alteradoRf: "222", alteradoEm: DateTime.Now.AddDays(-1));
            respostasDict = new Dictionary<(long, long, int?), RespostaAluno>
            {
                { (1001L, 1L, 1), r }
            };
        }
        else
        {
            respostasDict = new Dictionary<(long, long, int?), RespostaAluno>();
        }

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasAlunosPorQuestoesAsync(
                It.IsAny<List<long>>(), It.IsAny<List<long>>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostasDict);
    }

    private static TurmaElasticDto CriarTurmaElasticDto(
        int anoLetivo = 2024,
        int modalidade = (int)Modalidade.Fundamental,
        string anoTurma = "1",
        int codigoTurma = 1001,
        int semestre = 1)
    {
        var turma = new TurmaElasticDto
        {
            Modalidade = modalidade,
            AnoTurma = anoTurma,
            AnoLetivo = anoLetivo,
            CodigoTurma = codigoTurma
        };

        turma.GetType().GetProperty("Semestre")?.SetValue(turma, semestre);
        return turma;
    }

    /// <summary>
    /// Cria uma sondagem com um período de bimestre ativo atribuído via reflexão.
    /// A atribuição de PeriodosBimestre é obrigatória para que ObterColunasPorBimestres
    /// não receba lista vazia e lance ErroNaoEncontradoException.
    /// </summary>
    private static Dominio.Entidades.Sondagem.Sondagem CriarSondagem()
    {
        var sondagem = new Dominio.Entidades.Sondagem.Sondagem("Sondagem Teste", DateTime.Now.AddMonths(-1));
        sondagem.GetType().GetProperty("Id")?.SetValue(sondagem, 1);

        var periodo = CriarPeriodoBimestre(bimestreId: 1, descricao: "1º Bimestre");
        var listaPeriodos = new List<SondagemPeriodoBimestre> { periodo };

        sondagem.GetType()
            .GetProperty("PeriodosBimestre")!
            .SetValue(sondagem, listaPeriodos);

        return sondagem;
    }

    private static Dominio.Entidades.Sondagem.Sondagem CriarSondagemComPeriodo(SondagemPeriodoBimestre periodo)
    {
        var sondagem = new Dominio.Entidades.Sondagem.Sondagem("Sondagem", DateTime.Now);
        sondagem.GetType().GetProperty("Id")?.SetValue(sondagem, 1);

        var lista = new List<SondagemPeriodoBimestre> { periodo };
        sondagem.GetType()
            .GetProperty("PeriodosBimestre")!
            .SetValue(sondagem, lista);

        return sondagem;
    }

    private static SondagemPeriodoBimestre CriarPeriodoBimestre(
        int bimestreId = 1,
        string descricao = "1º Bimestre",
        DateTime? dataInicio = null,
        DateTime? dataFim = null)
    {
        var inicio = dataInicio ?? DateTime.Now.AddDays(-10);
        var fim = dataFim ?? DateTime.Now.AddDays(10);

        var periodo = new SondagemPeriodoBimestre(1, bimestreId, inicio, fim);

        var bimestre = new Dominio.Entidades.Bimestre(bimestreId, descricao);
        periodo.GetType().GetProperty("Bimestre")?.SetValue(periodo, bimestre);

        return periodo;
    }

    private static Dominio.Entidades.Questionario.Questao CriarQuestao(
        int id = 1,
        string nome = "Questão Teste",
        TipoQuestao tipo = TipoQuestao.Combo)
    {
        var questao = new Dominio.Entidades.Questionario.Questao(1, 1, nome, "", true, tipo, "", false, 1);
        questao.GetType().BaseType?.GetProperty("Id")?.SetValue(questao, id);
        return questao;
    }

    private static Dominio.Entidades.Questionario.Questao CriarQuestaoComId(
        int id = 1,
        string nome = "Questão",
        TipoQuestao tipo = TipoQuestao.Combo)
    {
        var questao = new Dominio.Entidades.Questionario.Questao(1, 1, nome, "", true, tipo, "", false, 1);
        questao.GetType().BaseType?.GetProperty("Id")?.SetValue(questao, id);
        return questao;
    }

    private static Dominio.Entidades.Questionario.Questao CriarQuestaoComOpcoes(int id = 1)
    {
        var questao = CriarQuestaoComId(id);

        var opcao1 = new Dominio.Entidades.Questionario.OpcaoResposta(1, "Opção 1", "A", "#FFF", "#000");
        var opcao2 = new Dominio.Entidades.Questionario.OpcaoResposta(2, "Opção 2", "B", "#FFF", "#000");

        opcao1.GetType().BaseType?.GetProperty("Id")?.SetValue(opcao1, 1);
        opcao2.GetType().BaseType?.GetProperty("Id")?.SetValue(opcao2, 2);

        var qo1 = new QuestaoOpcaoResposta(id, 1, 1);
        var qo2 = new QuestaoOpcaoResposta(id, 2, 2);

        qo1.GetType().GetProperty("OpcaoResposta")?.SetValue(qo1, opcao1);
        qo2.GetType().GetProperty("OpcaoResposta")?.SetValue(qo2, opcao2);

        var opcoes = new List<QuestaoOpcaoResposta> { qo1, qo2 };
        questao.GetType().GetProperty("QuestaoOpcoes")?.SetValue(questao, opcoes);

        return questao;
    }

    private static Dominio.Entidades.Questionario.Questao CriarQuestaoComOpcoesDesordenadas(int id = 1)
    {
        var questao = CriarQuestaoComId(id);

        var opcao1 = new Dominio.Entidades.Questionario.OpcaoResposta(1, "Primeira", "A", null, null);
        var opcao2 = new Dominio.Entidades.Questionario.OpcaoResposta(2, "Segunda", "B", null, null);

        var qo2 = new QuestaoOpcaoResposta(id, 2, 2);
        var qo1 = new QuestaoOpcaoResposta(id, 1, 1);

        qo1.GetType().GetProperty("OpcaoResposta")?.SetValue(qo1, opcao1);
        qo2.GetType().GetProperty("OpcaoResposta")?.SetValue(qo2, opcao2);

        var opcoes = new List<QuestaoOpcaoResposta> { qo2, qo1 };
        questao.GetType().GetProperty("QuestaoOpcoes")?.SetValue(questao, opcoes);

        return questao;
    }

    private static Dominio.Entidades.Questionario.Questao CriarQuestaoComVinculo(
        TipoQuestao tipoVinculo,
        int id = 1,
        string nome = "Subquestão",
        string nomeVinculo = "Questão Pai")
    {
        var questao = CriarQuestaoComId(id, nome);
        var questaoVinculo = CriarQuestaoComId(99, nomeVinculo, tipoVinculo);

        questao.GetType().GetProperty("QuestaoVinculo")?.SetValue(questao, questaoVinculo);
        return questao;
    }

    private static List<AlunoElasticDto> CriarAlunosElastic() =>
    [
        CriarAlunoElastic(codigo: 1001, nome: "Ana Lima", numero: "01"),
        CriarAlunoElastic(codigo: 1002, nome: "Bruno Costa", numero: "02")
    ];

    private static AlunoElasticDto CriarAlunoElastic(
        int codigo = 1001,
        string nome = "Aluno Teste",
        string numero = "01",
        DateTime? dataSituacao = null,
        int situacao = (int)SituacaoMatriculaAluno.Ativo)
    {
        return new AlunoElasticDto
        {
            CodigoAluno = codigo,
            NomeAluno = nome,
            NumeroAlunoChamada = numero,
            DataSituacao = dataSituacao ?? DateTime.Now.AddDays(-60),
            CodigoSituacaoMatricula = situacao
        };
    }

    private static AlunoElasticDto CriarAlunoElasticAtivo(DateTime dataSituacao, int situacao)
    {
        return new AlunoElasticDto
        {
            CodigoAluno = 500,
            NomeAluno = "Ativo",
            DataSituacao = dataSituacao,
            CodigoSituacaoMatricula = situacao
        };
    }

    private static RespostaAluno CriarRespostaAluno(
        int id,
        int? opcaoRespostaId,
        int alunoId = 1001,
        int questaoId = 1,
        int bimestreId = 1)
    {
        var resposta = new RespostaAluno(1, alunoId, questaoId, opcaoRespostaId, DateTime.Now, bimestreId);
        resposta.GetType().BaseType?.GetProperty("Id")?.SetValue(resposta, id);
        return resposta;
    }

    private static void DefinirAuditoria(
        RespostaAluno resposta,
        string criadoPor,
        string criadoRf,
        DateTime criadoEm,
        string? alteradoPor = null,
        string? alteradoRf = null,
        DateTime? alteradoEm = null)
    {
        var tipo = resposta.GetType().BaseType!;
        tipo.GetProperty("CriadoPor")?.SetValue(resposta, criadoPor);
        tipo.GetProperty("CriadoRF")?.SetValue(resposta, criadoRf);
        tipo.GetProperty("CriadoEm")?.SetValue(resposta, criadoEm);

        if (alteradoPor != null)
        {
            tipo.GetProperty("AlteradoPor")?.SetValue(resposta, alteradoPor);
            tipo.GetProperty("AlteradoRF")?.SetValue(resposta, alteradoRf);
            tipo.GetProperty("AlteradoEm")?.SetValue(resposta, alteradoEm);
        }
    }

    private static ColunaQuestionarioDto CriarColunaQuestionario(
        int idCiclo = 1,
        int? questaoSubrespostaId = null,
        bool periodoAtivo = false,
        int? opcaoRespostaId = null,
        bool incluirDuasOpcoes = false)
    {
        List<OpcaoRespostaDto> opcoes;

        if (incluirDuasOpcoes)
        {
            opcoes = [
                new OpcaoRespostaDto { Id = 1, Ordem = 1, DescricaoOpcaoResposta = "Op1" },
                new OpcaoRespostaDto { Id = 2, Ordem = 2, DescricaoOpcaoResposta = "Op2" }
            ];
        }
        else if (opcaoRespostaId.HasValue)
        {
            opcoes = [
                new OpcaoRespostaDto { Id = opcaoRespostaId.Value, Ordem = 1, DescricaoOpcaoResposta = "Op" }
            ];
        }
        else
        {
            opcoes = [new OpcaoRespostaDto { Id = 1, Ordem = 1, DescricaoOpcaoResposta = "Op" }];

        }

        return new ColunaQuestionarioDto
        {
            IdCiclo = idCiclo,
            DescricaoColuna = "Coluna Teste",
            PeriodoBimestreAtivo = periodoAtivo,
            QuestaoSubrespostaId = questaoSubrespostaId,
            OpcaoResposta = opcoes
        };
    }

    #endregion
}

internal static class QuestionarioSondagemUseCaseBaseExtensions
{
    public static void ValidarModalidadeEAnoPublico(this QuestionarioSondagemUseCaseBaseConcreto _, int modalidade, int ano)
        => QuestionarioSondagemUseCaseBaseConcreto.ValidarModalidadeEAnoPublico(modalidade, ano);
}

internal partial class QuestionarioSondagemUseCaseBaseConcreto
{
    public static void ValidarModalidadeEAnoPublico(int modalidade, int ano)
        => ValidarModalidadeEAno(modalidade, ano);

    public Task<IEnumerable<Dominio.Entidades.Questionario.Questao>> ObterQuestoesAtivasOuLancarExcecaoPublico(
        int modalidade, int ano, int anoLetivo, int proficienciaId, CancellationToken cancellationToken)
        => ObterQuestoesAtivasOuLancarExcecao(modalidade, ano, anoLetivo, proficienciaId, cancellationToken);

    public Task<IEnumerable<AlunoElasticDto>> ObterAlunosOuLancarExcecaoPublico(
        int turmaId, int anoLetivo, CancellationToken cancellationToken)
        => ObterAlunosOuLancarExcecao(turmaId, anoLetivo, cancellationToken);

    public static List<int> ObterQuestoesIdsPorTipoPublico(IEnumerable<Dominio.Entidades.Questionario.Questao> questoes)
        => ObterQuestoesIdsPorTipo(questoes);

    public static int ObterIdQuestionarioPublico(IEnumerable<Dominio.Entidades.Questionario.Questao> questoes)
        => ObterIdQuestionario(questoes);

    public static bool PossuiQuestaoVinculoPublico(IEnumerable<Dominio.Entidades.Questionario.Questao> questoes)
        => PossuiQuestaoVinculo(questoes);

    public static List<OpcaoRespostaDto> ObterOpcoesRespostasPorQuestaoPublico(
        int questaoId, IEnumerable<Dominio.Entidades.Questionario.Questao> questoes)
        => ObterOpcoesRespostasPorQuestao(questaoId, questoes);

    public static string ObterTituloTabelaRespostasPublico(IEnumerable<Dominio.Entidades.Questionario.Questao> questoes)
        => ObterTituloTabelaRespostas(questoes);

    public static RespostaDto ConstruirRespostaPublico(bool possuiResposta, RespostaAluno? resposta)
        => ConstruirResposta(possuiResposta, resposta);

    private static ColunaQuestionarioDto ConstruirColunaAlunoPublicoInterno(
    ColunaQuestionarioDto colunaBase,
    AlunoElasticDto aluno,
    Dominio.Entidades.Sondagem.Sondagem sondagemAtiva,
    long questaoIdPrincipal,
    Dictionary<(int CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> respostas,
    bool ehRelatorio = false)
    {
        var contexto = new ContextoColunaDto(
            sondagemAtiva,
            questaoIdPrincipal,
            false,
            respostas,
            new Dictionary<int, string>(),
            ehRelatorio
        );
        var filtro = new FiltroQuestionario { Modalidade = (int)Modalidade.Fundamental };
        return ConstruirColunaAluno(colunaBase, aluno, contexto, filtro);
    }

    public static ColunaQuestionarioDto ConstruirColunaAlunoPublico(
        ColunaQuestionarioDto colunaBase,
        AlunoElasticDto aluno,
        Dominio.Entidades.Sondagem.Sondagem sondagemAtiva,
        long questaoIdPrincipal,
        Dictionary<(int CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> respostas,
        bool ehRelatorio = false)
        => ConstruirColunaAlunoPublicoInterno(colunaBase, aluno, sondagemAtiva, questaoIdPrincipal, respostas, ehRelatorio);

    public static RespostasProcessadasDto ProcessarRespostasPublico(
        Dictionary<(long CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> respostasAlunosPorQuestoes,
        Dominio.Entidades.Questionario.Questao linguaPortuguesaSegundaLingua,
        IEnumerable<AlunoElasticDto> alunosAtivos,
        DateTime dataInicioSondagem)
        => ProcessarRespostas(respostasAlunosPorQuestoes, linguaPortuguesaSegundaLingua, alunosAtivos, dataInicioSondagem);

    public static Task<List<ColunaQuestionarioDto>> ObterColunasOuLancarExcecaoPublico(
        ICollection<SondagemPeriodoBimestre> periodosBimestre,
        IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas,
        int? bimestreId)
        => ObterColunasOuLancarExcecao(periodosBimestre, questoesAtivas, bimestreId);
}