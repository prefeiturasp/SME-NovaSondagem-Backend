using SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.ComponenteCurricular;

public class ComponenteCurricularUseCase : IComponenteCurricularUseCase
{
    private readonly IComponenteCurricularRepository componenteRepository;

    public ComponenteCurricularUseCase(IComponenteCurricularRepository componenteRepository)
    {
        this.componenteRepository = componenteRepository;
    }

    public async Task<IEnumerable<object>> ObterComponentesAsync()
    {
        return await componenteRepository.ObterTodosAsync();
    }
}
