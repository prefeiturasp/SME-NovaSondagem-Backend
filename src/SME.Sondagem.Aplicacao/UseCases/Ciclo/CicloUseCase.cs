using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Ciclo;

public class CicloUseCase : ICicloUseCase
{
    private readonly IRepositorioCiclo cicloRepository;

    public CicloUseCase(IRepositorioCiclo cicloRepository)
    {
        this.cicloRepository = cicloRepository;
    }

    public async Task<IEnumerable<object>> ObterCiclosAsync()
    {
        return await cicloRepository.ObterTodosAsync();
    }
}
