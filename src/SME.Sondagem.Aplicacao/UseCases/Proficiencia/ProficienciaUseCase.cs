using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Estudantes;

public class ProficienciaUseCase : IProficienciaUseCase
{
    private readonly IProficienciaRepository proficienciaRepository;

    public ProficienciaUseCase(IProficienciaRepository proficienciaRepository)
    {
        this.proficienciaRepository = proficienciaRepository;
    }

    public async Task<IEnumerable<object>> ObterProficienciasAsync()
    {
        return await proficienciaRepository.ObterTodosAsync();
    }
}
