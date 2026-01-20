using SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;

public class ObterQuestaoOpcaoRespostaPorIdUseCase : IObterQuestaoOpcaoRespostaPorIdUseCase
{
    private readonly IRepositorioQuestaoOpcaoResposta questaoOpcaoRespostaRepositorio;

    public ObterQuestaoOpcaoRespostaPorIdUseCase(IRepositorioQuestaoOpcaoResposta questaoOpcaoRespostaRepositorio)
    {
        this.questaoOpcaoRespostaRepositorio = questaoOpcaoRespostaRepositorio;
    }

    public async Task<QuestaoOpcaoRespostaDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var questaoOpcaoResposta = await questaoOpcaoRespostaRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (questaoOpcaoResposta == null)
            return null;

        return new QuestaoOpcaoRespostaDto
        {
            Id = questaoOpcaoResposta.Id,
            Ordem = questaoOpcaoResposta.Ordem,
            OpcaoRespostaId = questaoOpcaoResposta.OpcaoRespostaId,
            QuestaoId = questaoOpcaoResposta.QuestaoId,
            CriadoEm = questaoOpcaoResposta.CriadoEm,
            CriadoPor = questaoOpcaoResposta.CriadoPor,
            CriadoRF = questaoOpcaoResposta.CriadoRF,
            AlteradoEm = questaoOpcaoResposta.AlteradoEm,
            AlteradoPor = questaoOpcaoResposta.AlteradoPor,
            AlteradoRF = questaoOpcaoResposta.AlteradoRF
        };
    }
}