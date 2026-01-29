using SME.Sondagem.Dados.Repositorio;
using SME.Sondagem.Dominio.Entidades;
using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioBimestre : IRepositorioBase<Bimestre>
{
    Task<ICollection<SondagemPeriodoBimestre>> ObterBimestresPorQuestionarioIdAsync(int questionarioId, CancellationToken cancellationToken = default);
}