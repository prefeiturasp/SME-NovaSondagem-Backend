using System.Collections.Generic;
using System.Threading.Tasks;
using SME.Sondagem.Application.Interfaces;
using SME.Sondagem.Infra.Repositories;

namespace SME.Sondagem.Application.UseCases
{
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
}
