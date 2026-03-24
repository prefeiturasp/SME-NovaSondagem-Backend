using Moq;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Exceptions;
using SME.Sondagem.Infra.Teste.DTO;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.SondagemRespostas;

public class SondagemSalvarRespostasUseCaseTeste
{
    private readonly Mock<IRepositorioSondagem> _repositorioSondagem;
    private readonly Mock<IRepositorioRespostaAluno> _repositorioSondagemResposta;
    private readonly Mock<IRepositorioQuestao> _repositorioQuestao;
    private readonly Mock<IControleAcessoService> _controleAcessoService;
    private readonly SondagemSalvarRespostasUseCase _useCase;
    private readonly CancellationToken _cancellationToken;
    private readonly Mock<RepositoriosElastic> _repositoriosElastic;
    private readonly Mock<RepositoriosSondagem> _repositoriosSondagem;
    private readonly Mock<RepositorioSondagemRelatorioPorTodasTurma> _repositorioSondagemRelatorioPorTodasTurma;
    private readonly Mock<IRepositorioElasticTurma> _repositorioElasticTurma;
    private readonly Mock<IRepositorioElasticAluno> _repositorioElasticAluno;
    private readonly Mock<IRepositorioBimestre> _repositorioBimestre;
    private readonly Mock<IRepositorioProficiencia> _repositorioProficiencia;
    private readonly Mock<IDadosAlunosService> _dadosAlunosService;


    private readonly Mock<IRepositorioComponenteCurricular> _repositorioComponenteCurricular;
    private readonly Mock<IUeComDreEolService> _ueComDreEolService;
    private readonly ObterSondagemRelatorioPorTodasTurmaUseCase _0bterSondagemRelatorioPorTodasTurmaUseCase;

    public SondagemSalvarRespostasUseCaseTeste()
    {
        _ueComDreEolService = new Mock<IUeComDreEolService>();
        _dadosAlunosService = new Mock<IDadosAlunosService>();
        _repositorioComponenteCurricular = new Mock<IRepositorioComponenteCurricular>();
        _repositorioProficiencia = new Mock<IRepositorioProficiencia>();
        _repositorioElasticTurma = new Mock<IRepositorioElasticTurma>();
        _repositorioBimestre = new Mock<IRepositorioBimestre>();
        _repositorioElasticAluno = new Mock<IRepositorioElasticAluno>();
        _repositorioSondagem = new Mock<IRepositorioSondagem>();
        _repositorioSondagemResposta = new Mock<IRepositorioRespostaAluno>();
        _repositorioQuestao = new Mock<IRepositorioQuestao>();
        _controleAcessoService = new Mock<IControleAcessoService>();
        _repositoriosElastic = new Mock<RepositoriosElastic>(_repositorioElasticTurma.Object, _repositorioElasticAluno.Object);
        _repositoriosSondagem = new Mock<RepositoriosSondagem>(_repositorioSondagem.Object, _repositorioQuestao.Object, _repositorioSondagemResposta.Object, _repositorioBimestre.Object, _repositorioComponenteCurricular.Object, _repositorioProficiencia.Object);
        _repositorioSondagemRelatorioPorTodasTurma = new Mock<RepositorioSondagemRelatorioPorTodasTurma>(_dadosAlunosService.Object, _ueComDreEolService.Object);

        _cancellationToken = CancellationToken.None;

        _useCase = new SondagemSalvarRespostasUseCase(
            _repositorioSondagem.Object,
            _repositorioSondagemResposta.Object,
            _repositorioQuestao.Object,
            _controleAcessoService.Object
        );

        _repositorioComponenteCurricular = new Mock<IRepositorioComponenteCurricular>();
        _0bterSondagemRelatorioPorTodasTurmaUseCase = new ObterSondagemRelatorioPorTodasTurmaUseCase(_ueComDreEolService.Object, _repositoriosElastic.Object, _repositoriosSondagem.Object, _repositorioSondagemRelatorioPorTodasTurma.Object);
    }

