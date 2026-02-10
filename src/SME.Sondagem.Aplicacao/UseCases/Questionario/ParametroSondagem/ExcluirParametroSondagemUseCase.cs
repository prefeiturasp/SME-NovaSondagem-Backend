using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagem;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagem;

public class ExcluirParametroSondagemUseCase : IExcluirParametroSondagemUseCase
{
    private readonly IRepositorioParametroSondagem parametroSondagemRepositorio;

    public ExcluirParametroSondagemUseCase(IRepositorioParametroSondagem parametroSondagemRepositorio)
    {
        this.parametroSondagemRepositorio = parametroSondagemRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var parametroSondagemExistente = await parametroSondagemRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (parametroSondagemExistente == null)
            return false;

        return await parametroSondagemRepositorio.RemoverLogico(id, cancellationToken: cancellationToken) > 0;
    }
}