using MediatR;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Base;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Queries.Bimestre;
using SME.Sondagem.Aplicacao.Queries.ComponenteCurricular;
using SME.Sondagem.Aplicacao.Queries.Proficiencia;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Base;

public abstract class QuestionarioSondagemUseCaseBase : IQuestionarioSondagemUseCaseBase
{
    protected readonly RepositoriosElastic _repositoriosElastic;
    protected readonly RepositoriosSondagem _repositoriosSondagem;
    protected readonly IAlunoPapService _alunoPapService;
    protected readonly IControleAcessoService _controleAcessoService;
    protected readonly IServicoUsuario _servicoUsuario;
    private readonly IMediator _mediator;

    protected QuestionarioSondagemUseCaseBase(
        RepositoriosElastic repositoriosElastic,
        RepositoriosSondagem repositoriosSondagem,
        IAlunoPapService alunoPapService,
        IControleAcessoService controleAcessoService,
        IServicoUsuario servicoUsuario,
        IMediator mediator)
    {
        _repositoriosElastic = repositoriosElastic ?? throw new ArgumentNullException(nameof(repositoriosElastic));
        _repositoriosSondagem = repositoriosSondagem ?? throw new ArgumentNullException(nameof(repositoriosSondagem));
        _alunoPapService = alunoPapService ?? throw new ArgumentNullException(nameof(alunoPapService));
        _controleAcessoService = controleAcessoService ?? throw new ArgumentNullException(nameof(controleAcessoService));
        _servicoUsuario = servicoUsuario ?? throw new ArgumentNullException(nameof(servicoUsuario));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<object> ExecutarProcessamentoQuestionario(
        FiltroQuestionario filtro,
        bool ehRelatorio,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filtro);
        
        var ehTodosBimestreOuInicial = EhBimestreTodos(filtro.BimestreId);
        var ehMatematicaOuLinguaPortuguesa = await EhMatematicaOuLinguaPortuguesa(filtro.ComponenteCurricularId);
        var ehProficienciaLeitura = await EhProficienciaLeituraMapSaberes(filtro.ProficienciaId);
        var exibirBimestreNaDescricaoColuna = ehTodosBimestreOuInicial && ehMatematicaOuLinguaPortuguesa && ehProficienciaLeitura;

        var turma = await ValidarFiltroEModalidade(filtro, cancellationToken);
        var sondagemAtiva = await ObterSondagemAtiva(cancellationToken);

        return await ProcessarQuestionario(filtro, turma, sondagemAtiva, ehRelatorio, exibirBimestreNaDescricaoColuna, cancellationToken);
    }

    protected async Task<TurmaElasticDto> ValidarFiltroEModalidade(FiltroQuestionario filtro, CancellationToken cancellationToken)
    {
        var turma = await _repositoriosElastic.RepositorioElasticTurma.ObterTurmaPorId(filtro, cancellationToken)
            ?? throw new RegraNegocioException(MensagemNegocioComuns.TURMA_NAO_LOCALIZADA, 400);

        if (filtro.ProficienciaId == 0)
            throw new RegraNegocioException(MensagemNegocioComuns.PROFICIENCIA_OBRIGATORIA_NO_FILTRO, 400);

        return turma;
    }

    protected async Task<Dominio.Entidades.Sondagem.Sondagem> ObterSondagemAtiva(CancellationToken cancellationToken)
    {
        return await _repositoriosSondagem.RepositorioSondagem.ObterSondagemAtiva(cancellationToken)
            ?? throw new ErroInternoException(MensagemNegocioComuns.SONDAGEM_ATIVA_NAO_CADASTRADA);
    }

