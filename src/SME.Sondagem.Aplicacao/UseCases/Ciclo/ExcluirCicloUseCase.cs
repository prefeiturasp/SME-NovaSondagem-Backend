using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Ciclo;

public class ExcluirCicloUseCase : IExcluirCicloUseCase
{
    private readonly IRepositorioCiclo cicloRepositorio;

    public ExcluirCicloUseCase(IRepositorioCiclo cicloRepositorio)
    {
        this.cicloRepositorio = cicloRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var cicloExistente = await cicloRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (cicloExistente == null)
            return false;

        return await cicloRepositorio.ExcluirAsync(id, cancellationToken: cancellationToken);
    }
}