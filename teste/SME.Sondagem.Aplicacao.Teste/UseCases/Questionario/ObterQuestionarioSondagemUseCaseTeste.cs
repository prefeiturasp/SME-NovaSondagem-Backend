using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dados.Repositorio.Elastic;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.UseCases.Questionario;

public class ObterQuestionarioSondagemUseCaseTeste
{
    private readonly Mock<IRepositorioElasticTurma> _mockRepositorioElasticTurma;
    private readonly Mock<IRepositorioElasticAluno> _mockRepositorioElasticAluno;
    private readonly Mock<IRepositorioRespostaAluno> _mockRepositorioRespostaAluno;
    private readonly Mock<IRepositorioSondagem> _mockRepositorioSondagem;
    private readonly Mock<IRepositorioQuestao> _mockRepositorioQuestao;
    private readonly Mock<IRepositorioBimestre> _mockRepositorioBimestre;
    private readonly Mock<IAlunoPapService> _mockAlunoPapService;
    private readonly Mock<IControleAcessoService> _mockControleAcessoService;
    private readonly ObterQuestionarioSondagemUseCase _useCase;

    public ObterQuestionarioSondagemUseCaseTeste()
    {
        _mockRepositorioElasticTurma = new Mock<IRepositorioElasticTurma>();
        _mockRepositorioElasticAluno = new Mock<IRepositorioElasticAluno>();
        _mockRepositorioRespostaAluno = new Mock<IRepositorioRespostaAluno>();
        _mockRepositorioSondagem = new Mock<IRepositorioSondagem>();
        _mockRepositorioQuestao = new Mock<IRepositorioQuestao>();
        _mockRepositorioBimestre = new Mock<IRepositorioBimestre>();
        _mockAlunoPapService = new Mock<IAlunoPapService>();
        _mockControleAcessoService = new Mock<IControleAcessoService>();

        // Criar os agregadores com os mocks
        var repositoriosElastic = new RepositoriosElastic(
            _mockRepositorioElasticTurma.Object,
            _mockRepositorioElasticAluno.Object
        );

        var repositoriosSondagem = new RepositoriosSondagem(
            _mockRepositorioSondagem.Object,
            _mockRepositorioQuestao.Object,
            _mockRepositorioRespostaAluno.Object,
            _mockRepositorioBimestre.Object
        );

        _useCase = new ObterQuestionarioSondagemUseCase(
            repositoriosElastic,
            repositoriosSondagem,
            _mockAlunoPapService.Object,
            _mockControleAcessoService.Object
        );
    }

    #region Testes do Construtor

    [Fact]
    public void Construtor_DeveLancarArgumentNullException_QuandoRepositoriosElasticForNulo()
    {
        var repositoriosSondagem = new RepositoriosSondagem(
            _mockRepositorioSondagem.Object,
            _mockRepositorioQuestao.Object,
            _mockRepositorioRespostaAluno.Object,
            _mockRepositorioBimestre.Object
        );

        Assert.Throws<ArgumentNullException>(() => new ObterQuestionarioSondagemUseCase(
            null!,
            repositoriosSondagem,
            _mockAlunoPapService.Object,
            _mockControleAcessoService.Object
        ));
    }

    [Fact]
    public void Construtor_DeveLancarArgumentNullException_QuandoRepositoriosSondagemForNulo()
    {
        var repositoriosElastic = new RepositoriosElastic(
            _mockRepositorioElasticTurma.Object,
            _mockRepositorioElasticAluno.Object
        );

        Assert.Throws<ArgumentNullException>(() => new ObterQuestionarioSondagemUseCase(
            repositoriosElastic,
            null!,
            _mockAlunoPapService.Object,
            _mockControleAcessoService.Object
        ));
    }

    [Fact]
    public void Construtor_DeveLancarArgumentNullException_QuandoAlunoPapServiceForNulo()
    {
        var repositoriosElastic = new RepositoriosElastic(
            _mockRepositorioElasticTurma.Object,
            _mockRepositorioElasticAluno.Object
        );

        var repositoriosSondagem = new RepositoriosSondagem(
            _mockRepositorioSondagem.Object,
            _mockRepositorioQuestao.Object,
            _mockRepositorioRespostaAluno.Object,
            _mockRepositorioBimestre.Object
        );

        Assert.Throws<ArgumentNullException>(() => new ObterQuestionarioSondagemUseCase(
            repositoriosElastic,
            repositoriosSondagem,
            null!,
            _mockControleAcessoService.Object
        ));
    }

