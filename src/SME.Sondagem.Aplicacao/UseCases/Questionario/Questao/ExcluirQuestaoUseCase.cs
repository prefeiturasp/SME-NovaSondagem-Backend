using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questao;

public class ExcluirQuestaoUseCase : IExcluirQuestaoUseCase
{
    private readonly IRepositorioQuestao questaoRepositorio;

    public ExcluirQuestaoUseCase(IRepositorioQuestao questaoRepositorio)
    {
        this.questaoRepositorio = questaoRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var questaoExistente = await questaoRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (questaoExistente == null)
            return false;

        return await questaoRepositorio.ExcluirAsync(id, cancellationToken: cancellationToken);
    }
}