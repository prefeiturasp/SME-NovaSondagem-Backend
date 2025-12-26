using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario
{
    public class QuestionarioUseCase : IQuestionarioUseCase
    {
        private readonly IRepositorioQuestionario questionarioRepository;

        public QuestionarioUseCase(IRepositorioQuestionario questionarioRepository)
        {
            this.questionarioRepository = questionarioRepository;
        }

        public async Task<IEnumerable<object>> ObterQuestionariosAsync()
        {
            return await questionarioRepository.ObterTodosAsync();
        }
    }
}
