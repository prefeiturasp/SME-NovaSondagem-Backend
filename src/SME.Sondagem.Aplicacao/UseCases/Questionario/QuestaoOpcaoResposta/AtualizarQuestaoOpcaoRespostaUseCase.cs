using SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;

public class AtualizarQuestaoOpcaoRespostaUseCase : IAtualizarQuestaoOpcaoRespostaUseCase
{
    private readonly IRepositorioQuestaoOpcaoResposta questaoOpcaoRespostaRepositorio;

    public AtualizarQuestaoOpcaoRespostaUseCase(IRepositorioQuestaoOpcaoResposta questaoOpcaoRespostaRepositorio)
    {
        this.questaoOpcaoRespostaRepositorio = questaoOpcaoRespostaRepositorio;
    }

    public async Task<QuestaoOpcaoRespostaDto?> ExecutarAsync(long id, QuestaoOpcaoRespostaDto questaoOpcaoRespostaDto, CancellationToken cancellationToken = default)
    {
        var questaoOpcaoRespostaExistente = await questaoOpcaoRespostaRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (questaoOpcaoRespostaExistente == null)
            return null;

        questaoOpcaoRespostaExistente.AtualizarQuestaoId(questaoOpcaoRespostaDto.QuestaoId);
        questaoOpcaoRespostaExistente.AtualizarOpcaoRespostaId(questaoOpcaoRespostaDto.OpcaoRespostaId);
        questaoOpcaoRespostaExistente.AtualizarOrdem(questaoOpcaoRespostaDto.Ordem);

        var sucesso = await questaoOpcaoRespostaRepositorio.AtualizarAsync(questaoOpcaoRespostaExistente, cancellationToken: cancellationToken);
        
        if (!sucesso)
            return null;

        return new QuestaoOpcaoRespostaDto
        {
            Id = questaoOpcaoRespostaExistente.Id,
            OpcaoRespostaId = questaoOpcaoRespostaExistente.OpcaoRespostaId,
            QuestaoId = questaoOpcaoRespostaExistente.QuestaoId,
            Ordem = questaoOpcaoRespostaExistente.Ordem,           
            CriadoEm = questaoOpcaoRespostaExistente.CriadoEm,
            CriadoPor = questaoOpcaoRespostaExistente.CriadoPor,
            CriadoRF = questaoOpcaoRespostaExistente.CriadoRF,
            AlteradoEm = questaoOpcaoRespostaExistente.AlteradoEm,
            AlteradoPor = questaoOpcaoRespostaExistente.AlteradoPor,
            AlteradoRF = questaoOpcaoRespostaExistente.AlteradoRF
        };
    }
}