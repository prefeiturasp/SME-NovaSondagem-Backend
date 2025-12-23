using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Ciclo;

public class CicloUseCase : ICicloUseCase
{
    private readonly ICicloRepository cicloRepository;

    public CicloUseCase(ICicloRepository cicloRepository)
    {
        this.cicloRepository = cicloRepository;
    }

    public async Task<IEnumerable<object>> ObterCiclosAsync()
    {
        return await cicloRepository.ObterTodosAsync();
    }
}