    private async Task<object> ProcessarQuestionario(
        FiltroQuestionario filtro,
        TurmaElasticDto turma,
        Dominio.Entidades.Sondagem.Sondagem sondagemAtiva,
        bool ehRelatorio,bool exibirBimestreNaDescricaoColuna,
        CancellationToken cancellationToken)
    {
        var contextoProcessamento = await ConstruirContextoProcessamento(filtro, turma, sondagemAtiva, ehRelatorio, cancellationToken);

        var dadosAlunos = await ObterDadosAlunos(filtro.TurmaId, turma.AnoLetivo, contextoProcessamento, cancellationToken);

        var linguaPortuguesaSegundaLingua = contextoProcessamento.QuestaoLinguaPortuguesa;

        var respostasProcessadas = ProcessarRespostas(
            contextoProcessamento.RespostasAlunosPorQuestoes,
            linguaPortuguesaSegundaLingua!,
            contextoProcessamento.Alunos,
            sondagemAtiva.PeriodosBimestre.OrderBy(c => c.DataInicio).FirstOrDefault()!.DataInicio
        );

        var estudantes = await ConstruirEstudantes(dadosAlunos, sondagemAtiva, contextoProcessamento, respostasProcessadas, ehRelatorio, exibirBimestreNaDescricaoColuna);

        var legenda = ConstruirLegenda(contextoProcessamento);

        var questaoId = contextoProcessamento.QuestoesAtivas
            .FirstOrDefault(x => x.Tipo != TipoQuestao.LinguaPortuguesaSegundaLingua)?.Id ?? 0;

        var tituloTabelaRespostas = ObterTituloTabelaRespostas(contextoProcessamento.QuestoesAtivas);
        if (!ehRelatorio)
        {
            return new QuestionarioSondagemDto
            {
                QuestaoId = questaoId,
                SondagemId = sondagemAtiva.Id,
                TituloTabelaRespostas = tituloTabelaRespostas,
                PodeSalvar = await _controleAcessoService.ValidarPermissaoAcessoAsync(turma.CodigoTurma.ToString(), cancellationToken),
                Estudantes = estudantes.OrderBy(e => e.Nome).ToList(),
                InseridoPor = respostasProcessadas.InseridoPor,
                AlteradoPor = respostasProcessadas.AlteradoPor,
                QuestionarioId = contextoProcessamento.QuestionarioId
            };
        }
        else
        {
            return new QuestionarioSondagemRelatorioDto
            {
                TituloTabelaRespostas = tituloTabelaRespostas,
                Semestre = turma.Semestre.ToString() + "º semestre",
                Estudantes = estudantes.OrderBy(e => e.Nome).ToList(),
                Legenda = legenda,
                QuestionarioId = contextoProcessamento.QuestionarioId
            };
        }
    }

    private async Task<ContextoProcessamentoDto> ConstruirContextoProcessamento(
        FiltroQuestionario filtro,
        TurmaElasticDto turma,
        Dominio.Entidades.Sondagem.Sondagem sondagemAtiva,
        bool ehRelatorio,
        CancellationToken cancellationToken)
    {
        var modalidade = turma.Modalidade;
        int ano = int.TryParse(turma.AnoTurma, out int anoTurma) ? anoTurma : filtro.Ano;
        int anoLetivo = filtro.AnoLetivo != 0 ? filtro.AnoLetivo : turma.AnoLetivo;

        ValidarModalidadeEAno(modalidade, ano);

        var questoesAtivas = await ObterQuestoesAtivasOuLancarExcecao(modalidade, ano, anoLetivo, filtro.ProficienciaId, cancellationToken);

        var questaoLinguaPortuguesa = questoesAtivas.FirstOrDefault(x => x.Tipo == TipoQuestao.LinguaPortuguesaSegundaLingua);
        var questoesIds = ObterQuestoesIdsPorTipo(questoesAtivas);

        var bimestresForaDoPadrao = await _repositoriosSondagem.RepositorioBimestre
            .ObterBimestresPorQuestionarioIdAsync(ObterIdQuestionario(questoesAtivas), cancellationToken);

        var periodosBimestre = bimestresForaDoPadrao?.Count > 0 ? bimestresForaDoPadrao : sondagemAtiva.PeriodosBimestre;

        var colunas = await ObterColunasOuLancarExcecao(
            ehRelatorio && filtro.BimestreId.HasValue
                ? periodosBimestre.Where(p => p.BimestreId == filtro.BimestreId.Value).ToList()
                : periodosBimestre,
            questoesAtivas,
            filtro.BimestreId
        );

        var alunos = await ObterAlunosOuLancarExcecao(filtro.TurmaId, anoLetivo, cancellationToken);

        var codigosAlunos = alunos.Select(a => a.CodigoAluno).ToList();

        var respostasAlunosPorQuestoes = await _repositoriosSondagem.RepositorioRespostaAluno.ObterRespostasAlunosPorQuestoesAsync(
            codigosAlunos.Select(x => (long)x).ToList(),
            questoesIds.Select(x => (long)x).ToList(),
            sondagemAtiva.Id,
            cancellationToken);

        var questaoIdPrincipal = questoesAtivas.First(x => x.Tipo != TipoQuestao.LinguaPortuguesaSegundaLingua).Id;

        return new ContextoProcessamentoDto
        {
            QuestoesAtivas = questoesAtivas,
            QuestaoLinguaPortuguesa = questaoLinguaPortuguesa,
            Colunas = colunas,
            Alunos = alunos,
            CodigosAlunos = codigosAlunos,
            RespostasAlunosPorQuestoes = respostasAlunosPorQuestoes.ToDictionary(
                x => (x.Key.CodigoAluno, x.Key.BimestreId, x.Key.QuestaoId),
                x => x.Value
            ),
            QuestaoIdPrincipal = questaoIdPrincipal,
            QuestionarioId = questoesAtivas.FirstOrDefault()?.QuestionarioId
        };
    }

