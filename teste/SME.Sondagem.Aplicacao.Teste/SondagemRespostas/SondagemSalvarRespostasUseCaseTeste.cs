using Moq;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Exceptions;
using SME.Sondagem.Infra.Teste.DTO;
using SME.Sondagem.Infrastructure.Dtos.Sondagem;
using Xunit;

namespace SME.Sondagem.Aplicacao.Teste.SondagemRespostas;

public class SondagemSalvarRespostasUseCaseTeste
{
    private readonly Mock<IRepositorioSondagem> _repositorioSondagem;
    private readonly Mock<IRepositorioRespostaAluno> _repositorioSondagemResposta;
    private readonly Mock<IRepositorioQuestao> _repositorioQuestao;
    private readonly SondagemSalvarRespostasUseCase _useCase;

    public SondagemSalvarRespostasUseCaseTeste()
    {
        _repositorioSondagem = new Mock<IRepositorioSondagem>();
        _repositorioSondagemResposta = new Mock<IRepositorioRespostaAluno>();
        _repositorioQuestao = new Mock<IRepositorioQuestao>(); 
        _useCase = new SondagemSalvarRespostasUseCase(_repositorioSondagem.Object, _repositorioSondagemResposta.Object, _repositorioQuestao.Object);
    }

    [Fact]
    public async Task SalvarOuAtualizarSondagemAsync_DeveRetornarNegocioException_QuandoNenhumaSondagemAtivaEncontrada()
    {
        var dto = SondagemMockData.ObterSondagemMock();

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())!
            .ReturnsAsync((Dominio.Entidades.Sondagem.Sondagem?)null);

        var exception = await Assert.ThrowsAsync<NegocioException>(() => _useCase.SalvarOuAtualizarSondagemAsync(dto));

        Assert.Equal(MensagemNegocioComuns.NENHUM_SONDAGEM_ATIVA_ENCONRADA, exception.Message);