    [Fact]
    public void Construtor_DeveLancarArgumentNullException_QuandoControleAcessoServiceForNulo()
    {
        var repositoriosElastic = new RepositoriosElastic(
            _mockRepositorioElasticTurma.Object,
            _mockRepositorioElasticAluno.Object
        );

        var repositoriosSondagem = new RepositoriosSondagem(
            _mockRepositorioSondagem.Object,
            _mockRepositorioQuestao.Object,
            _mockRepositorioRespostaAluno.Object,
            _mockRepositorioBimestre.Object
        );

        Assert.Throws<ArgumentNullException>(() => new ObterQuestionarioSondagemUseCase(
            repositoriosElastic,
            repositoriosSondagem,
            _mockAlunoPapService.Object,
            null!
        ));
    }

    #endregion

    #region Testes dos Agregadores

    [Fact]
    public void RepositoriosElastic_DeveLancarArgumentNullException_QuandoRepositorioElasticTurmaForNulo()
    {
        Assert.Throws<ArgumentNullException>(() => new RepositoriosElastic(
            null!,
            _mockRepositorioElasticAluno.Object
        ));
    }

    [Fact]
    public void RepositoriosElastic_DeveLancarArgumentNullException_QuandoRepositorioElasticAlunoForNulo()
    {
        Assert.Throws<ArgumentNullException>(() => new RepositoriosElastic(
            _mockRepositorioElasticTurma.Object,
            null!
        ));
    }

    [Fact]
    public void RepositoriosSondagem_DeveLancarArgumentNullException_QuandoRepositorioSondagemForNulo()
    {
        Assert.Throws<ArgumentNullException>(() => new RepositoriosSondagem(
            null!,
            _mockRepositorioQuestao.Object,
            _mockRepositorioRespostaAluno.Object,
            _mockRepositorioBimestre.Object
        ));
    }

    [Fact]
    public void RepositoriosSondagem_DeveLancarArgumentNullException_QuandoRepositorioQuestaoForNulo()
    {
        Assert.Throws<ArgumentNullException>(() => new RepositoriosSondagem(
            _mockRepositorioSondagem.Object,
            null!,
            _mockRepositorioRespostaAluno.Object,
            _mockRepositorioBimestre.Object
        ));
    }

    [Fact]
    public void RepositoriosSondagem_DeveLancarArgumentNullException_QuandoRepositorioRespostaAlunoForNulo()
    {
        Assert.Throws<ArgumentNullException>(() => new RepositoriosSondagem(
            _mockRepositorioSondagem.Object,
            _mockRepositorioQuestao.Object,
            null!,
            _mockRepositorioBimestre.Object
        ));
    }

    [Fact]
    public void RepositoriosSondagem_DeveLancarArgumentNullException_QuandoRepositorioBimestreForNulo()
    {
        Assert.Throws<ArgumentNullException>(() => new RepositoriosSondagem(
            _mockRepositorioSondagem.Object,
            _mockRepositorioQuestao.Object,
            _mockRepositorioRespostaAluno.Object,
            null!
        ));
    }

    #endregion

