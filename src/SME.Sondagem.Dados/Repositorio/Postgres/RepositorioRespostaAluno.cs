using Microsoft.EntityFrameworkCore;
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

    public async Task<Dictionary<int, bool>> VerificarAlunosPossuiLinguaPortuguesaAsync(List<int> alunosIds,
        Dominio.Entidades.Questionario.Questao? questao, CancellationToken cancellationToken)
    {
        var respostas = new List<int>();

        if (questao is not null)
        {
            respostas = await _context.RespostasAluno
            .Include(ra => ra.Questao)
            .Where(ra => ra.AlunoId > 0
                && alunosIds.Contains(ra.AlunoId)
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


    public async Task<IEnumerable<RespostaAluno>> ObterRespostasPorSondagemEAlunosAsync(int sondagemId,
        IEnumerable<int> alunosIds, IEnumerable<int> questoesIds,
        CancellationToken cancellationToken = default)
    {
        var alunosIdsList = alunosIds.ToList();
        var questoesIdsList = questoesIds.ToList();

        if (alunosIdsList.Count == 0 || questoesIdsList.Count == 0)
            return [];

        return await _context.RespostasAluno
            .AsNoTracking()
            .Where(ra => !ra.Excluido && ra.SondagemId == sondagemId
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
            .ThenInclude(q => q.Questionario)
            .ThenInclude(q2 => q2.Proficiencia)
            .Include(ra => ra.OpcaoResposta)
            .Where(ra => !ra.Excluido && ra.OpcaoRespostaId.HasValue)
            .AsNoTracking();

        query = AplicarFiltrosRelatorioConsolidado(query, filtro);

        return await query.Select(ra => new RelatorioRespostaAlunoDto
        {
            Id = ra.Id,
            SondagemId = ra.SondagemId,
            SondagemDescricao = ra.Sondagem.Descricao,
            AlunoId = ra.AlunoId,
            QuestaoId = ra.QuestaoId,
            QuestaoNome = ra.Questao.Nome,
            OpcaoRespostaId = ra.OpcaoRespostaId,
            OpcaoRespostaDescricao = ra.OpcaoResposta.DescricaoOpcaoResposta,
            OpcaoRespostaLegenda = ra.OpcaoResposta.Legenda,
            DataResposta = ra.DataResposta,
            BimestreId = ra.BimestreId,
            BimestreDescricao = ra.Bimestre != null ? ra.Bimestre.Descricao : null,
            TurmaId = ra.TurmaId,
            UeId = ra.UeId,
            DreId = ra.DreId,
            AnoLetivo = ra.AnoLetivo,
            ModalidadeId = ra.ModalidadeId,
            RacaCor = ra.RacaCor != null ? new RelatorioRacaCorDto
            {
                Id = ra.RacaCor.Id,
                Descricao = ra.RacaCor.Descricao,
                CodigoEol = ra.RacaCor.CodigoEolRacaCor
            } : null,
            GeneroSexo = ra.GeneroSexo != null ? new RelatorioGeneroSexoDto
            {
                Id = ra.GeneroSexo.Id,
                Descricao = ra.GeneroSexo.Descricao,
                Sigla = ra.GeneroSexo.Sigla
            } : null,
            ProgramaAtendimento = ra.ProgramaAtendimento != null ? new RelatorioProgramaAtendimentoDto
            {
                Id = ra.ProgramaAtendimento.Id,
                Descricao = ra.ProgramaAtendimento.Descricao
            } : null,
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
            CancellationToken cancellationToken = default)
    {
        var respostas = await _context.RespostasAluno
            .Where(r => r.AlunoId > 0
                        && codigosAlunos.Contains((long)r.AlunoId)
                        && questoesIds.Contains(r.QuestaoId)
                        && r.SondagemId == sondagemId
                        && !r.Excluido)
            .ToListAsync(cancellationToken);

        return respostas.ToDictionary(
            r => ((long)(r.AlunoId), (long)r.QuestaoId, r.BimestreId)
        );
    }
    
    
    public async Task<IEnumerable<ExtracaoSondagemLpEscritaDto>> ObterExtracaoDadosRespostasAsync(
        int modalidadeId,
        int componenteCurricularId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RespostasAluno
            .AsNoTracking()
            .Include(ra => ra.Questao)
            .ThenInclude(q => q.Questionario)
            .ThenInclude(q2 => q2.ComponenteCurricular)
            .Include(ra => ra.Questao)
            .ThenInclude(q => q.Questionario)
            .ThenInclude(q2 => q2.Proficiencia)
            .Include(ra => ra.OpcaoResposta)
            .Where(ra =>
                ra.Questao.Questionario.ModalidadeId == modalidadeId &&
                ra.Questao.Questionario.ComponenteCurricularId == componenteCurricularId)
            .OrderBy(ra => ra.AlunoId)
            .ThenBy(ra => ra.QuestaoId)
            .Select(ra => new ExtracaoSondagemLpEscritaDto
            {
                RespostaId           = ra.Id,
                CodigoEolEstudante   = ra.AlunoId.ToString(),
                Questao              = ra.Questao.Nome,
                Resposta             = ra.OpcaoResposta != null ? ra.OpcaoResposta.DescricaoOpcaoResposta : null,
                Legenda              = ra.OpcaoResposta != null ? ra.OpcaoResposta.Legenda : null,
                Bimestre             = ra.BimestreId.ToString(),
                ComponenteCurricular = ra.Questao.Questionario.ComponenteCurricular.Nome,
                Proficiencia         = ra.Questao.Questionario.Proficiencia.Nome,
                ModalidadeId         = ra.Questao.Questionario.ModalidadeId ?? 0,
                DataResposta         = ra.DataResposta
            })
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<RespostaAluno> AplicarFiltrosRelatorioConsolidado(IQueryable<RespostaAluno> query, FiltroConsolidadoDto filtro)
    {
        if (filtro.AnoLetivo > 0)
            query = query.Where(ra => ra.AnoLetivo == filtro.AnoLetivo);

        if (!string.IsNullOrEmpty(filtro.Dre))
            query = query.Where(ra => ra.DreId == filtro.Dre);

        if (!string.IsNullOrEmpty(filtro.Ue))
            query = query.Where(ra => ra.UeId == filtro.Ue);

        if (filtro.Modalidade > 0)
            query = query.Where(ra => ra.ModalidadeId == filtro.Modalidade.ToString());

        if (filtro.BimestreId.HasValue)
            query = query.Where(ra => ra.BimestreId == filtro.BimestreId.Value);

        if (filtro.ProficienciaId > 0)
            query = query.Where(ra => ra.Questao.Questionario.ProficienciaId == filtro.ProficienciaId);

        if (filtro.ComponenteCurricularId > 0)
            query = query.Where(ra => ra.Questao.Questionario.ComponenteCurricularId == filtro.ComponenteCurricularId);

        if (filtro.GeneroId > 0)
            query = query.Where(ra => ra.GeneroSexo != null && ra.GeneroSexo.Id == filtro.GeneroId);

        if (filtro.RacaId > 0)
            query = query.Where(ra => ra.RacaCor != null && ra.RacaCor.Id == filtro.RacaId);

        if (filtro.ProgramaAtendimentoId > 0)
            query = query.Where(ra => ra.ProgramaAtendimento != null && ra.ProgramaAtendimento.Id == filtro.ProgramaAtendimentoId);

        return query;
    }
}