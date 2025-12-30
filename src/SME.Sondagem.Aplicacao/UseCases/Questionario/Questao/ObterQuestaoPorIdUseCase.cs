using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questao;

namespace SME.Sondagem.Aplicacao.UseCases.Questao;

public class ObterQuestaoPorIdUseCase : IObterQuestaoPorIdUseCase
{
    private readonly IRepositorioQuestao questaoRepositorio;

    public ObterQuestaoPorIdUseCase(IRepositorioQuestao questaoRepositorio)
    {
        this.questaoRepositorio = questaoRepositorio;
    }

    public async Task<QuestaoDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var questao = await questaoRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (questao == null)
            return null;

        return new QuestaoDto
        {
            Id = questao.Id,
            QuestionarioId = questao.QuestionarioId,
            GrupoQuestoesId = questao.GrupoQuestoesId,
            Ordem = questao.Ordem,
            Nome = questao.Nome,
            Observacao = questao.Observacao,
            Obrigatorio = questao.Obrigatorio,
            Tipo = questao.Tipo,
            Opcionais = questao.Opcionais,
            SomenteLeitura = questao.SomenteLeitura,
            Dimensao = questao.Dimensao,
            Tamanho = questao.Tamanho,
            Mascara = questao.Mascara,
            PlaceHolder = questao.PlaceHolder,
            NomeComponente = questao.NomeComponente,
            CriadoEm = questao.CriadoEm,
            CriadoPor = questao.CriadoPor,
            CriadoRF = questao.CriadoRF,
            AlteradoEm = questao.AlteradoEm,
            AlteradoPor = questao.AlteradoPor,
            AlteradoRF = questao.AlteradoRF
        };
    }
}