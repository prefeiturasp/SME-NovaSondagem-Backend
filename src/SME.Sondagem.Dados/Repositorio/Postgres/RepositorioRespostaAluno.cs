using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SME.Sondagem.Dados.Contexto;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infra.Contexto;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Dados.Repositorio.Postgres;

public class RepositorioRespostaAluno : RepositorioBase<RespostaAluno>, IRepositorioRespostaAluno
{
    public RepositorioRespostaAluno(SondagemDbContext context, IServicoAuditoria servicoAuditoria, ContextoBase contextoBase) : base(context,
        servicoAuditoria, contextoBase)
    {
    }
    public async Task<bool> VerificarAlunoTemRespostaPorTipoQuestaoAsync(int alunoId, TipoQuestao tipoQuestao,
        CancellationToken cancellationToken)
    {
        return await _context.RespostasAluno
            .AnyAsync(ra => ra.AlunoId == alunoId && ra.Questao.Tipo == tipoQuestao, cancellationToken);
    }

    public async Task<Dictionary<int, bool>> VerificarAlunosPossuiLinguaPortuguesaAsync(
        List<int> alunosIds,
        Dominio.Entidades.Questionario.Questao? questao,
        string turmaId,
        CancellationToken cancellationToken)
    {
        var respostas = new List<int>();

        if (questao is not null)
        {
            respostas = await _context.RespostasAluno
            .Include(ra => ra.Questao)
            .Where(ra => ra.AlunoId > 0
                && alunosIds.Contains(ra.AlunoId)
                && ra.TurmaId == turmaId
                && ra.Questao.Tipo == TipoQuestao.LinguaPortuguesaSegundaLingua
                && ra.QuestaoId == questao.Id
                && ra.OpcaoResposta.DescricaoOpcaoResposta.ToLower() == "sim")
            .Select(ra => ra.AlunoId)
            .Distinct()
            .ToListAsync(cancellationToken);
        }

        return alunosIds
            .Distinct()
            .ToDictionary(
                alunoId => alunoId,
                alunoId => respostas.Contains(alunoId)
            );
    }


