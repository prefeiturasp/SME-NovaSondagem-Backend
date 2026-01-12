using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

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
            questionarioId: questaoDto.QuestionarioId,
            ordem: questaoDto.Ordem,
            nome: questaoDto.Nome,
            observacao: questaoDto.Observacao,
            obrigatorio: questaoDto.Obrigatorio,
            tipo: questaoDto.Tipo,
            opcionais: questaoDto.Opcionais,
            somenteLeitura: questaoDto.SomenteLeitura,
            dimensao: questaoDto.Dimensao,
            grupoQuestoesId: questaoDto.GrupoQuestoesId,
            tamanho: questaoDto.Tamanho,
            mascara: questaoDto.Mascara,
            placeHolder: questaoDto.PlaceHolder,
            nomeComponente: questaoDto.NomeComponente
        );

        return await questaoRepositorio.CriarAsync(questao, cancellationToken: cancellationToken);
    }
}