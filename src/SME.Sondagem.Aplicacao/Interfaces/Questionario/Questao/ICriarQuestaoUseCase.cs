using SME.Sondagem.Infrastructure.Dtos.Questao;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;

public interface ICriarQuestaoUseCase
{
    Task<long> ExecutarAsync(QuestaoDto proficienciaDto, CancellationToken cancellationToken = default);
}