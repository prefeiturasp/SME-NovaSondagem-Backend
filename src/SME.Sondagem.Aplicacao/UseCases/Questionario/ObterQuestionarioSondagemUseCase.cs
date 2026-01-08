using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class ObterQuestionarioSondagemUseCase : IObterQuestionarioSondagemUseCase
{
    private readonly IRepositorioElasticTurma _repositorioElasticTurma;
    private readonly IRepositorioElasticAluno _repositorioElasticAluno;
    private readonly IRepositorioCiclo _repositorioCiclo;
    private readonly IRepositorioOpcaoResposta _repositorioOpcaoResposta;
    private readonly IRepositorioRespostaAluno _repositorioRespostaAluno;
    private readonly IAlunoPapService _alunoPapService;

    public ObterQuestionarioSondagemUseCase(
        IRepositorioElasticTurma repositorioElasticTurma,
        IRepositorioElasticAluno repositorioElasticAluno,
        IRepositorioOpcaoResposta repositorioOpcaoResposta,
        IRepositorioRespostaAluno repositorioRespostaAluno,
        IAlunoPapService alunoPapService)
    {
        this._repositorioElasticTurma = repositorioElasticTurma ?? throw new ArgumentNullException(nameof(repositorioElasticTurma));
        this._repositorioElasticAluno = repositorioElasticAluno ?? throw new ArgumentNullException(nameof(repositorioElasticAluno));
        this._repositorioOpcaoResposta = repositorioOpcaoResposta ?? throw new ArgumentNullException(nameof(repositorioOpcaoResposta));
        this._repositorioRespostaAluno = repositorioRespostaAluno ?? throw new ArgumentNullException(nameof(repositorioRespostaAluno));
        this._alunoPapService = alunoPapService ?? throw new ArgumentNullException(nameof(alunoPapService));
    }

    public async Task<QuestionarioSondagemDto> ObterQuestionarioSondagem([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        if (filtro == null)
            throw new ArgumentNullException(nameof(filtro));

        var turma = await _repositorioElasticTurma.ObterTurmaPorId(filtro, cancellationToken);

        if (turma == null)
            throw new ErroInternoException("Turma não localizada");

        if (filtro.Modalidade == 0)
            throw new RegraNegocioException("A modalidade é obrigatória no filtro");

        if (filtro.ProficienciaId == 0)
            throw new RegraNegocioException("A proficiência é obrigatória no filtro");

        if (turma.Modalidade != filtro.Modalidade)
            throw new RegraNegocioException("A modalidade do filtro não corresponde a esta turma");

        if (string.IsNullOrWhiteSpace(turma.SerieEnsino))
            throw new ErroInternoException("Série de ensino da turma não informada");

        if (turma.SerieEnsino != filtro.Ano.ToString())
            throw new RegraNegocioException("A série ano do filtro não corresponde a esta turma");

        if (filtro.Modalidade == (int)Modalidade.Fundamental || filtro.Modalidade == (int)Modalidade.EJA)
        {
            if (filtro.Ano == 1 || filtro.Ano == 2 || filtro.Ano == 3)
            {
                var alunos = await _repositorioElasticAluno.ObterAlunosPorIdTurma(filtro.TurmaId, cancellationToken);

                if (alunos == null || !alunos.Any())
                    throw new ErroInternoException("Não há alunos cadastrados para a turma informada");

                var opcoesResposta = await _repositorioOpcaoResposta.ObterTodosAsync(cancellationToken);
                
                if (opcoesResposta == null)
                    throw new ErroInternoException("Não foi possível obter as opções de resposta");

                var opcoesRespostaDto = opcoesResposta.Select(op => new OpcaoRespostaDto
                {
                    DescricaoOpcaoResposta = op?.DescricaoOpcaoResposta ?? string.Empty,
                    Legenda = op?.Legenda ?? string.Empty,
                    CorFundo = op?.CorFundo ?? string.Empty,
                    CorTexto = op?.CorTexto ?? string.Empty
                }).ToList();

                var colunas = ObterColunasCiclos(opcoesRespostaDto);
                
                if (colunas == null || !colunas.Any())
                    throw new ErroInternoException("Não foi possível obter as colunas dos ciclos");

                var ciclosIds = colunas.Select(c => c.IdCiclo).ToList();

                var codigosAlunos = alunos.Select(a => a.CodigoAluno).ToList();

                var alunosComPap = await _alunoPapService.VerificarAlunosPossuemProgramaPapAsync(
                    codigosAlunos, 
                    turma.AnoLetivo, 
                    cancellationToken);

                var alunosComLinguaPortuguesaSegundaLingua = await _repositorioRespostaAluno
                    .VerificarAlunosTemRespostaPorTipoQuestaoAsync(
                        codigosAlunos, 
                        TipoQuestao.LinguaPortuguesaSegundaLingua, 
                        cancellationToken);

                // Buscar todas as respostas dos alunos para os ciclos
                var respostasAlunos = await _repositorioRespostaAluno.ObterRespostasAlunosPorCiclosAsync(
                    codigosAlunos,
                    ciclosIds,
                    filtro.ProficienciaId,
                    filtro.Ano,
                    cancellationToken);

                var estudantes = new List<EstudanteQuestionarioDto>();
                
                foreach (var aluno in alunos)
                {
                    var colunasAluno = colunas.Select(c =>
                    {
                        RespostaDto? respostaDto = null;
                        
                        if (respostasAlunos.TryGetValue((aluno.CodigoAluno, c.IdCiclo), out var resposta))
                        {
                            respostaDto = new RespostaDto
                            {
                                Id = resposta.Id,
                                OpcaoRespostaId = resposta.OpcaoRespostaId
                            };
                        }
                        
                        return new ColunaQuestionarioDto
                        {
                            IdCiclo = c.IdCiclo,
                            DescricaoColuna = c.DescricaoColuna,
                            PeriodoBimestreAtivo = c.PeriodoBimestreAtivo,
                            OpcaoResposta = c.OpcaoResposta,
                            Resposta = respostaDto ?? new RespostaDto()
                        };
                    }).ToList();

                    estudantes.Add(new EstudanteQuestionarioDto
                    {
                        NumeroAlunoChamada = aluno.NumeroAlunoChamada,
                        Codigo = aluno.CodigoAluno,
                        Nome = aluno.NomeAluno ?? string.Empty,
                        LinguaPortuguesaSegundaLingua = alunosComLinguaPortuguesaSegundaLingua.GetValueOrDefault(aluno.CodigoAluno, false),
                        Pap = alunosComPap.GetValueOrDefault(aluno.CodigoAluno, false),
                        //  Aee = aluno.PossuiAee,
                        PossuiDeficiencia = aluno.PossuiDeficiencia == 1,
                        Coluna = colunasAluno
                    });
                }

                var questionario = new QuestionarioSondagemDto
                {
                    TituloTabelaRespostas = ObterTituloTabelaRespostas(filtro.ProficienciaId, filtro.Ano),
                    Estudantes = estudantes,
                };

                return questionario;
            }
            else
            {
                throw new ErroInternoException("Não há questionário para a série informada");
            }
        }
        else
            throw new ErroInternoException("Não há questionário para a modalidade informada");
    }

    private string ObterTituloTabelaRespostas(int proficienciaId, int ano)
    {
        return proficienciaId switch
        {
            (int)Dominio.Enums.Proficiencia.Escrita when ano == 1 => "Sistema de escrita",
            (int)Dominio.Enums.Proficiencia.Escrita when ano == 2 => "Reescrita",
            (int)Dominio.Enums.Proficiencia.Escrita when ano == 3 => "Produção",
            (int)Dominio.Enums.Proficiencia.Leitura when ano >= 1 && ano <= 3 => "Compreensão de textos",
            _ => string.Empty
        };
    }

    private List<ColunaQuestionarioDto> ObterColunasCiclos(List<OpcaoRespostaDto> opcoesResposta)
    {
        if (opcoesResposta == null)
            throw new ArgumentNullException(nameof(opcoesResposta));

        // TODO: Implementar lógica para determinar qual período está ativo baseado em datas do backend
        var periodoAtivo = ObterPeriodoAtivo();

        return new List<ColunaQuestionarioDto>
        {
            new ColunaQuestionarioDto
            {
                IdCiclo = 1,
                DescricaoColuna = "Sondagem inicial",
                PeriodoBimestreAtivo = periodoAtivo == 0,
                OpcaoResposta = opcoesResposta
            },
            new ColunaQuestionarioDto
            {
                IdCiclo = 2,
                DescricaoColuna = "1º bimestre",
                PeriodoBimestreAtivo = periodoAtivo == 1,
                OpcaoResposta = opcoesResposta
            },
            new ColunaQuestionarioDto
            {
                IdCiclo = 3,
                DescricaoColuna = "2º bimestre",
                PeriodoBimestreAtivo = periodoAtivo == 2,
                OpcaoResposta = opcoesResposta
            },
            new ColunaQuestionarioDto
            {
                IdCiclo = 4,
                DescricaoColuna = "3º bimestre",
                PeriodoBimestreAtivo = periodoAtivo == 3,
                OpcaoResposta = opcoesResposta
            },
            new ColunaQuestionarioDto
            {
                IdCiclo = 5,
                DescricaoColuna = "4º bimestre",
                PeriodoBimestreAtivo = periodoAtivo == 4,
                OpcaoResposta = opcoesResposta
            }
        };
    }

    private int ObterPeriodoAtivo()
    {
        // TODO: Implementar lógica real baseada em dados do backend (datas de início/fim dos períodos)
        // Por enquanto, retornando período fixo para exemplo
        // 0 = Sondagem inicial, 1 = 1º bimestre, 2 = 2º bimestre, 3 = 3º bimestre, 4 = 4º bimestre
        return 0; // Sondagem inicial ativa por padrão
    }
}
