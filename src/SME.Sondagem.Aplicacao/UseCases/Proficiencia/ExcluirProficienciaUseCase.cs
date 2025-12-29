using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Proficiencia;

public class ExcluirProficienciaUseCase : IExcluirProficienciaUseCase
{
    private readonly IRepositorioProficiencia proficienciaRepositorio;

    public ExcluirProficienciaUseCase(IRepositorioProficiencia proficienciaRepositorio)
    {
        this.proficienciaRepositorio = proficienciaRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var proficienciaExistente = await proficienciaRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (proficienciaExistente == null)
            return false;

        return await proficienciaRepositorio.ExcluirAsync(id);
    }
}