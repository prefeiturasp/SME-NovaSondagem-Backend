using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;

public class ExcluirOpcaoRespostaUseCase : IExcluirOpcaoRespostaUseCase
{
    private readonly IRepositorioOpcaoResposta opcaoRespostaRepositorio;

    public ExcluirOpcaoRespostaUseCase(IRepositorioOpcaoResposta opcaoRespostaRepositorio)
    {
        this.opcaoRespostaRepositorio = opcaoRespostaRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var opcaoRespostaExistente = await opcaoRespostaRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (opcaoRespostaExistente == null)
            return false;

        return await opcaoRespostaRepositorio.RemoverLogico(id, cancellationToken: cancellationToken) > 0;
    }
}