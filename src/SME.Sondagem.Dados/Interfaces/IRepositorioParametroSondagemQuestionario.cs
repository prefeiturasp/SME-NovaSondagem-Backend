using SME.Sondagem.Dominio.Entidades;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioParametroSondagemQuestionario : IRepositorioBase<ParametroSondagemQuestionario>
{
    Task<IEnumerable<ParametroSondagemQuestionario>> ObterPorIdQuestionarioAsync(long idQuestionario, CancellationToken cancellationToken = default);
}