    [Fact]
    public async Task DeveRetornarNegocioException_QuandoNenhumaSondagemAtivaEncontrada()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "TURMA-TESTE";

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(dto.TurmaId))
            .ReturnsAsync(true);

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())!
            .ReturnsAsync((Dominio.Entidades.Sondagem.Sondagem?)null);

        var exception = await Assert.ThrowsAsync<NegocioException>(() =>
            _useCase.SalvarOuAtualizarSondagemAsync(dto));

        Assert.Equal(MensagemNegocioComuns.NENHUM_SONDAGEM_ATIVA_ENCONRADA, exception.Message);
    }

    [Fact]
    public async Task DeveRetornarNegocioException_QuandoSondagemIdDiferenteDaAtiva()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "TURMA-TESTE";

        var sondagemAtiva = SondagemMockData.CriarSondagemAtiva(2);

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(sondagemAtiva);

        var exception = await Assert.ThrowsAsync<NegocioException>(() =>
            _useCase.SalvarOuAtualizarSondagemAsync(dto));

        Assert.Equal(MensagemNegocioComuns.SALVAR_SOMENTE_PARA_SONDAGEM_ATIVA, exception.Message);
    }

    [Fact]
    public async Task DeveSalvarComSucesso_QuandoDadosValidos()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "TURMA-TESTE";

        var sondagemAtiva = SondagemMockData.CriarSondagemAtiva(1, 1);
        var questaoLP = CriarQuestaoLinguaPortuguesaSegundaLingua(1);

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(sondagemAtiva);

        _repositorioQuestao
            .Setup(x => x.ObterQuestionarioIdPorQuestoesAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new List<Questao> { questaoLP });

        _repositorioQuestao
            .Setup(x => x.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(
                It.IsAny<int>(),
                TipoQuestao.LinguaPortuguesaSegundaLingua))
            .ReturnsAsync(questaoLP);

        _repositorioSondagemResposta
            .Setup(x => x.ObterRespostasPorSondagemEAlunosAsync(
                It.IsAny<int>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new List<RespostaAluno>());

        _repositorioSondagemResposta
            .Setup(x => x.SalvarAsync(It.IsAny<List<RespostaAluno>>()))
            .ReturnsAsync(true);

        var resultado = await _useCase.SalvarOuAtualizarSondagemAsync(dto);

        Assert.True(resultado);
    }

    [Fact]
    public async Task DeveAtualizarRespostaExistente()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "TURMA-TESTE";

        var questaoLP = CriarQuestaoLinguaPortuguesaSegundaLingua(1);
        var respostaExistente = new RespostaAluno(1, 101, questaoLP.Id, 2, DateTime.UtcNow.AddDays(-1), null);

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(SondagemMockData.CriarSondagemAtiva(1, 1));

        _repositorioQuestao
            .Setup(x => x.ObterQuestionarioIdPorQuestoesAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new List<Questao> { questaoLP });

        _repositorioQuestao
            .Setup(x => x.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(
                It.IsAny<int>(),
                TipoQuestao.LinguaPortuguesaSegundaLingua))
            .ReturnsAsync(questaoLP);

        _repositorioSondagemResposta
            .Setup(x => x.ObterRespostasPorSondagemEAlunosAsync(
                It.IsAny<int>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new List<RespostaAluno> { respostaExistente });

        _repositorioSondagemResposta
            .Setup(x => x.SalvarAsync(It.IsAny<List<RespostaAluno>>()))
            .ReturnsAsync(true);

        var resultado = await _useCase.SalvarOuAtualizarSondagemAsync(dto);

        Assert.True(resultado);
    }

    [Fact]
    public async Task DeveRetornarExcecao_QuandoSemPermissao()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "TURMA-TESTE";

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(dto.TurmaId))
            .ReturnsAsync(false);

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(SondagemMockData.CriarSondagemAtiva(dto.SondagemId, 1));

        var exception = await Assert.ThrowsAsync<RegraNegocioException>(() =>
            _useCase.SalvarOuAtualizarSondagemAsync(dto));

        Assert.Equal(
            MensagemNegocioComuns.SEM_PERMISSAO_SALVAR_SONDAGEM,
            exception.Message);
    }

    [Fact]
    public async Task DeveRetornarArrayVazioNoObterExtracaoDadosRespostasAsync()
    {
        var modalidadeId = 1;
        var componenteCurricularId = 1;
        _repositorioSondagemResposta
                .Setup(x => x.ObterExtracaoDadosRespostasAsync(modalidadeId, componenteCurricularId))
                .ReturnsAsync([]);

        var uc = await _0bterSondagemRelatorioPorTodasTurmaUseCase.ObterSondagemRelatorio(_cancellationToken);
        Assert.NotNull(uc);
        Assert.NotNull(uc.FileName);

    }

    private static Questao CriarQuestaoLinguaPortuguesaSegundaLingua(int questionarioId)
    {
        var questao = new Questao(
            questionarioId,
            1,
            "Língua Portuguesa é Segunda Língua?",
            string.Empty,
            false,
            TipoQuestao.LinguaPortuguesaSegundaLingua,
            string.Empty,
            false,
            1,
            null,
            null,
            null,
            null,
            null)
        {
            Id = 999
        };

        var opcaoSim = new OpcaoResposta(1, "Sim", "S", null, null) { Id = 1 };
        var opcaoNao = new OpcaoResposta(2, "Não", "N", null, null) { Id = 2 };

        var questaoOpcaoSim = new QuestaoOpcaoResposta(questao.Id, opcaoSim.Id, 1);
        typeof(QuestaoOpcaoResposta)
            .GetProperty("OpcaoResposta")!
            .SetValue(questaoOpcaoSim, opcaoSim, null);
        questao.QuestaoOpcoes.Add(questaoOpcaoSim);

        var questaoOpcaoNao = new QuestaoOpcaoResposta(questao.Id, opcaoNao.Id, 1);
        typeof(QuestaoOpcaoResposta)
            .GetProperty("OpcaoResposta")!
            .SetValue(questaoOpcaoNao, opcaoNao, null);
        questao.QuestaoOpcoes.Add(questaoOpcaoNao);

        return questao;
    }
}
