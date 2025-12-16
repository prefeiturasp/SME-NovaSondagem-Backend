using System.Collections.Generic;
using System.Threading.Tasks;
using SME.Sondagem.Application.Interfaces;
using SME.Sondagem.Infra.Repositories;

namespace SME.Sondagem.Application.UseCases
{
    public class QuestionarioUseCase : IQuestionarioUseCase
    {
        private readonly IQuestionarioRepository questionarioRepository;

        public QuestionarioUseCase(IQuestionarioRepository questionarioRepository)
        {
            this.questionarioRepository = questionarioRepository;
        }

        public async Task<IEnumerable<object>> ObterQuestionariosAsync()
        {
            return await questionarioRepository.ObterTodosAsync();
        }
    }
}