    protected abstract Task<DadosAlunosDto> ObterDadosAlunos(
        int turmaId,
        int anoLetivo,
        ContextoProcessamentoDto contexto,
        CancellationToken cancellationToken);

    private async Task<List<EstudanteQuestionarioDto>> ConstruirEstudantes(
    DadosAlunosDto dadosAlunos,
    Dominio.Entidades.Sondagem.Sondagem sondagemAtiva,
    ContextoProcessamentoDto contexto,
    RespostasProcessadasDto respostasProcessadas,
    bool ehRelatorio,
    bool exibirBimestreNaDescricaoColuna)
    {
        var descricoesBimestre = new Dictionary<int, string>();
        if (exibirBimestreNaDescricaoColuna)
        {
            var bimestreIds = contexto.Colunas
                .Where(c => c.IdCiclo != 0)
                .Select(c => c.IdCiclo)
                .Distinct();

            foreach (var id in bimestreIds)
            {
                var bimestre = await _mediator.Send(new ObterBimestrePorIdQuery(id));
                descricoesBimestre[id] = bimestre?.Descricao ?? string.Empty;
            }
        }

        var contextoColunaDto = new ContextoColunaDto(
            sondagemAtiva,
            contexto.QuestaoIdPrincipal,
            exibirBimestreNaDescricaoColuna,
            respostasProcessadas.RespostasConvertidas,
            descricoesBimestre,
            ehRelatorio
        );

        var estudantes = new List<EstudanteQuestionarioDto>();

        foreach (var aluno in contexto.Alunos)
        {
            var colunasAluno = contexto.Colunas
                .Select(c => ConstruirColunaAluno(c, aluno, contextoColunaDto))
                .ToList();

            var estudante = await ConstruirEstudante(
                aluno, dadosAlunos,
                [.. colunasAluno.OrderBy(x => x.BimestreId)],
                aluno.CodigoAluno);

            estudantes.Add(estudante);
        }

        return estudantes;
    }

    private static List<LegendaQuestionarioDto> ConstruirLegenda(
        ContextoProcessamentoDto contexto)
    {
        var descricoesExcluidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Sim", "Não", "Nao"
        };

        var todasOpcoesResposta = contexto.QuestoesAtivas
            .Where(q => q.Tipo != TipoQuestao.LinguaPortuguesaSegundaLingua)
            .SelectMany(q => q.QuestaoOpcoes)
            .Where(qo => !descricoesExcluidas.Contains(qo.OpcaoResposta.DescricaoOpcaoResposta?.Trim() ?? ""))
            .Select(qo => qo.OpcaoResposta)
            .DistinctBy(or => or.Id)
            .OrderBy(l => l.Ordem)
            .ThenBy(l => l.Id)
            .ToList();