    #region Testes de Validação de Filtro

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveLancarArgumentNullException_QuandoFiltroForNulo()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _useCase.ObterQuestionarioSondagem(null!, CancellationToken.None));
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveLancarErroInternoException_QuandoTurmaNaoForLocalizada()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        _mockRepositorioElasticTurma.Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TurmaElasticDto?)null!);

        var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));
        Assert.Equal("Turma não localizada", exception.Message);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveLancarRegraNegocioException_QuandoProficienciaIdForZero()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 0 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };

        _mockRepositorioElasticTurma.Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(turma);

        var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));
        Assert.Equal("A proficiência é obrigatória no filtro", exception.Message);
    }

    #endregion

    #region Testes de Validação de Sondagem

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveLancarErroInternoException_QuandoNaoHouverSondagemAtiva()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 1, AnoTurma = "1", AnoLetivo = 2024 };

        _mockRepositorioElasticTurma.Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(turma);
        _mockRepositorioSondagem.Setup(x => x.ObterSondagemAtiva(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dominio.Entidades.Sondagem.Sondagem?)null!);

        var exception = await Assert.ThrowsAsync<ErroInternoException>(() =>
            _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));
        Assert.Equal("Não há sondagem ativa cadastrada no sistema", exception.Message);
    }

    #endregion

    #region Testes de Validação de Modalidade e Ano

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(6)]
    public async Task ObterQuestionarioSondagem_DeveLancarErroInternoException_QuandoModalidadeNaoForSuportada(int modalidade)
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = modalidade, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();

        _mockRepositorioElasticTurma.Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(turma);
        _mockRepositorioSondagem.Setup(x => x.ObterSondagemAtiva(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sondagem);

        var exception = await Assert.ThrowsAsync<ErroNaoEncontradoException>(() =>
            _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None));
        Assert.Equal("Não há questionário para a modalidade informada", exception.Message);
    }

    [Theory]
    [InlineData(5, "1")]
    [InlineData(5, "2")]
    [InlineData(5, "3")]
    [InlineData(3, "1")]
    [InlineData(3, "2")]
    [InlineData(3, "3")]
    public async Task ObterQuestionarioSondagem_DeveRetornarQuestionarioComSucesso_ParaModalidadesEAnosValidos(
        int modalidade, string anoTurma)
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = modalidade, AnoTurma = anoTurma, AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos);

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        Assert.NotNull(resultado);
        Assert.NotNull(resultado.Estudantes);
        Assert.Equal(2, resultado.Estudantes.Count());
        Assert.Equal("Questão Teste", resultado.TituloTabelaRespostas);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveRetornarQuestionarioComEstudantesComPropriedadesCorretas()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos);

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        var primeiroEstudante = resultado.Estudantes!.First();
        Assert.Equal("1", primeiroEstudante.NumeroAlunoChamada);
        Assert.Equal(1001, primeiroEstudante.Codigo);
        Assert.Equal("João Silva", primeiroEstudante.Nome);
        Assert.False(primeiroEstudante.LinguaPortuguesaSegundaLingua);
        Assert.False(primeiroEstudante.Pap);
        Assert.False(primeiroEstudante.PossuiDeficiencia);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveRetornarEstudantesComPap_QuandoAlunosEstiveremNoProgramaPap()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksBase(filtro, turma, sondagem, questoes);
        _mockRepositorioElasticAluno.Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunos);

        var alunosComPap = new Dictionary<int, bool> { { 1001, true }, { 1002, false } };
        _mockAlunoPapService.Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
            It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunosComPap);

        _mockRepositorioRespostaAluno.Setup(x => x.VerificarAlunosPossuiLinguaPortuguesaAsync(
            It.IsAny<List<int>>(), It.IsAny<Dominio.Entidades.Questionario.Questao>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool>());

        _mockRepositorioRespostaAluno.Setup(x => x.ObterRespostasAlunosPorQuestoesAsync(
            It.IsAny<List<long>>(), It.IsAny<List<long>>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<(long, long, int?), RespostaAluno>());

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        Assert.True(resultado.Estudantes!.First(e => e.Codigo == 1001).Pap);
        Assert.False(resultado.Estudantes!.First(e => e.Codigo == 1002).Pap);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveRetornarEstudantesComLinguaPortuguesaSegundaLingua()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksBase(filtro, turma, sondagem, questoes);
        _mockRepositorioElasticAluno.Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunos);

        var alunosComLingua = new Dictionary<int, bool> { { 1001, true }, { 1002, false } };
        _mockRepositorioRespostaAluno.Setup(x => x.VerificarAlunosPossuiLinguaPortuguesaAsync(
            It.IsAny<List<int>>(), It.IsAny<Dominio.Entidades.Questionario.Questao>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunosComLingua);

        _mockAlunoPapService.Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
            It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool>());

        _mockRepositorioRespostaAluno.Setup(x => x.ObterRespostasAlunosPorQuestoesAsync(
            It.IsAny<List<long>>(), It.IsAny<List<long>>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<(long, long, int?), RespostaAluno>());

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        Assert.True(resultado.Estudantes!.First(e => e.Codigo == 1001).LinguaPortuguesaSegundaLingua);
        Assert.False(resultado.Estudantes!.First(e => e.Codigo == 1002).LinguaPortuguesaSegundaLingua);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveRetornarEstudantesComDeficiencia()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();

        var alunosComDeficiencia = new List<AlunoElasticDto>
        {
            new AlunoElasticDto
            {
                CodigoAluno = 1001,
                NumeroAlunoChamada = "1",
                NomeAluno = "João Silva",
                PossuiDeficiencia = 1
            },
            new AlunoElasticDto
            {
                CodigoAluno = 1002,
                NumeroAlunoChamada = "2",
                NomeAluno = "Maria Santos",
                PossuiDeficiencia = 0
            }
        };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunosComDeficiencia);

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        Assert.True(resultado.Estudantes!.First(e => e.Codigo == 1001).PossuiDeficiencia);
        Assert.False(resultado.Estudantes!.First(e => e.Codigo == 1002).PossuiDeficiencia);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_AssociaRespostaNaColunaCorreta()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };

        var questaoRespondidaId = 13;

        ConfigurarMocksBase(
            filtro,
            new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 },
            CriarSondagemMock(),
            CriarQuestoesMockComSubResposta(questaoRespondidaId)
        );

        _mockRepositorioElasticAluno
            .Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
            new AlunoElasticDto
            {
                CodigoAluno = 1001,
                NomeAluno = "Aluno Teste",
                NumeroAlunoChamada = "1",
                PossuiDeficiencia = 0
            }
            });

        var respostaAluno = new RespostaAluno(
            sondagemId: 1,
            questaoId: questaoRespondidaId,
            alunoId: 1001,
            bimestreId: 1,
            opcaoRespostaId: 10,
            dataResposta: DateTime.Now
        );

        respostaAluno.GetType().BaseType!
            .GetProperty("Id")!
            .SetValue(respostaAluno, 1);

        _mockRepositorioRespostaAluno
            .Setup(x => x.ObterRespostasAlunosPorQuestoesAsync(
                It.IsAny<List<long>>(),
                It.IsAny<List<long>>(),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<(long CodigoAluno, long QuestaoId, int? BimestreId), RespostaAluno>
            {
            { (1001L, questaoRespondidaId, 1), respostaAluno }
            });

        _mockRepositorioRespostaAluno
            .Setup(x => x.VerificarAlunosPossuiLinguaPortuguesaAsync(
                It.IsAny<List<int>>(),
                It.IsAny<Dominio.Entidades.Questionario.Questao>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool>());

        _mockAlunoPapService
            .Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
                It.IsAny<List<int>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool>());

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        var resposta = resultado.Estudantes!
            .SelectMany(e => e.Coluna!)
            .Select(c => c.Resposta)
            .FirstOrDefault();

        Assert.NotNull(resposta);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveRetornarColunaSemResposta_QuandoAlunoNaoTiverResposta()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos);

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        var primeiroEstudante = resultado.Estudantes!.First();
        var primeiraColuna = primeiroEstudante.Coluna!.First();
        Assert.Null(primeiraColuna.Resposta);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveFiltrarApenasQuestoesDoTipoComboELinguaPortuguesa()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesVariadasMock();
        var alunos = CriarAlunosMock();

        _mockRepositorioElasticTurma.Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(turma);

        _mockRepositorioSondagem.Setup(x => x.ObterSondagemAtiva(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sondagem);

        _mockRepositorioQuestao.Setup(x => x.ObterQuestoesAtivasPorFiltroAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(questoes);

        _mockRepositorioElasticAluno.Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunos);

        ConfigurarMocksComplementares();

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        _mockRepositorioRespostaAluno.Verify(x =>
            x.ObterRespostasAlunosPorQuestoesAsync(
                It.IsAny<List<long>>(),
                It.Is<List<long>>(ids => ids.Count == 3),
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveRetornarTituloVazio_QuandoNaoHouverQuestoes()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();
        var questoesSemNome = CriarQuestoesSemNomeMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoesSemNome, alunos);

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        Assert.Empty(resultado.TituloTabelaRespostas);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveRetornarColunasComPeriodoBimestreAtivo()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagemComPeriodoAtivo = CriarSondagemComPeriodoAtivoMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksCompleto(filtro, turma, sondagemComPeriodoAtivo, questoes, alunos);

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        var primeiraColuna = resultado.Estudantes!.First().Coluna!.First();
        Assert.True(primeiraColuna.PeriodoBimestreAtivo);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveRetornarColunasComOpcoesResposta()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos);

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        var primeiraColuna = resultado.Estudantes!.First()!.Coluna!.First();
        Assert.NotNull(primeiraColuna.OpcaoResposta);
        Assert.Equal(2, primeiraColuna.OpcaoResposta.Count());

        var primeiraOpcao = primeiraColuna.OpcaoResposta.First();
        Assert.Equal(1, primeiraOpcao.Id);
        Assert.Equal(1, primeiraOpcao.Ordem);
        Assert.Equal("Opção 1", primeiraOpcao.DescricaoOpcaoResposta);
        Assert.Equal("A", primeiraOpcao.Legenda);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveIgnorarPeriodosBimestresExcluidos()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagemComExcluidos = CriarSondagemComBimestresExcluidosMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksCompleto(filtro, turma, sondagemComExcluidos, questoes, alunos);

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        var primeiroEstudante = resultado.Estudantes!.First();
        Assert.Single(primeiroEstudante.Coluna ?? []);
    }

    #endregion

    #region Testes para bimestresForaDoPadrao (repositorio de bimestres)

    [Fact]
    public async Task ObterQuestionarioSondagem_UsaBimestresDoRepositorio_QuandoExistir()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };

        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        var periodoRepo = new Dominio.Entidades.Sondagem.SondagemPeriodoBimestre(1, 99, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10));
        var bimestreRepo = new Dominio.Entidades.Bimestre(99, "Bimestre Repo");
        periodoRepo.GetType().GetProperty("Bimestre")!.SetValue(periodoRepo, bimestreRepo);
        var bimestresRepo = new List<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre> { periodoRepo };

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos);

        _mockRepositorioBimestre.Setup(x => x.ObterBimestresPorQuestionarioIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bimestresRepo);

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        var primeiraColuna = resultado.Estudantes!.First().Coluna!.First();
        Assert.Equal("Bimestre Repo", primeiraColuna.DescricaoColuna);
    }

    [Fact]
    public async Task ObterQuestionarioSondagem_DeveLancarErro_QuandoRepositorioRetornarListaVazia()
    {
        var filtro = new FiltroQuestionario { TurmaId = 1, ProficienciaId = 1 };
        var turma = new TurmaElasticDto { Modalidade = 5, AnoTurma = "1", AnoLetivo = 2024 };
        var sondagem = CriarSondagemMock();
        var questoes = CriarQuestoesMock();
        var alunos = CriarAlunosMock();

        ConfigurarMocksCompleto(filtro, turma, sondagem, questoes, alunos);

        _mockRepositorioBimestre.Setup(x => x.ObterBimestresPorQuestionarioIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre>());

        var resultado = await _useCase.ObterQuestionarioSondagem(filtro, CancellationToken.None);

        var primeiraColuna = resultado.Estudantes!.First().Coluna!.First();
        Assert.Equal("1º Bimestre", primeiraColuna.DescricaoColuna);
    }

    #endregion

    #region Métodos Auxiliares
    private static Dominio.Entidades.Sondagem.Sondagem CriarSondagemMock()
    {
        var sondagem = new Dominio.Entidades.Sondagem.Sondagem(
            "Sondagem 2024",
            DateTime.Now.AddMonths(-1)
        );

        var bimestre = new Dominio.Entidades.Bimestre(1, "1º Bimestre");

        var periodo = new Dominio.Entidades.Sondagem.SondagemPeriodoBimestre(
            1,
            1,
            DateTime.Now.AddDays(-10),
            DateTime.Now.AddDays(10)
        );

        var periodosBimestreProperty = sondagem.GetType().GetProperty("PeriodosBimestre");
        var periodos = new List<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre> { periodo };
        periodosBimestreProperty?.SetValue(sondagem, periodos);

        var bimestreProperty = periodo.GetType().GetProperty("Bimestre");
        bimestreProperty?.SetValue(periodo, bimestre);

        return sondagem;
    }

    private static List<Dominio.Entidades.Questionario.Questao>
    CriarQuestoesMockComSubResposta(int subQuestaoId)
    {
        var questaoPai = new Dominio.Entidades.Questionario.Questao(
            questionarioId: 1,
            ordem: 1,
            nome: "Compreensão de textos",
            observacao: string.Empty,
            obrigatorio: true,
            tipo: TipoQuestao.Combo,
            opcionais: string.Empty,
            somenteLeitura: false,
            dimensao: 12
        );

        questaoPai.GetType().BaseType!
            .GetProperty("Id")!
            .SetValue(questaoPai, 1);

        var subQuestao = new Dominio.Entidades.Questionario.Questao(
            questionarioId: 1,
            ordem: 1,
            nome: "Localização",
            observacao: string.Empty,
            obrigatorio: true,
            tipo: TipoQuestao.Combo,
            opcionais: string.Empty,
            somenteLeitura: false,
            dimensao: 12
        );

        subQuestao.GetType().BaseType!
            .GetProperty("Id")!
            .SetValue(subQuestao, subQuestaoId);

        typeof(Dominio.Entidades.Questionario.Questao)
            .GetProperty(nameof(Dominio.Entidades.Questionario.Questao.QuestaoVinculo))!
            .SetValue(subQuestao, questaoPai);


        return new List<Dominio.Entidades.Questionario.Questao> { subQuestao };
    }

    private static Dominio.Entidades.Sondagem.Sondagem CriarSondagemComPeriodoAtivoMock()
    {
        var sondagem = new Dominio.Entidades.Sondagem.Sondagem(
            "Sondagem 2024",
            DateTime.Now.AddMonths(-1)
        );

        var bimestre = new Dominio.Entidades.Bimestre(1, "1º Bimestre");

        var periodo = new Dominio.Entidades.Sondagem.SondagemPeriodoBimestre(
            1,
            1,
            DateTime.Now.AddDays(-1),
            DateTime.Now.AddDays(1)
        );

        var periodosBimestreProperty = sondagem.GetType().GetProperty("PeriodosBimestre");
        var periodos = new List<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre> { periodo };
        periodosBimestreProperty?.SetValue(sondagem, periodos);

        var bimestreProperty = periodo.GetType().GetProperty("Bimestre");
        bimestreProperty?.SetValue(periodo, bimestre);

        return sondagem;
    }

    private static Dominio.Entidades.Sondagem.Sondagem CriarSondagemComBimestresExcluidosMock()
    {
        var sondagem = new Dominio.Entidades.Sondagem.Sondagem(
            "Sondagem 2024",
            DateTime.Now.AddMonths(-1)
        );

        var bimestre1 = new Dominio.Entidades.Bimestre(1, "1º Bimestre");
        var bimestre2 = new Dominio.Entidades.Bimestre(2, "2º Bimestre");

        var periodo1 = new Dominio.Entidades.Sondagem.SondagemPeriodoBimestre(
            1, 1, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10));

        var periodo2 = new Dominio.Entidades.Sondagem.SondagemPeriodoBimestre(
            1, 2, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10));

        periodo2.Excluido = true;

        var periodosBimestreProperty = sondagem.GetType().GetProperty("PeriodosBimestre");
        var periodos = new List<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre> { periodo1, periodo2 };
        periodosBimestreProperty?.SetValue(sondagem, periodos);

        var bimestreProperty1 = periodo1.GetType().GetProperty("Bimestre");
        bimestreProperty1?.SetValue(periodo1, bimestre1);

        var bimestreProperty2 = periodo2.GetType().GetProperty("Bimestre");
        bimestreProperty2?.SetValue(periodo2, bimestre2);

        return sondagem;
    }

    private static List<Dominio.Entidades.Questionario.Questao> CriarQuestoesMock()
    {
        var questao = new Dominio.Entidades.Questionario.Questao(
            1, 1, "Questão Teste", "Observação", true,
            TipoQuestao.Combo, "", false, 1);

        var opcaoResposta1 = new Dominio.Entidades.Questionario.OpcaoResposta(
            1, "Opção 1", "A", "#FFFFFF", "#000000");

        var opcaoResposta2 = new Dominio.Entidades.Questionario.OpcaoResposta(
            1, "Opção 2", "B", "#FFFFFF", "#000000");

        var questaoOpcao1 = new Dominio.Entidades.Questionario.QuestaoOpcaoResposta(1, 1, 1);
        var questaoOpcao2 = new Dominio.Entidades.Questionario.QuestaoOpcaoResposta(1, 2, 2);

        questaoOpcao1.GetType().BaseType!.GetProperty("Id")!.SetValue(questaoOpcao1, 1);
        questaoOpcao2.GetType().BaseType!.GetProperty("Id")!.SetValue(questaoOpcao2, 2);

        var opcaoRespostaProperty1 = questaoOpcao1.GetType().GetProperty("OpcaoResposta");
        opcaoRespostaProperty1?.SetValue(questaoOpcao1, opcaoResposta1);

        var opcaoRespostaProperty2 = questaoOpcao2.GetType().GetProperty("OpcaoResposta");
        opcaoRespostaProperty2?.SetValue(questaoOpcao2, opcaoResposta2);

        var questaoOpcoesProperty = questao.GetType().GetProperty("QuestaoOpcoes");
        var opcoes = new List<Dominio.Entidades.Questionario.QuestaoOpcaoResposta>
            { questaoOpcao1, questaoOpcao2 };
        questaoOpcoesProperty?.SetValue(questao, opcoes);

        return new List<Dominio.Entidades.Questionario.Questao> { questao };
    }

    private static List<Dominio.Entidades.Questionario.Questao> CriarQuestoesVariadasMock()
    {
        var questaoCombo = new Dominio.Entidades.Questionario.Questao(
            1, 1, "Questão Combo", "Obs", true, TipoQuestao.Combo, "", false, 1);

        var questaoLingua = new Dominio.Entidades.Questionario.Questao(
            1, 2, "Questão Língua", "Obs", true,
            TipoQuestao.LinguaPortuguesaSegundaLingua, "", false, 1);

        var questaoTexto = new Dominio.Entidades.Questionario.Questao(
            1, 3, "Questão Texto", "Obs", true, TipoQuestao.Texto, "", false, 1);

        return new List<Dominio.Entidades.Questionario.Questao>
            { questaoCombo, questaoLingua, questaoTexto };
    }

    private static List<Dominio.Entidades.Questionario.Questao> CriarQuestoesSemNomeMock()
    {
        var questao = new Dominio.Entidades.Questionario.Questao(
            1, 1, "", "", true, TipoQuestao.Combo, "", false, 1);

        return new List<Dominio.Entidades.Questionario.Questao> { questao };
    }

    private static List<AlunoElasticDto> CriarAlunosMock()
    {
        return new List<AlunoElasticDto>
        {
            new AlunoElasticDto
            {
                CodigoAluno = 1001,
                NumeroAlunoChamada = "1",
                NomeAluno = "João Silva",
                PossuiDeficiencia = 0
            },
            new AlunoElasticDto
            {
                CodigoAluno = 1002,
                NumeroAlunoChamada = "2",
                NomeAluno = "Maria Santos",
                PossuiDeficiencia = 0
            }
        };
    }

    private void ConfigurarMocksBase(
        FiltroQuestionario filtro,
        TurmaElasticDto turma,
        Dominio.Entidades.Sondagem.Sondagem sondagem,
        List<Dominio.Entidades.Questionario.Questao> questoes)
    {
        _mockRepositorioElasticTurma.Setup(x => x.ObterTurmaPorId(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(turma);

        _mockRepositorioSondagem.Setup(x => x.ObterSondagemAtiva(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sondagem);

        _mockRepositorioQuestao.Setup(x => x.ObterQuestoesAtivasPorFiltroAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(questoes);
    }

    private void ConfigurarMocksComplementares()
    {
        _mockRepositorioRespostaAluno.Setup(x => x.VerificarAlunosPossuiLinguaPortuguesaAsync(
            It.IsAny<List<int>>(), It.IsAny<Dominio.Entidades.Questionario.Questao>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool>());

        _mockAlunoPapService.Setup(x => x.VerificarAlunosPossuemProgramaPapAsync(
            It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, bool>());

        _mockRepositorioRespostaAluno.Setup(x => x.ObterRespostasAlunosPorQuestoesAsync(
            It.IsAny<List<long>>(), It.IsAny<List<long>>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<(long, long, int?), RespostaAluno>());
    }

    private void ConfigurarMocksCompleto(
        FiltroQuestionario filtro,
        TurmaElasticDto turma,
        Dominio.Entidades.Sondagem.Sondagem sondagem,
        List<Dominio.Entidades.Questionario.Questao> questoes,
        List<AlunoElasticDto> alunos)
    {
        ConfigurarMocksBase(filtro, turma, sondagem, questoes);

        _mockRepositorioElasticAluno.Setup(x => x.ObterAlunosPorIdTurma(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunos);

        ConfigurarMocksComplementares();
    }

    #endregion
}