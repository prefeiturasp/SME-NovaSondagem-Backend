using SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;

public class ExcluirQuestaoOpcaoRespostaUseCase : IExcluirQuestaoOpcaoRespostaUseCase
{
    private readonly IRepositorioQuestaoOpcaoResposta questaoOpcaoRespostaRepositorio;

    public ExcluirQuestaoOpcaoRespostaUseCase(IRepositorioQuestaoOpcaoResposta questaoOpcaoRespostaRepositorio)
    {
        this.questaoOpcaoRespostaRepositorio = questaoOpcaoRespostaRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var opcaoQuestaoRespostaExistente = await questaoOpcaoRespostaRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (opcaoQuestaoRespostaExistente == null)
            return false;

        return await questaoOpcaoRespostaRepositorio.RemoverLogico(id, cancellationToken: cancellationToken)>0;
    }
}