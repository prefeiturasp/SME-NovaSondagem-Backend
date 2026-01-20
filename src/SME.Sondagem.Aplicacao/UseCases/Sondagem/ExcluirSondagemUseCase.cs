using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Sondagem;

public class ExcluirSondagemUseCase : IExcluirSondagemUseCase
{
    private readonly IRepositorioSondagem sondagemRepositorio;

    public ExcluirSondagemUseCase(IRepositorioSondagem sondagemRepositorio)
    {
        this.sondagemRepositorio = sondagemRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var sondagemExistente = await sondagemRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (sondagemExistente == null)
            return false;

        return await sondagemRepositorio.ExcluirAsync(id, cancellationToken: cancellationToken);
    }
}