using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Services.SGP;
using SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Dominio.ValueObjects;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Exceptions;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infra.Teste.DTO;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.SondagemRespostas;

public class SondagemSalvarRespostasUseCaseTeste
{
    private const string CODIGO_ESCOLA_PERMITIDA = "111111";
    private const string TURMA_ID = "123456";
    private const string ANO_TURMA = "2023";

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
    private readonly Mock<IConsultaDeDresService> _consultaDeDresService;
    private readonly Mock<IHttpClientFactory> _httpClientFactory;
    private readonly Mock<IServicoLog> _servicoLog;


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
        _consultaDeDresService = new Mock<IConsultaDeDresService>();
        ConfigurarMockAlunosTurma(CriarAlunosAtivosParaSalvar());
        _repositoriosElastic = new Mock<RepositoriosElastic>(_repositorioElasticTurma.Object, _repositorioElasticAluno.Object);
        _repositoriosSondagem = new Mock<RepositoriosSondagem>(_repositorioSondagem.Object, _repositorioQuestao.Object, _repositorioSondagemResposta.Object, _repositorioBimestre.Object, _repositorioComponenteCurricular.Object, _repositorioProficiencia.Object, new Mock<IRepositorioRacaCor>().Object, new Mock<IRepositorioGeneroSexo>().Object);
        _repositorioSondagemRelatorioPorTodasTurma = new Mock<RepositorioSondagemRelatorioPorTodasTurma>(_dadosAlunosService.Object, _ueComDreEolService.Object);
        _httpClientFactory = new Mock<IHttpClientFactory>();
        _servicoLog = new Mock<IServicoLog>();
        _cancellationToken = CancellationToken.None;

        _useCase = new SondagemSalvarRespostasUseCase(
            _repositorioSondagem.Object,
            _repositorioSondagemResposta.Object,
            _repositorioQuestao.Object,
            _controleAcessoService.Object,
            _repositorioElasticTurma.Object,
            _repositorioElasticAluno.Object,
            _dadosAlunosService.Object
        );

