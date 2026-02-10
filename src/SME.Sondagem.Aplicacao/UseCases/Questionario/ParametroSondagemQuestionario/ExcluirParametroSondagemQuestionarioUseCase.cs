using SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.ParametroSondagemQuestionario;

public class ExcluirParametroSondagemQuestionarioUseCase : IExcluirParametroSondagemQuestionarioUseCase
{
    private readonly IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio;

    public ExcluirParametroSondagemQuestionarioUseCase(IRepositorioParametroSondagemQuestionario parametroSondagemQuestionarioRepositorio)
    {
        this.parametroSondagemQuestionarioRepositorio = parametroSondagemQuestionarioRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var parametroSondagemQuestionarioExistente = await parametroSondagemQuestionarioRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (parametroSondagemQuestionarioExistente == null)
            return false;

        return await parametroSondagemQuestionarioRepositorio.RemoverLogico(id, cancellationToken: cancellationToken) > 0;
    }
}