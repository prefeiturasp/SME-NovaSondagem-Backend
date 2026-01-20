using SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;

public class ObterQuestaoOpcaoRespostasUseCase : IObterQuestaoOpcaoRespostaUseCase
{
    private readonly IRepositorioQuestaoOpcaoResposta questaoQuestaoOpcaoRespostaRepositorio;

    public ObterQuestaoOpcaoRespostasUseCase(IRepositorioQuestaoOpcaoResposta questaoQuestaoOpcaoRespostaRepositorio)
    {
        this.questaoQuestaoOpcaoRespostaRepositorio = questaoQuestaoOpcaoRespostaRepositorio;
    }

    public async Task<IEnumerable<QuestaoOpcaoRespostaDto>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var questaoQuestaoOpcaoRespostas = await questaoQuestaoOpcaoRespostaRepositorio.ObterTodosAsync(cancellationToken: cancellationToken);

        return questaoQuestaoOpcaoRespostas.Select(p => new QuestaoOpcaoRespostaDto
        {
            Id = p.Id,
            Ordem = p.Ordem,
            QuestaoId = p.QuestaoId,
            OpcaoRespostaId = p.OpcaoRespostaId,
            CriadoEm = p.CriadoEm,
            CriadoPor = p.CriadoPor,
            CriadoRF = p.CriadoRF,
            AlteradoEm = p.AlteradoEm,
            AlteradoPor = p.AlteradoPor,
            AlteradoRF = p.AlteradoRF
        });
    }
}