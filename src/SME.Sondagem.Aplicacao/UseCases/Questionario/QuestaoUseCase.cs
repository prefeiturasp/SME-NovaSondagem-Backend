using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class QuestaoUseCase : IQuestaoUseCase
{
    private readonly IQuestaoRepository questaoRepository;

    public QuestaoUseCase(IQuestaoRepository questaoRepository)
    {
        this.questaoRepository = questaoRepository;
    }

    public async Task<IEnumerable<object>> ObterQuestoesAsync()
    {
        return await questaoRepository.ObterTodosAsync();
    }
}