    public async Task<IEnumerable<RespostaAluno>> ObterRespostasPorSondagemEAlunosAsync(
        int sondagemId,
        string turmaId,
        IEnumerable<int> alunosIds,
        IEnumerable<int> questoesIds,
        CancellationToken cancellationToken = default)
    {
        var alunosIdsList = alunosIds.ToList();
        var questoesIdsList = questoesIds.ToList();

        if (alunosIdsList.Count == 0 || questoesIdsList.Count == 0)
            return [];

        return await _context.RespostasAluno
            .AsNoTracking()
            .Where(ra => !ra.Excluido && ra.SondagemId == sondagemId
                                  && ra.TurmaId == turmaId
                                  && alunosIdsList.Contains(ra.AlunoId)
                                  && questoesIdsList.Contains(ra.QuestaoId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RelatorioRespostaAlunoDto>> ObterRespostasParaRelatorioConsolidadoAsync(FiltroConsolidadoDto filtro, CancellationToken cancellationToken = default)
    {
        var query = _context.RespostasAluno
            .Include(ra => ra.Questao)
            .ThenInclude(q => q.Questionario)
            .ThenInclude(q2 => q2.ComponenteCurricular)
            .Include(ra => ra.Questao)
            .Include(ra => ra.OpcaoResposta)
            .Where(ra => !ra.Excluido && !ra.OpcaoResposta.Excluido && ra.OpcaoRespostaId.HasValue && ra.Questao.Tipo != TipoQuestao.LinguaPortuguesaSegundaLingua)
            .AsNoTracking();

        query = AplicarFiltrosRelatorioConsolidado(query, filtro);


        return await query.Select(ra => new RelatorioRespostaAlunoDto
        {
            Id = ra.Id,
            SondagemId = ra.SondagemId,
            SondagemDescricao = ra.Sondagem.Descricao,
            AlunoId = ra.AlunoId,
            QuestaoId = ra.QuestaoId,
            QuestaoNome = $"{ra.Questao.Nome} ({ra.AnoTurma}º ano)",
            OpcaoRespostaId = ra.OpcaoRespostaId,
            OpcaoRespostaDescricao = ra.OpcaoResposta.DescricaoOpcaoResposta,
            OpcaoRespostaLegenda = ra.OpcaoResposta.Legenda,
            DataResposta = ra.DataResposta,
            BimestreId = ra.BimestreId,
            BimestreDescricao = ra.Bimestre != null ? ra.Bimestre.Descricao : null,
            AnoTurma = ra.AnoTurma,
            TurmaId = ra.TurmaId,
            UeId = ra.UeId,
            DreId = ra.DreId,
            AnoLetivo = ra.AnoLetivo,
            ModalidadeId = ra.ModalidadeId,
            RacaCorId = ra.RacaCorId,
            GeneroSexoId = ra.GeneroSexoId,
            OpcoesDisponiveis = ra.Questao.QuestaoOpcoes
                .OrderBy(qo => qo.Ordem)
                .Select(qo => new RelatorioOpcaoRespostaDto
                {
                    Id = qo.OpcaoRespostaId,
                    Descricao = qo.OpcaoResposta.DescricaoOpcaoResposta,
                    Legenda = qo.OpcaoResposta.Legenda,
                    Ordem = qo.Ordem,
                    CorFundo = qo.OpcaoResposta.CorFundo,
                    CorTexto = qo.OpcaoResposta.CorTexto
                })
        }).ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<(long CodigoAluno, long QuestaoId, int? BimestreId), RespostaAluno>>
        ObterRespostasAlunosPorQuestoesAsync(
            List<long> codigosAlunos,
            List<long> questoesIds,
            long sondagemId,
            string turmaId,
            CancellationToken cancellationToken = default)
    {
        var respostas = await _context.RespostasAluno
            .Where(r => r.AlunoId > 0
                        && codigosAlunos.Contains((long)r.AlunoId)
                        && questoesIds.Contains(r.QuestaoId)
                        && r.SondagemId == sondagemId
                        && r.TurmaId == turmaId
                        && !r.Excluido)
            .ToListAsync(cancellationToken);

        return respostas.ToDictionary(
            r => ((long)(r.AlunoId), (long)r.QuestaoId, r.BimestreId)
        );
    }


    public async Task<IEnumerable<ExtracaoConsultaSondagemLpEscritaDto>> ObterExtracaoDadosRespostasAsync(
        int modalidadeId,
        int componenteCurricularId,
        string dreId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RespostasAluno
            .AsNoTracking()
            .Where(ra =>
                !ra.Excluido &&
                ra.DreId == dreId &&
                ra.Questao.Questionario.ModalidadeId.HasValue &&
                ra.Questao.Questionario.ModalidadeId.Value == modalidadeId &&
                ra.Questao.Questionario.ComponenteCurricularId == componenteCurricularId &&
                ra.Questao != null &&
                ra.Questao.Questionario != null &&
                ra.Questao.Questionario.ComponenteCurricular != null &&
                ra.Questao.Questionario.Proficiencia != null)
            .Include(ra => ra.Questao)
            .ThenInclude(q => q.Questionario)
            .ThenInclude(q2 => q2.ComponenteCurricular)
            .Include(ra => ra.Questao)
            .ThenInclude(q => q.Questionario)
            .ThenInclude(q2 => q2.Proficiencia)
            .Include(ra => ra.OpcaoResposta)
            .OrderBy(ra => ra.AlunoId)
            .ThenBy(ra => ra.QuestaoId)
            .Select(ra => new ExtracaoConsultaSondagemLpEscritaDto
            {
                CodigoEolEstudante = ra.AlunoId.ToString(),
                Questao = ra.Questao.Nome,
                Resposta = ra.OpcaoResposta != null ? ra.OpcaoResposta.DescricaoOpcaoResposta : null,
                Legenda = ra.OpcaoResposta != null ? ra.OpcaoResposta.Legenda : null,
                Bimestre = ra.BimestreId.HasValue ? ra.BimestreId.Value.ToString() : null,
                ComponenteCurricular = ra.Questao.Questionario.ComponenteCurricular.Nome,
                Proficiencia = ra.Questao.Questionario.Proficiencia.Nome,
                ModalidadeId = ra.Questao.Questionario.ModalidadeId ?? 0,
                CodigoEolEscola = ra.UeId,
                RacaId = ra.RacaCorId,
                GeneroId = ra.GeneroSexoId,
                CodigoDre = ra.DreId,
                TurmaId = ra.TurmaId,
                AnoTurma = ra.AnoTurma
            })
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<RespostaAluno> AplicarFiltrosRelatorioConsolidado(IQueryable<RespostaAluno> query, FiltroConsolidadoDto filtro)
    {
        if (!AbrangenciaValida(filtro))
            return query.Where(_ => false);

        var dresAbrangencia = filtro.DresAbrangencia;
        var uesAbrangencia = filtro.UesAbrangencia;
        var turmasAbrangencia = filtro.TurmasAbrangencia;

        var filtros = new List<(bool Aplicar, System.Linq.Expressions.Expression<Func<RespostaAluno, bool>> Predicado)>
        {
            (filtro.AnoLetivo > 0,                                          ra => ra.AnoLetivo == filtro.AnoLetivo),
            (!string.IsNullOrEmpty(filtro.Dre) || (dresAbrangencia != null && dresAbrangencia.Count > 0),
                                                                            ra => filtro.Dre != null ? ra.DreId == filtro.Dre : dresAbrangencia!.Contains(ra.DreId!)),
            (!string.IsNullOrEmpty(filtro.Ue) || (uesAbrangencia != null && uesAbrangencia.Count > 0),
                                                                            ra => filtro.Ue != null ? ra.UeId == filtro.Ue : uesAbrangencia!.Contains(ra.UeId!)),
            (turmasAbrangencia != null && turmasAbrangencia.Count > 0,      ra => ra.TurmaId != null && turmasAbrangencia!.Contains(ra.TurmaId!)),
            (filtro.Modalidade > 0,                                         ra => ra.ModalidadeId == filtro.Modalidade),
            (filtro.BimestreId.HasValue,                                    ra => ra.BimestreId == filtro.BimestreId),
            (filtro.SemestreId > 0,                                         ra => ra.SemestreId == filtro.SemestreId),
            (filtro.ProficienciaId > 0,                                     ra => ra.Questao.Questionario.ProficienciaId == filtro.ProficienciaId),
            (filtro.ComponenteCurricularId > 0,                             ra => ra.Questao.Questionario.ComponenteCurricularId == filtro.ComponenteCurricularId),
            (filtro.GeneroId > 0,                                           ra => ra.GeneroSexo != null && ra.GeneroSexo.Id == filtro.GeneroId),
            (filtro.RacaId > 0,                                             ra => ra.RacaCor != null && ra.RacaCor.Id == filtro.RacaId),
            (filtro.AnoTurma != null && filtro.AnoTurma.Count != 0,         ra => ra.AnoTurma.HasValue && filtro.AnoTurma!.Contains(ra.AnoTurma.Value)),
            (filtro.Pap.HasValue,                                           ra => ra.Pap == filtro.Pap),
            (filtro.Aee.HasValue,                                           ra => ra.Aee == filtro.Aee),
            (filtro.Deficiente.HasValue,                                    ra => ra.Deficiente == filtro.Deficiente),
            (filtro.PossuiLinguaPortuguesaSegundaLingua.HasValue,           PredicadoLinguaPortuguesaSegundaLingua(filtro.PossuiLinguaPortuguesaSegundaLingua ?? false))
        };

        return filtros
            .Where(f => f.Aplicar)
            .Aggregate(query, (q, f) => q.Where(f.Predicado));
    }

    private static bool AbrangenciaValida(FiltroConsolidadoDto filtro)
    {
        if (filtro.AcessoIrrestrito)
            return true;

        if (string.IsNullOrEmpty(filtro.Dre) && (filtro.DresAbrangencia == null || filtro.DresAbrangencia.Count == 0))
            return false;

        if (!string.IsNullOrEmpty(filtro.Dre) && string.IsNullOrEmpty(filtro.Ue) && (filtro.UesAbrangencia == null || filtro.UesAbrangencia.Count == 0))
            return false;

        if (filtro.TurmasAbrangencia != null && filtro.TurmasAbrangencia.Count == 0)
            return false;

        return true;
    }

    private static System.Linq.Expressions.Expression<Func<RespostaAluno, bool>> PredicadoLinguaPortuguesaSegundaLingua(bool possuiLingua)
    {
        return ra => ra.Sondagem.Respostas.Any(ra2 =>
            ra2.AlunoId == ra.AlunoId &&
            ra2.Questao.Tipo == TipoQuestao.LinguaPortuguesaSegundaLingua &&
            ra2.OpcaoResposta != null &&
            ra2.OpcaoResposta.DescricaoOpcaoResposta != null &&
            string.Equals(ra2.OpcaoResposta.DescricaoOpcaoResposta, "sim", StringComparison.OrdinalIgnoreCase) &&
            !ra2.Excluido) == possuiLingua;
    }

    public async Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Sondagem.RespostaAlunoLegadoDto>> ObterRespostasSemContextoPaginadoAsync(int respostaId, int pagina, int tamanhoLote, CancellationToken cancellationToken = default)
    {
        var offset = (pagina - 1) * tamanhoLote;
        var query = @"
            SELECT 
                r.id AS Id,
                r.aluno_id AS AlunoId,
                r.sondagem_id AS SondagemId,
                EXTRACT(YEAR FROM s.data_aplicacao) AS AnoLetivo
            FROM resposta_aluno r
            INNER JOIN sondagem s ON s.id = r.sondagem_id
            WHERE r.id > @respostaId AND r.turma_id IS NULL AND r.excluido = false and r.ano_turma IS NULL
            ORDER BY r.id
            LIMIT @TamanhoLote OFFSET @Offset";

        var conexao = _context.Database.GetDbConnection();
        return await conexao.QueryAsync<SME.Sondagem.Infrastructure.Dtos.Sondagem.RespostaAlunoLegadoDto>(query, new { respostaId, TamanhoLote = tamanhoLote, Offset = offset });
    }

    public async Task<int> AtualizarContextoLoteAsync(IEnumerable<SME.Sondagem.Infrastructure.Dtos.Sondagem.AtualizarContextoRespostaAlunoDto> lote, CancellationToken cancellationToken = default)
    {
        if (!lote.Any()) return 0;

        var query = @"
            UPDATE resposta_aluno
            SET 
                turma_id = @TurmaId,
                modalidade_id = @ModalidadeId,
                ue_id = @UeId,
                dre_id = @DreId,
                ano_letivo = @AnoLetivo,
                ano_turma = @AnoTurma,
                raca_cor_id = @RacaCorId,
                genero_sexo_id = @GeneroSexoId,
                pap = @Pap,
                aee = @Aee,
                deficiente = @Deficiente
            WHERE id = @Id";

        var conexao = _context.Database.GetDbConnection();
        return await conexao.ExecuteAsync(query, lote);
    }

    public async Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Sondagem.TurmaPendenteAeeDto>> ObterTurmasPendentesAeeAsync(int ultimoId, int tamanhoLote, CancellationToken cancellationToken = default)
    {
        var query = @"
            SELECT turma_id AS TurmaId, MAX(ue_id) AS UeId, MAX(id) AS UltimoId
            FROM resposta_aluno
            WHERE id > @UltimoId AND aee = false AND excluido = false AND turma_id IS NOT NULL
            GROUP BY turma_id
            ORDER BY MAX(id)
            LIMIT @TamanhoLote";

        var conexao = _context.Database.GetDbConnection();
        return await conexao.QueryAsync<SME.Sondagem.Infrastructure.Dtos.Sondagem.TurmaPendenteAeeDto>(query, new { UltimoId = ultimoId, TamanhoLote = tamanhoLote });
    }

    public async Task<int> AtualizarAeeLoteAsync(IEnumerable<int> alunoIds, CancellationToken cancellationToken = default)
    {
        var alunoIdsList = alunoIds.ToList();
        if (alunoIdsList.Count == 0) return 0;

        var parametros = new DynamicParameters();
        var placeholders = new List<string>(alunoIdsList.Count);
        for (var i = 0; i < alunoIdsList.Count; i++)
        {
            var nomeParametro = $"AlunoId{i}";
            placeholders.Add($"@{nomeParametro}");
            parametros.Add(nomeParametro, alunoIdsList[i]);
        }

        var query = $"UPDATE resposta_aluno SET aee = true WHERE aee = false AND excluido = false AND aluno_id IN ({string.Join(",", placeholders)})";
        var conexao = _context.Database.GetDbConnection();
        return await conexao.ExecuteAsync(query, parametros);
    }
}
