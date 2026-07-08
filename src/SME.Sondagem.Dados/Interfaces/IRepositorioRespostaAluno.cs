using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Infrastructure.Dtos;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioRespostaAluno : IRepositorioBase<RespostaAluno>
{
    Task<bool> VerificarAlunoTemRespostaPorTipoQuestaoAsync(int alunoId, TipoQuestao tipoQuestao,
        CancellationToken cancellationToken);

    Task<Dictionary<int, bool>> VerificarAlunosPossuiLinguaPortuguesaAsync(
        List<int> alunosIds,
        Questao? questao,
        string turmaId,
        CancellationToken cancellationToken);

    Task<IEnumerable<RespostaAluno>> ObterRespostasPorSondagemEAlunosAsync(
        int sondagemId,
        string turmaId,
        IEnumerable<int> alunosIds,
        IEnumerable<int> questoesIds,
        CancellationToken cancellationToken = default);

    Task<Dictionary<(long CodigoAluno, long QuestaoId, int? BimestreId), RespostaAluno>> ObterRespostasAlunosPorQuestoesAsync(
        List<long> codigosAlunos,
        List<long> questoesIds,
        long sondagemId,
        string turmaId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ExtracaoConsultaSondagemLpEscritaDto>> ObterExtracaoDadosRespostasAsync(
        int modalidadeId,
        int componenteCurricularId,
        string dreId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<RelatorioRespostaAlunoDto>> ObterRespostasParaRelatorioConsolidadoAsync(FiltroConsolidadoDto filtro, CancellationToken cancellationToken = default);

    Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Sondagem.RespostaAlunoLegadoDto>> ObterRespostasSemContextoPaginadoAsync(int respostaId, int pagina, int tamanhoLote, CancellationToken cancellationToken = default);

    Task<int> AtualizarContextoLoteAsync(IEnumerable<SME.Sondagem.Infrastructure.Dtos.Sondagem.AtualizarContextoRespostaAlunoDto> lote, CancellationToken cancellationToken = default);

    Task<IEnumerable<SME.Sondagem.Infrastructure.Dtos.Sondagem.LotePendenteAeeDto>> ObterLotePendenteAeeAsync(int ultimoId, int tamanhoLote, CancellationToken cancellationToken = default);

    Task<int> AtualizarAeeLoteAsync(IEnumerable<int> alunoIds, CancellationToken cancellationToken = default);
}