        _repositorioSondagem.Verify(x => x.ObterSondagemAtiva(), Times.Once);
        _repositorioSondagemResposta.Verify(
            x => x.ObterRespostasPorSondagemEAlunosAsync(It.IsAny<int>(), It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<int>>()),
            Times.Never);
        _repositorioSondagemResposta.Verify(
            x => x.SalvarAsync(It.IsAny<List<RespostaAluno>>()),
            Times.Never);
    }

    [Fact]
    public async Task
        SalvarOuAtualizarSondagemAsync_DeveRetornarNegocioException_QuandoSondagemIdDiferenteDaSondagemAtiva()
    {
        var dto = SondagemMockData.ObterSondagemMock();
        var sondagemAtiva = SondagemMockData.CriarSondagemAtiva(2);

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(sondagemAtiva);

        var exception = await Assert.ThrowsAsync<NegocioException>(() => _useCase.SalvarOuAtualizarSondagemAsync(dto));

        Assert.Equal(MensagemNegocioComuns.SALVAR_SOMENTE_PARA_SONDAGEM_ATIVA, exception.Message);

        _repositorioSondagem.Verify(x => x.ObterSondagemAtiva(), Times.Once);
        _repositorioSondagemResposta.Verify(
            x => x.ObterRespostasPorSondagemEAlunosAsync(It.IsAny<int>(), It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<int>>()),
            Times.Never);
        _repositorioSondagemResposta.Verify(
            x => x.SalvarAsync(It.IsAny<List<RespostaAluno>>()),
            Times.Never);
    }

    [Fact]
    public async Task SalvarOuAtualizarSondagemAsync_DeveSalvarComSucesso_QuandoDadosValidos()
    {
        var sondagemId = 1;
        var bimestreId = 1;
        var questionarioId = 1;

        var dto = SondagemMockData.ObterSondagemMock();

        var sondagemAtiva = SondagemMockData.CriarSondagemAtiva(
            sondagemId,
            bimestreId);

        var respostasExistentes = new List<RespostaAluno>();

        var questaoLinguaPortuguesa = CriarQuestaoLinguaPortuguesaSegundaLingua(questionarioId);

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(sondagemAtiva);

        _repositorioQuestao
            .Setup(x => x.ObterQuestionarioIdPorQuestoesAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Questao> { questaoLinguaPortuguesa });

        _repositorioQuestao
            .Setup(x => x.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(
                It.IsAny<int>(),
                TipoQuestao.LinguaPortuguesaSegundaLingua,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(questaoLinguaPortuguesa);

        _repositorioSondagemResposta
            .Setup(x => x.ObterRespostasPorSondagemEAlunosAsync(
                It.IsAny<int>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(respostasExistentes);

        _repositorioSondagemResposta
            .Setup(x => x.SalvarAsync(It.IsAny<List<RespostaAluno>>()))
            .ReturnsAsync(true);

        var resultado = await _useCase.SalvarOuAtualizarSondagemAsync(dto);

        Assert.True(resultado);

        _repositorioSondagem.Verify(x => x.ObterSondagemAtiva(), Times.Once);

        _repositorioSondagemResposta.Verify(
            x => x.ObterRespostasPorSondagemEAlunosAsync(
                It.IsAny<int>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<int>>()),
            Times.Once);

        _repositorioSondagemResposta.Verify(
            x => x.SalvarAsync(It.Is<List<RespostaAluno>>(respostas =>
                respostas.Count == 12 &&
                respostas.All(r => r.SondagemId == sondagemId))),
            Times.Once);
    }


    [Fact]
    public async Task SalvarOuAtualizarSondagemAsync_DeveAtualizarRespostaExistente_QuandoRespostaJaExiste()
    {
        const int sondagemId = 1;
        const int alunoId = 101;
        const int questaoId = 1;
        const int bimestreId = 1;
        const int questionarioId = 1;
        const int opcaoRespostaIdAntiga = 2;
        const int opcaoRespostaIdNova = 3;

        var dto = SondagemMockData.ObterSondagemMock();

        var sondagem = new Dominio.Entidades.Sondagem.Sondagem("SondagemAtiva", DateTime.Now)
        {
            Id = sondagemId
        };

        var periodosBimestre = new List<SondagemPeriodoBimestre>
        {
            new(
                sondagemId,
                bimestreId,
                DateTime.Now.AddDays(-1),
                DateTime.Now.AddDays(1))
        };
        periodosBimestre.ForEach(s => sondagem.PeriodosBimestre.Add(s));

        var questaoLinguaPortuguesa = CriarQuestaoLinguaPortuguesaSegundaLingua(questionarioId);

        var respostaExistente = new RespostaAluno(
            sondagemId,
            alunoId,
            questaoId,
            opcaoRespostaIdAntiga,
            DateTime.UtcNow.AddDays(-1),
            bimestreId);

        var respostasExistentes = new List<RespostaAluno>
        {
            respostaExistente
        };

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(sondagem);

        _repositorioQuestao
            .Setup(x => x.ObterQuestionarioIdPorQuestoesAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Questao> { questaoLinguaPortuguesa });

        _repositorioQuestao
            .Setup(x => x.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(
                It.IsAny<int>(),
                TipoQuestao.LinguaPortuguesaSegundaLingua,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(questaoLinguaPortuguesa);

        _repositorioSondagemResposta
            .Setup(x => x.ObterRespostasPorSondagemEAlunosAsync(
                It.IsAny<int>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(respostasExistentes);

        _repositorioSondagemResposta
            .Setup(x => x.SalvarAsync(It.IsAny<List<RespostaAluno>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _useCase.SalvarOuAtualizarSondagemAsync(dto);

        Assert.True(resultado);

        _repositorioSondagemResposta.Verify(
            x => x.SalvarAsync(
                It.Is<List<RespostaAluno>>(respostas =>
                    respostas.Count == 12 &&
                    respostas.Any(r =>
                        r.AlunoId == alunoId &&
                        r.QuestaoId == questaoId &&
                        r.SondagemId == sondagemId &&
                        r.BimestreId == bimestreId &&
                        r.OpcaoRespostaId == opcaoRespostaIdNova)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task SalvarOuAtualizarSondagemAsync_DeveProcessarMultiplosAlunos_QuandoDtoContemVariosAlunos()
    {
        var sondagemId = 1;
        var bimestreId = 1;
        var questionarioId = 1;

        var dto = new SondagemSalvarDto
        {
            SondagemId = sondagemId,
            Alunos = new List<AlunoSondagemDto>
            {
                new()
                {
                    Codigo = 100,
                    Respostas = new List<RespostaSondagemDto>
                    {
                        new()
                        {
                            QuestaoId = 10,
                            OpcaoRespostaId = 5,
                            BimestreId = bimestreId
                        }
                    }
                },
                new()
                {
                    Codigo = 200,
                    Respostas = new List<RespostaSondagemDto>
                    {
                        new()
                        {
                            QuestaoId = 10,
                            OpcaoRespostaId = 6,
                            BimestreId = bimestreId
                        }
                    }
                }
            }
        };

        var sondagemAtiva = SondagemMockData.CriarSondagemAtiva(sondagemId, bimestreId);
        var questaoLinguaPortuguesa = CriarQuestaoLinguaPortuguesaSegundaLingua(questionarioId);

        _repositorioSondagem
            .Setup(x => x.ObterSondagemAtiva())
            .ReturnsAsync(sondagemAtiva);

        _repositorioQuestao
            .Setup(x => x.ObterQuestionarioIdPorQuestoesAsync(
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Questao> { questaoLinguaPortuguesa });

        _repositorioQuestao
            .Setup(x => x.ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(
                It.IsAny<int>(),
                TipoQuestao.LinguaPortuguesaSegundaLingua,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(questaoLinguaPortuguesa);

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

        _repositorioSondagemResposta.Verify(
            x => x.SalvarAsync(It.Is<List<RespostaAluno>>(respostas =>
                respostas.Count == 4)),
            Times.Once);
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
            null            
        )
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