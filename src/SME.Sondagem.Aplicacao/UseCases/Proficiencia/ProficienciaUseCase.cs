using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Aluno;

public class ProficienciaUseCase : IProficienciaUseCase
{
    private readonly IRepositorioProficiencia repositorioProficiencia;

    public ProficienciaUseCase(IRepositorioProficiencia proficienciaRepository)
    {
        this.repositorioProficiencia = proficienciaRepository;
    }

    public async Task<IEnumerable<object>> ObterProficienciasAsync()
    {
        return await repositorioProficiencia.ObterTodosAsync();
    }
}