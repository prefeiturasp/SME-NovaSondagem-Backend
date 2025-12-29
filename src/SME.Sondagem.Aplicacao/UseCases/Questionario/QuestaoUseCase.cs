using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class QuestaoUseCase : IQuestaoUseCase
{
    private readonly IRepositorioQuestao questaoRepository;

    public QuestaoUseCase(IRepositorioQuestao questaoRepository)
    {
        this.questaoRepository = questaoRepository;
    }

    public async Task<IEnumerable<object>> ObterQuestoesAsync()
    {
        return await questaoRepository.ObterTodosAsync();
    }
}
