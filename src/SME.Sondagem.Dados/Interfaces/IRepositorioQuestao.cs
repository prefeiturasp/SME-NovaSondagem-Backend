using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioQuestao : IRepositorioBase<Questao>
{
    // Task<IEnumerable<Questao>> ObterTodosAsync(CancellationToken cancellationToken = default);
    // Task<Questao?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    // Task<long> CriarAsync(Questao questao, CancellationToken cancellationToken = default);
    // Task<bool> AtualizarAsync(Questao questao, CancellationToken cancellationToken = default);
    // Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Questao>> ObterQuestoesAtivasPorFiltroAsync(
        int modalidadeId, 
        int anoLetivo, 
        int proficienciaId, 
        int serieAno, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Questao>> ObterQuestionarioIdPorQuestoesAsync(IEnumerable<int> questoesId, CancellationToken cancellationToken = default);

    Task<Questao?> ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(int questionarioId, TipoQuestao tipoQuestao, CancellationToken cancellationToken = default);
}
