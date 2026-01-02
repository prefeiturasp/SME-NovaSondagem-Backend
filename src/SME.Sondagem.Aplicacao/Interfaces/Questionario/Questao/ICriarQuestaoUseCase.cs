using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;

public interface ICriarQuestaoUseCase
{
    Task<long> ExecutarAsync(QuestaoDto questaoDto, CancellationToken cancellationToken = default);
}