        _repositorioComponenteCurricular = new Mock<IRepositorioComponenteCurricular>();
        _0bterSondagemRelatorioPorTodasTurmaUseCase = new ObterSondagemRelatorioPorTodasTurmaUseCase(_ueComDreEolService.Object, _repositoriosElastic.Object, _repositoriosSondagem.Object, _repositorioSondagemRelatorioPorTodasTurma.Object, _consultaDeDresService.Object);
    }

    private void ConfigurarMockTurmaSucesso()
    {
        _repositorioElasticTurma
            .Setup(r => r.ObterTurmaPorId(It.IsAny<FiltroQuestionario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TurmaElasticDto
            {
                CodigoTurma = int.Parse(TURMA_ID),
                CodigoEscola = CODIGO_ESCOLA_PERMITIDA,
                AnoTurma = ANO_TURMA
            });
    }

    private void ConfigurarMockAlunosTurma(IEnumerable<AlunoElasticDto> alunos)
    {
        _repositorioElasticAluno
            .Setup(r => r.ObterAlunosPorIdTurma(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(alunos);
    }

    private static List<AlunoElasticDto> CriarAlunosAtivosParaSalvar() =>
    [
        new() { CodigoAluno = 101, CodigoSituacaoMatricula = (int)SituacaoMatriculaAluno.Ativo },
        new() { CodigoAluno = 102, CodigoSituacaoMatricula = (int)SituacaoMatriculaAluno.Ativo },
        new() { CodigoAluno = 103, CodigoSituacaoMatricula = (int)SituacaoMatriculaAluno.Ativo }
    ];

    [Fact]
    public async Task DeveRetornarNegocioException_QuandoNenhumaSondagemAtivaEncontrada()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "1";

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

        ConfigurarMockTurmaSucesso();

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
        dto.TurmaId = "1";

        var sondagemAtiva = SondagemMockData.CriarSondagemAtiva(2);

        ConfigurarMockTurmaSucesso();

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
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
        dto.TurmaId = "1";

        ConfigurarMockTurmaSucesso();

        var sondagemAtiva = SondagemMockData.CriarSondagemAtiva(1, 1);
        var questaoLP = CriarQuestaoLinguaPortuguesaSegundaLingua(1);

        _controleAcessoService
             .Setup(x => x.ValidarPermissaoAcessoAsync(
                 It.IsAny<string>(),
                 It.IsAny<string>(),
                 It.IsAny<string>()))
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

        _dadosAlunosService
                .Setup(x => x.ObterDadosRacaGeneroAlunos(It.IsAny<int>()))
                .ReturnsAsync(new List<Infrastructure.Dtos.AlunoRacaGeneroDto>
                {
                    new() { CodigoAluno = 101, Raca = "Parda", Sexo = "Feminino" }
                });

        _repositorioSondagemResposta
            .Setup(x => x.ObterRespostasPorSondagemEAlunosAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
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
    public async Task DeveSalvarComSucesso_QuandoSemQuestaoLinguaPortuguesa()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "1";

        ConfigurarMockTurmaSucesso();

        var sondagemAtiva = SondagemMockData.CriarSondagemAtiva(1, 1);
        var questaoLP = CriarQuestaoLinguaPortuguesaSegundaLingua(1);

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _dadosAlunosService
            .Setup(x => x.ObterDadosRacaGeneroAlunos(It.IsAny<int>()))
            .ReturnsAsync(new List<Infrastructure.Dtos.AlunoRacaGeneroDto>());

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(sondagemAtiva);

        _repositorioQuestao
            .Setup(x => x.ObterQuestionarioIdPorQuestoesAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new List<Questao> { questaoLP });

        _repositorioQuestao
            .Setup(x => x.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(
                It.IsAny<int>(), TipoQuestao.LinguaPortuguesaSegundaLingua))
            .ReturnsAsync((Questao?)null);

        _repositorioSondagemResposta
            .Setup(x => x.ObterRespostasPorSondagemEAlunosAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IEnumerable<int>>(), It.IsAny<IEnumerable<int>>()))
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
        dto.TurmaId = "1";



        var questaoLP = CriarQuestaoLinguaPortuguesaSegundaLingua(1);
        var contextoEdu = CriarContextoEducacional();
        var respostaExistente = new RespostaAluno(1, 101, questaoLP.Id, 2, DateTime.UtcNow.AddDays(-1), contextoEdu);

        _controleAcessoService
                .Setup(x => x.ValidarPermissaoAcessoAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(true);

        ConfigurarMockTurmaSucesso();

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(SondagemMockData.CriarSondagemAtiva(1, 1));

        _dadosAlunosService
            .Setup(x => x.ObterDadosRacaGeneroAlunos(It.IsAny<int>()))
            .ReturnsAsync(new List<Infrastructure.Dtos.AlunoRacaGeneroDto>
            {
                new() { CodigoAluno = 101, Raca = "Parda", Sexo = "Feminino" }
            });

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
                It.IsAny<string>(),
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
    public async Task DeveCriarRespostaNaTurmaAtual_QuandoRespostaExistentePertencerATurmaAnterior()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "1";
        dto.Alunos = [dto.Alunos.First()];
        dto.Alunos[0].Respostas = [];

        var questaoLP = CriarQuestaoLinguaPortuguesaSegundaLingua(1);
        var respostaTurmaAnterior = new RespostaAluno(
            1,
            dto.Alunos[0].Codigo,
            questaoLP.Id,
            2,
            DateTime.UtcNow.AddDays(-1),
            CriarContextoEducacional() with { TurmaId = "2", BimestreId = null })
        {
            Id = 99
        };

        ConfigurarCenarioSalvar(questaoLP, [respostaTurmaAnterior]);

        List<RespostaAluno>? respostasSalvas = null;
        _repositorioSondagemResposta
            .Setup(x => x.SalvarAsync(
                It.IsAny<List<RespostaAluno>>(),
                It.IsAny<CancellationToken>()))
            .Callback<List<RespostaAluno>, CancellationToken>((respostas, _) => respostasSalvas = respostas)
            .ReturnsAsync(true);

        var resultado = await _useCase.SalvarOuAtualizarSondagemAsync(dto);

        Assert.True(resultado);
        var novaResposta = Assert.Single(respostasSalvas!);
        Assert.Equal(0, novaResposta.Id);
        Assert.Equal("1", novaResposta.TurmaId);
        Assert.Equal(1, novaResposta.OpcaoRespostaId);
        Assert.Equal(2, respostaTurmaAnterior.OpcaoRespostaId);
    }

    [Fact]
    public async Task DeveRetornarSucessoSemSalvar_QuandoRespostaNaoTiverAlteracoes()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "1";
        dto.AnoTurma = 2023;
        dto.Alunos = [dto.Alunos.First()];
        dto.Alunos[0].Respostas = [];

        var questaoLP = CriarQuestaoLinguaPortuguesaSegundaLingua(1);
        var contextoAtual = new ContextoEducacional
        {
            TurmaId = dto.TurmaId,
            UeId = dto.UeId,
            DreId = dto.DreId,
            AnoLetivo = dto.AnoLetivo,
            AnoTurma = dto.AnoTurma,
            ModalidadeId = dto.ModalidadeId
        };
        var respostaExistente = new RespostaAluno(
            1,
            dto.Alunos[0].Codigo,
            questaoLP.Id,
            1,
            DateTime.UtcNow.AddDays(-1),
            contextoAtual)
        {
            Id = 99,
            AnoTurma = dto.AnoTurma
        };

        ConfigurarCenarioSalvar(questaoLP, [respostaExistente]);

        var resultado = await _useCase.SalvarOuAtualizarSondagemAsync(dto);

        Assert.True(resultado);
        _repositorioSondagemResposta.Verify(
            x => x.SalvarAsync(It.IsAny<List<RespostaAluno>>()),
            Times.Never);
    }

    [Fact]
    public async Task DeveIgnorarRespostaSemOpcao_QuandoNaoExistirRespostaAnterior()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "1";
        dto.Alunos = [dto.Alunos.First()];
        dto.Alunos[0].Respostas =
        [
            new RespostaSondagemDto
            {
                BimestreId = 1,
                QuestaoId = 3,
                OpcaoRespostaId = null
            }
        ];

        var questao = CriarQuestaoSondagem(1, 3);

        ConfigurarCenarioSalvarSemQuestaoLinguaPortuguesa(questao, []);

        var resultado = await _useCase.SalvarOuAtualizarSondagemAsync(dto);

        Assert.True(resultado);
        _repositorioSondagemResposta.Verify(
            x => x.SalvarAsync(It.IsAny<List<RespostaAluno>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeveLimparRespostaExistente_QuandoOpcaoRespostaForNula()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "1";
        dto.Alunos = [dto.Alunos.First()];
        dto.Alunos[0].Respostas =
        [
            new RespostaSondagemDto
            {
                BimestreId = 1,
                QuestaoId = 3,
                OpcaoRespostaId = null
            }
        ];

        var questao = CriarQuestaoSondagem(1, 3);
        var respostaExistente = new RespostaAluno(
            1,
            dto.Alunos[0].Codigo,
            questao.Id,
            4,
            DateTime.UtcNow.AddDays(-1),
            CriarContextoEducacional() with { TurmaId = dto.TurmaId, BimestreId = 1 })
        {
            Id = 99
        };

        ConfigurarCenarioSalvarSemQuestaoLinguaPortuguesa(questao, [respostaExistente]);

        List<RespostaAluno>? respostasSalvas = null;
        _repositorioSondagemResposta
            .Setup(x => x.SalvarAsync(
                It.IsAny<List<RespostaAluno>>(),
                It.IsAny<CancellationToken>()))
            .Callback<List<RespostaAluno>, CancellationToken>((respostas, _) => respostasSalvas = respostas)
            .ReturnsAsync(true);

        var resultado = await _useCase.SalvarOuAtualizarSondagemAsync(dto);

        Assert.True(resultado);
        var respostaSalva = Assert.Single(respostasSalvas!);
        Assert.Equal(99, respostaSalva.Id);
        Assert.Null(respostaSalva.OpcaoRespostaId);
    }

    [Fact]
    public async Task DeveConsultarRespostasPelaTurmaInformada()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "1";
        dto.Alunos = [dto.Alunos.First()];
        dto.Alunos[0].Respostas = [];

        var questaoLP = CriarQuestaoLinguaPortuguesaSegundaLingua(1);
        ConfigurarCenarioSalvar(questaoLP, []);

        _repositorioSondagemResposta
            .Setup(x => x.SalvarAsync(It.IsAny<List<RespostaAluno>>()))
            .ReturnsAsync(true);

        await _useCase.SalvarOuAtualizarSondagemAsync(dto);

        _repositorioSondagemResposta.Verify(x => x.ObterRespostasPorSondagemEAlunosAsync(
            dto.SondagemId,
            dto.TurmaId,
            It.IsAny<IEnumerable<int>>(),
            It.IsAny<IEnumerable<int>>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeveIgnorarAlunoRemanejadoNoSalvar()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "1";
        dto.Alunos = [dto.Alunos[0], dto.Alunos[1]];
        dto.Alunos[0].Respostas = [];
        dto.Alunos[1].Respostas = [];

        ConfigurarMockAlunosTurma([
            new AlunoElasticDto { CodigoAluno = dto.Alunos[0].Codigo, CodigoSituacaoMatricula = (int)SituacaoMatriculaAluno.Ativo },
            new AlunoElasticDto { CodigoAluno = dto.Alunos[1].Codigo, CodigoSituacaoMatricula = (int)SituacaoMatriculaAluno.RemanejadoSaida }
        ]);

        var questaoLP = CriarQuestaoLinguaPortuguesaSegundaLingua(1);
        ConfigurarCenarioSalvar(questaoLP, []);

        List<RespostaAluno>? respostasSalvas = null;
        _repositorioSondagemResposta
            .Setup(x => x.SalvarAsync(
                It.IsAny<List<RespostaAluno>>(),
                It.IsAny<CancellationToken>()))
            .Callback<List<RespostaAluno>, CancellationToken>((respostas, _) => respostasSalvas = respostas)
            .ReturnsAsync(true);

        var resultado = await _useCase.SalvarOuAtualizarSondagemAsync(dto);

        Assert.True(resultado);
        var resposta = Assert.Single(respostasSalvas!);
        Assert.Equal(dto.Alunos[0].Codigo, resposta.AlunoId);
    }

    [Fact]
    public async Task DeveRetornarExcecao_QuandoSemPermissao()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        dto.TurmaId = "1";

        _repositorioElasticTurma
               .Setup(r => r.ObterTurmaPorId(
                   It.IsAny<FiltroQuestionario>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(new TurmaElasticDto
               {
                   CodigoTurma = int.Parse(TURMA_ID),
                   CodigoEscola = CODIGO_ESCOLA_PERMITIDA
               });
        

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(dto.TurmaId.ToString()))
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
        var dreId = "1";
        _repositorioSondagemResposta
                .Setup(x => x.ObterExtracaoDadosRespostasAsync(modalidadeId, componenteCurricularId, dreId))
                .ReturnsAsync([]);

        _repositorioComponenteCurricular.Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Dominio.Entidades.ComponenteCurricular> { new Dominio.Entidades.ComponenteCurricular("NomeComponente",1,"Fundamental",2) });

        var filtro = new FiltroExtracaoDadosDto() {
            Modalidade = (Modalidade)modalidadeId
        };
        var uc = await _0bterSondagemRelatorioPorTodasTurmaUseCase.ObterSondagemRelatorio(filtro, _cancellationToken);
        Assert.Null(uc);
        Assert.Null(uc?.FileName);

    }

    [Fact]
    public async Task DeveRetornarListaDresComSucesso()
    {
        var dresMock = new List<ObterDresSgpDto>
    {
        new() { CodigoDre = "1", Nome = "DRE Centro" },
        new() { CodigoDre = "2", Nome = "DRE Sul" }
    };

        var jsonContent = JsonConvert.SerializeObject(dresMock);
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json")
        };

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var service = new ConsultaDeDresService(httpClientFactoryMock.Object, _servicoLog.Object);
        var resultado = await service.ObterDresSgpAsync();

        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
        Assert.Equal("1", resultado.First().CodigoDre);
        Assert.Equal("DRE Centro", resultado.First().Nome);
    }

    [Fact]
    public async Task DeveRetornarListaVaziaQuandoRequisicaoFalha()
    {
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.InternalServerError
        };

        httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new MockHttpMessageHandler(responseMessage)));

        var service = new ConsultaDeDresService(httpClientFactoryMock.Object, _servicoLog.Object);
        var resultado = await service.ObterDresSgpAsync();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task DeveRetornarListaVaziaQuandoExcecaoOcorre()
    {
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Throws(new HttpRequestException("Erro de conexão"));

        var service = new ConsultaDeDresService(httpClientFactoryMock.Object, _servicoLog.Object);
        var resultado = await service.ObterDresSgpAsync();

        Assert.Empty(resultado);
        _servicoLog.Verify(x => x.Registrar(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public void DeveThrowArgumentNullExceptionQuandoHttpClientFactoryNulo()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ConsultaDeDresService(null!, _servicoLog.Object));
    }

    [Fact]
    public void DeveThrowArgumentNullExceptionQuandoServicoLogNulo()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ConsultaDeDresService(_httpClientFactory.Object, null!));
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

    private static Questao CriarQuestaoSondagem(int questionarioId, int questaoId) =>
        new(
            questionarioId,
            1,
            "Questao sondagem",
            string.Empty,
            false,
            TipoQuestao.Radio,
            string.Empty,
            false,
            1)
        {
            Id = questaoId
        };

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

    private void ConfigurarCenarioSalvar(
        Questao questaoLinguaPortuguesa,
        IEnumerable<RespostaAluno> respostasExistentes)
    {
        ConfigurarMockTurmaSucesso();

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(SondagemMockData.CriarSondagemAtiva(1, 1));

        _dadosAlunosService
            .Setup(x => x.ObterDadosRacaGeneroAlunos(It.IsAny<int>()))
            .ReturnsAsync([]);

        _repositorioQuestao
            .Setup(x => x.ObterQuestionarioIdPorQuestoesAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([questaoLinguaPortuguesa]);

        _repositorioQuestao
            .Setup(x => x.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(
                It.IsAny<int>(),
                TipoQuestao.LinguaPortuguesaSegundaLingua))
            .ReturnsAsync(questaoLinguaPortuguesa);

        _repositorioSondagemResposta
            .Setup(x => x.ObterRespostasPorSondagemEAlunosAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostasExistentes);
    }

    private void ConfigurarCenarioSalvarSemQuestaoLinguaPortuguesa(
        Questao questao,
        IEnumerable<RespostaAluno> respostasExistentes)
    {
        ConfigurarMockTurmaSucesso();

        _controleAcessoService
            .Setup(x => x.ValidarPermissaoAcessoAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(SondagemMockData.CriarSondagemAtiva(1, 1));

        _dadosAlunosService
            .Setup(x => x.ObterDadosRacaGeneroAlunos(It.IsAny<int>()))
            .ReturnsAsync([]);

        _repositorioQuestao
            .Setup(x => x.ObterQuestionarioIdPorQuestoesAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([questao]);

        _repositorioQuestao
            .Setup(x => x.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(
                It.IsAny<int>(),
                TipoQuestao.LinguaPortuguesaSegundaLingua))
            .ReturnsAsync((Questao?)null);

        _repositorioSondagemResposta
            .Setup(x => x.ObterRespostasPorSondagemEAlunosAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(respostasExistentes);
    }

    private static ContextoEducacional CriarContextoEducacional()
    {
        return new ContextoEducacional
        {
            TurmaId = "1",
            UeId = "3",
            DreId = "2",
            AnoLetivo = 2026,
            ModalidadeId = 4,
            RacaCorId = 1,
            GeneroSexoId = 1,
            BimestreId = 2
        };
    }
}

