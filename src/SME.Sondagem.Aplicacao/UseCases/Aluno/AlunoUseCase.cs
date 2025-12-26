using SME.Sondagem.Aplicacao.Interfaces.Aluno;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Aluno;

public class AlunoUseCase : IAlunoUseCase
{
    private readonly IAlunoRepository alunoRepository;

    public AlunoUseCase(IAlunoRepository alunoRepository)
    {
        this.alunoRepository = alunoRepository;
    }

    public async Task<IEnumerable<object>> ObterAlunosAsync()
    {
        return await alunoRepository.ObterTodosAsync();
    }
}
