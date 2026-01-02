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
        var questao = new SME.Sondagem.Dominio.Entidades.Questionario.Questao
        {
            QuestionarioId = questaoDto.QuestionarioId,
            GrupoQuestoesId = questaoDto.GrupoQuestoesId,
            Ordem = questaoDto.Ordem,
            Nome = questaoDto.Nome,
            Observacao = questaoDto.Observacao,
            Obrigatorio = questaoDto.Obrigatorio,
            Tipo = questaoDto.Tipo,
            Opcionais = questaoDto.Opcionais,
            SomenteLeitura = questaoDto.SomenteLeitura,
            Dimensao = questaoDto.Dimensao,
            Tamanho = questaoDto.Tamanho,
            Mascara = questaoDto.Mascara,
            PlaceHolder = questaoDto.PlaceHolder,
            NomeComponente = questaoDto.NomeComponente
        };

        return await questaoRepositorio.CriarAsync(questao, cancellationToken: cancellationToken);
    }
}