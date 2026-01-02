using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;

public interface IObterQuestoesUseCase
{
    Task<IEnumerable<QuestaoDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}