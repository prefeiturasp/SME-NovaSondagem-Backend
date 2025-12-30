using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questao;

namespace SME.Sondagem.Aplicacao.UseCases.Questao;

public class CriarQuestaoUseCase : ICriarQuestaoUseCase
{
    private readonly IRepositorioQuestao questaoRepositorio;

    public CriarQuestaoUseCase(IRepositorioQuestao questaoRepositorio)
    {
        this.questaoRepositorio = questaoRepositorio;
    }

    public async Task<long> ExecutarAsync(QuestaoDto questaoDto, CancellationToken cancellationToken = default)
    {
        var questao = new SME.Sondagem.Dominio.Entidades.Questionario.Questao(
            questaoDto.QuestionarioId,
            questaoDto.GrupoQuestoesId,
            questaoDto.Ordem,
            questaoDto.Nome,
            questaoDto.Observacao,
            questaoDto.Obrigatorio,
            questaoDto.Tipo,
            questaoDto.Opcionais,
            questaoDto.SomenteLeitura,
            questaoDto.Dimensao,
            questaoDto.Tamanho,
            questaoDto.Mascara,
            questaoDto.PlaceHolder,
            questaoDto.NomeComponente);

        return await questaoRepositorio.CriarAsync(questao, cancellationToken: cancellationToken);
    }
}