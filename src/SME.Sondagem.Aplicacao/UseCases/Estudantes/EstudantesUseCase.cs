using SME.Sondagem.Aplicacao.Interfaces.Estudantes;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Estudantes;

public class EstudantesUseCase : IEstudantesUseCase
{
    private readonly IEstudantesRepository estudantesRepository;

    public EstudantesUseCase(IEstudantesRepository estudantesRepository)
    {
        this.estudantesRepository = estudantesRepository;
    }

    public async Task<IEnumerable<object>> ObterEstudantesAsync()
    {
        return await estudantesRepository.ObterTodosAsync();
    }
}
