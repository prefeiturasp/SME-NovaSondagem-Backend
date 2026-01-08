using SME.Sondagem.Aplicacao.Interfaces.Bimestre;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Bimestre;

public class ExcluirBimestreUseCase : IExcluirBimestreUseCase
{
    private readonly IRepositorioBimestre bimestreRepositorio;

    public ExcluirBimestreUseCase(IRepositorioBimestre bimestreRepositorio)
    {
        this.bimestreRepositorio = bimestreRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var bimestreExistente = await bimestreRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (bimestreExistente == null)
            return false;

        return await bimestreRepositorio.ExcluirAsync(id, cancellationToken: cancellationToken);
    }
}