        return todasOpcoesResposta
            .Select(opcao => new LegendaQuestionarioDto
            {
                Id = opcao.Id,
                Ordem = opcao.Ordem,
                DescricaoOpcaoResposta = opcao.DescricaoOpcaoResposta,
                Legenda = opcao.Legenda,
                CorFundo = opcao.CorFundo,
                CorTexto = opcao.CorTexto
            }).ToList();
    }

    protected virtual Task<EstudanteQuestionarioDto> ConstruirEstudante(
        AlunoElasticDto aluno,
        DadosAlunosDto dadosAlunos,
        List<ColunaQuestionarioDto> colunasAluno,
        int codigoAluno)
    {
        var estudante = new EstudanteQuestionarioDto
        {
            NumeroAlunoChamada = aluno.NumeroAlunoChamada,
            Codigo = codigoAluno,
            Nome = aluno.NomeAluno ?? string.Empty,
            NomeRelatorio = (aluno.NomeAluno + " (" + codigoAluno + ")") ?? string.Empty,
            LinguaPortuguesaSegundaLingua = dadosAlunos.AlunosComLinguaPortuguesaSegundaLingua.TryGetValue(codigoAluno, out var lingua) && lingua,
            Pap = dadosAlunos.AlunosComPap.TryGetValue(codigoAluno, out var pap) && pap,
            PossuiDeficiencia = aluno.PossuiDeficiencia == 1,
            Coluna = colunasAluno
        };

        if (dadosAlunos.DadosRacaGenero?.TryGetValue((long)aluno.CodigoAluno, out var dados) == true)
        {
            estudante.Raca = dados.Raca;
            estudante.Genero = dados.Sexo;
        }

        return Task.FromResult(estudante);
    }

    protected static void ValidarModalidadeEAno(int modalidade, int ano)
    {
        if (modalidade != (int)Modalidade.Fundamental && modalidade != (int)Modalidade.EJA)
            throw new ErroNaoEncontradoException(MensagemNegocioComuns.MODALIDADE_SEM_QUESTIONARIO);

        if (ano is < 1 or > 3)
            throw new ErroNaoEncontradoException(MensagemNegocioComuns.SERIE_SEM_QUESTIONARIO);
    }

    protected async Task<IEnumerable<Dominio.Entidades.Questionario.Questao>> ObterQuestoesAtivasOuLancarExcecao(
        int modalidade,
        int ano,
        int anoLetivo,
        int proficienciaId,
        CancellationToken cancellationToken)
    {
        var questoesAtivas = await _repositoriosSondagem.RepositorioQuestao.ObterQuestoesAtivasPorFiltroAsync(
            modalidade,
            anoLetivo,
            proficienciaId,
            ano,
            cancellationToken);

        return questoesAtivas?.Any() == true
            ? questoesAtivas
            : throw new ErroNaoEncontradoException(MensagemNegocioComuns.QUESTOES_ATIVAS_NAO_ENCONTRADAS);
    }

    protected static List<int> ObterQuestoesIdsPorTipo(IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        return questoesAtivas.Select(q => q.Id).ToList();
    }

    protected static int ObterIdQuestionario(IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        return questoesAtivas.FirstOrDefault()?.QuestionarioId ?? 0;
    }

    protected async Task<IEnumerable<AlunoElasticDto>> ObterAlunosOuLancarExcecao(int turmaId, int anoLetivo, CancellationToken cancellationToken)
    {
        var alunos = await _repositoriosElastic.RepositorioElasticAluno.ObterAlunosPorIdTurma(turmaId, anoLetivo, cancellationToken);

        return alunos?.Any() == true
            ? alunos
            : throw new ErroNaoEncontradoException(MensagemNegocioComuns.ALUNOS_NAO_CADASTRADOS_TURMA);
    }

    protected static Task<List<ColunaQuestionarioDto>> ObterColunasOuLancarExcecao(
        ICollection<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre> periodosBimestre,
        IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas,
        int? bimestreId)
    {
        var possuiSubperguntas = PossuiQuestaoVinculo(questoesAtivas);

        if (possuiSubperguntas)
        {
            return bimestreId.HasValue
                ? ObterColunasPorSubperguntas(periodosBimestre, questoesAtivas, bimestreId.Value)
                : ObterColunasPorSubperguntasParaTodosBimestres(periodosBimestre, questoesAtivas);
        }

        return ObterColunasPorBimestres(periodosBimestre, questoesAtivas);
    }

    private static Task<List<ColunaQuestionarioDto>> ObterColunasPorSubperguntasParaTodosBimestres(
        ICollection<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre> periodosBimestre,
        IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        var agora = DateTime.Now;

        var colunas = periodosBimestre
            .Where(p => !p.Excluido)
            .OrderBy(p => p.DataInicio)
            .SelectMany(p =>
            {
                var periodoAtivo = agora >= p.DataInicio && agora <= p.DataFim;

                return questoesAtivas
                    .Where(q => !q.Excluido && q.Tipo != TipoQuestao.QuestaoComSubpergunta)
                    .OrderBy(q => q.Ordem)
                    .Select(q => new ColunaQuestionarioDto
                    {
                        IdCiclo = p.BimestreId,
                        DescricaoColuna = q.Nome,
                        PeriodoBimestreAtivo = periodoAtivo,
                        QuestaoSubrespostaId = q.Id,
                        OpcaoResposta = ObterOpcoesRespostasPorQuestao(q.Id, questoesAtivas)
                    });
            })
            .ToList();

        return colunas.Count != 0
            ? Task.FromResult(colunas)
            : throw new ErroNaoEncontradoException(MensagemNegocioComuns.COLUNAS_SUBPERGUNTAS_NAO_OBTIDAS);
    }

    private static Task<List<ColunaQuestionarioDto>> ObterColunasPorSubperguntas(
        ICollection<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre> periodosBimestre,
        IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas,
        int bimestreId)
    {
        var periodo = periodosBimestre
            .FirstOrDefault(p => !p.Excluido && p.BimestreId == bimestreId)
            ?? throw new ErroNaoEncontradoException(MensagemNegocioComuns.PERIODO_BIMESTRE_NAO_ENCONTRADO);

        var agora = DateTime.Now;
        var periodoAtivo = agora >= periodo.DataInicio && agora <= periodo.DataFim;

        var colunasPorQuestao = questoesAtivas
            .Where(q => !q.Excluido && q.Tipo != TipoQuestao.QuestaoComSubpergunta)
            .OrderBy(q => q.Ordem)
            .Select(q => new ColunaQuestionarioDto
            {
                IdCiclo = bimestreId,
                DescricaoColuna = q.Nome,
                PeriodoBimestreAtivo = periodoAtivo,
                QuestaoSubrespostaId = q.Id,
                OpcaoResposta = ObterOpcoesRespostasPorQuestao(q.Id, questoesAtivas)
            })
            .ToList();

        return colunasPorQuestao.Count != 0
            ? Task.FromResult(colunasPorQuestao)
            : throw new ErroNaoEncontradoException(MensagemNegocioComuns.COLUNAS_SUBPERGUNTAS_NAO_OBTIDAS);
    }

    private static Task<List<ColunaQuestionarioDto>> ObterColunasPorBimestres(
        ICollection<Dominio.Entidades.Sondagem.SondagemPeriodoBimestre> periodosBimestre,
        IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        var questaoBimestre = questoesAtivas.FirstOrDefault(q => q.Tipo != TipoQuestao.LinguaPortuguesaSegundaLingua)
            ?? throw new ErroNaoEncontradoException(MensagemNegocioComuns.QUESTAO_PRINCIPAL_NAO_ENCONTRADA);

        var opcoesResposta = ObterOpcoesRespostasPorQuestao(questaoBimestre.Id, questoesAtivas);

        var bimestresAtivos = periodosBimestre
            .Where(p => !p.Excluido)
            .Select(p => new ColunaQuestionarioDto
            {
                IdCiclo = p.BimestreId,
                DescricaoColuna = p.Bimestre?.Descricao ?? string.Empty,
                PeriodoBimestreAtivo = DateTime.Now >= p.DataInicio && DateTime.Now <= p.DataFim,
                OpcaoResposta = opcoesResposta
            })
            .ToList();

        return bimestresAtivos.Count != 0
            ? Task.FromResult(bimestresAtivos)
            : throw new ErroNaoEncontradoException(MensagemNegocioComuns.COLUNAS_CICLOS_NAO_OBTIDAS);
    }

    protected static List<OpcaoRespostaDto> ObterOpcoesRespostasPorQuestao(int questaoId, IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        return questoesAtivas
            .Where(q => q.Id == questaoId)
            .SelectMany(q => q.QuestaoOpcoes)
            .OrderBy(qo => qo.Ordem)
            .Select(qo => new OpcaoRespostaDto
            {
                Ordem = qo.Ordem,
                DescricaoOpcaoResposta = qo.OpcaoResposta.DescricaoOpcaoResposta,
                Legenda = qo.OpcaoResposta.Legenda,
                CorFundo = qo.OpcaoResposta.CorFundo,
                CorTexto = qo.OpcaoResposta.CorTexto,
                Id = qo.OpcaoResposta.Id
            })
            .ToList();
    }

    protected static bool PossuiQuestaoVinculo(IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        return questoesAtivas.Any(q => q.QuestaoVinculo?.Tipo == TipoQuestao.QuestaoComSubpergunta);
    }

    protected static RespostasProcessadasDto ProcessarRespostas(
        Dictionary<(long CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> respostasAlunosPorQuestoes,
        Dominio.Entidades.Questionario.Questao linguaPortuguesaSegundaLingua,
        IEnumerable<AlunoElasticDto> alunosAtivos,
        DateTime dataInicioSondagem)
    {
        var codigosAlunosAtivos = alunosAtivos
            .Where(a => a.DataSituacao <= dataInicioSondagem)
            .Select(a => a.CodigoAluno)
            .ToHashSet();

        var respostasAuditoria = respostasAlunosPorQuestoes.Values
            .Where(r => r.AlunoId.HasValue && codigosAlunosAtivos.Contains(r.AlunoId.Value))
            .ToList();

        if (linguaPortuguesaSegundaLingua != null)
            respostasAuditoria = respostasAuditoria
                .Where(x => x.QuestaoId != linguaPortuguesaSegundaLingua.Id)
                .ToList();

        var criadoMaisAntigo = respostasAuditoria
            .OrderBy(r => r.CriadoEm)
            .FirstOrDefault();

        var alteradoMaisRecente = respostasAuditoria
            .Where(r => r.AlteradoEm.HasValue)
            .OrderByDescending(r => r.AlteradoEm)
            .FirstOrDefault();

        var nomeInseridoPor = criadoMaisAntigo is not null
            ? $"Inserido por {criadoMaisAntigo.CriadoPor} ({criadoMaisAntigo.CriadoRF}) em {criadoMaisAntigo.CriadoEm:dd/MM/yyyy HH:mm}"
            : null;

        var nomeAlteradoPor = alteradoMaisRecente is not null
            ? $"Alterado por {alteradoMaisRecente.AlteradoPor} ({alteradoMaisRecente.AlteradoRF}) em {alteradoMaisRecente.AlteradoEm:dd/MM/yyyy HH:mm}"
            : null;

        var respostasConvertidas = respostasAlunosPorQuestoes
            .Where(x => x.Value?.OpcaoRespostaId is not null && codigosAlunosAtivos.Contains((int)x.Key.CodigoAluno))
            .GroupBy(x => (
                CodigoAluno: (int)x.Key.CodigoAluno,
                x.Key.BimestreId,
                x.Key.QuestaoId
            ))
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Value!).First()
            );

        return new RespostasProcessadasDto
        {
            InseridoPor = nomeInseridoPor,
            AlteradoPor = nomeAlteradoPor,
            RespostasConvertidas = respostasConvertidas
        };
    }

    protected static ColunaQuestionarioDto ConstruirColunaAluno(
    ColunaQuestionarioDto colunaBase,
    AlunoElasticDto aluno,
    ContextoColunaDto contexto)
    {
        long questaoIdChave = colunaBase.QuestaoSubrespostaId ?? (int)contexto.QuestaoIdPrincipal;
        int? bimestreIdChave = colunaBase.IdCiclo == 0 ? null : colunaBase.IdCiclo;

        int codigoAluno = aluno.CodigoAluno;
        int situacaoMatricula = aluno.CodigoSituacaoMatricula;
        DateTime dataSituacaoMatricula = aluno.DataSituacao;

        var chave = (CodigoAluno: codigoAluno, BimestreId: bimestreIdChave, QuestaoId: questaoIdChave);
        var possuiResposta = contexto.RespostasAlunosPorQuestoes.TryGetValue(chave, out var resposta);

        var podeLancarSondagem = contexto.SondagemAtiva.PeriodosBimestre.Any(p =>
                dataSituacaoMatricula <= p.DataFim && dataSituacaoMatricula >= p.DataInicio)
            && situacaoMatricula == (int)SituacaoMatriculaAluno.Ativo;

        string? descricaoBimestre = string.Empty;
        if (contexto.ExibirBimestreNaDescricaoColuna && bimestreIdChave.HasValue)
            contexto.DescricoesBimestre.TryGetValue(bimestreIdChave.Value, out descricaoBimestre);

        return new ColunaQuestionarioDto
        {
            IdCiclo = colunaBase.IdCiclo,
            BimestreId = bimestreIdChave,
            DescricaoColuna = contexto.ExibirBimestreNaDescricaoColuna
                ? $"{colunaBase.DescricaoColuna} - {descricaoBimestre}"
                : colunaBase.DescricaoColuna,
            PeriodoBimestreAtivo = podeLancarSondagem || colunaBase.PeriodoBimestreAtivo,
            QuestaoSubrespostaId = colunaBase.QuestaoSubrespostaId,
            OpcaoResposta = contexto.EhRelatorio
                ? colunaBase.OpcaoResposta?.Where(op => op.Id == resposta?.OpcaoRespostaId)
                : colunaBase.OpcaoResposta,
            Resposta = ConstruirResposta(possuiResposta, resposta)
        };
    }

    protected static RespostaDto ConstruirResposta(bool possuiResposta, RespostaAluno? resposta)
    {
        if (!possuiResposta || resposta is null)
            return new RespostaDto();

        var opcaoRespostaId = resposta.OpcaoRespostaId == 0 ? null : resposta.OpcaoRespostaId;

        return new RespostaDto
        {
            Id = resposta.Id,
            OpcaoRespostaId = opcaoRespostaId
        };
    }

    protected static string ObterTituloTabelaRespostas(IEnumerable<Dominio.Entidades.Questionario.Questao> questoesAtivas)
    {
        var primeiraQuestao = questoesAtivas.FirstOrDefault();

        return PossuiQuestaoVinculo(questoesAtivas)
            ? questoesAtivas.FirstOrDefault(q => q.QuestaoVinculo?.Tipo == TipoQuestao.QuestaoComSubpergunta)?.QuestaoVinculo?.Nome!
            : primeiraQuestao?.Nome ?? string.Empty;
    }
    private static bool EhBimestreTodos(int? bimestreId)
    {
        return bimestreId == null;
    }
    private async Task<bool> EhMatematicaOuLinguaPortuguesa(int componenteCurricularId)
    {
        var componentes = new List<string> { "LÍNGUA PORTUGUESA", "MATEMÁTICA" };
        var componente = await _mediator.Send(new ObterComponenteCurricularPorIdQuery(componenteCurricularId));
        return componente != null && componentes.Contains(componente.Nome.ToUpper());
    }
    private async Task<bool> EhProficienciaLeituraMapSaberes(int proficienciaId)
    {
        var proficienciaLeitura = new List<string> { "LEITURA" , "MAPEAMENTO DOS SABERES" };
        var proficiencia = await _mediator.Send(new ObterProficienciaPorIdQuery(proficienciaId));
        return proficiencia != null && proficienciaLeitura.Contains(proficiencia.Nome.ToUpper());
    }
}
