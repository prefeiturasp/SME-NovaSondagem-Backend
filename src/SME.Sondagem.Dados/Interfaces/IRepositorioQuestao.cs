using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioQuestao : IRepositorioBase<Questao>
{
    Task<IEnumerable<Questao>> ObterQuestoesAtivasPorFiltroAsync(
        int modalidadeId, 
        int anoLetivo, 
        int proficienciaId, 
        int serieAno, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Questao>> ObterQuestionarioIdPorQuestoesAsync(IEnumerable<int> questoesId, CancellationToken cancellationToken = default);

    Task<Questao?> ObterQuestaoPorQuestionarioETipoNaoExcluidaAsync(int questionarioId, TipoQuestao tipoQuestao, CancellationToken cancellationToken = default);
}
