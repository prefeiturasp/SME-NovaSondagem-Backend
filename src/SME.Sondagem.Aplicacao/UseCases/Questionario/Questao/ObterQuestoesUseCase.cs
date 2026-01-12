using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Questao;

public class ObterQuestoesUseCase : IObterQuestoesUseCase
{
    private readonly IRepositorioQuestao questaoRepositorio;

    public ObterQuestoesUseCase(IRepositorioQuestao questaoRepositorio)
    {
        this.questaoRepositorio = questaoRepositorio;
    }

    public async Task<IEnumerable<QuestaoDto>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var questoes = await questaoRepositorio.ObterTodosAsync(cancellationToken: cancellationToken);

        return questoes.Select(p => new QuestaoDto
        {
            Id = p.Id,
            QuestionarioId = p.QuestionarioId,
            GrupoQuestoesId = p.GrupoQuestoesId,
            Ordem = p.Ordem,
            Nome = p.Nome,
            Observacao = p.Observacao,
            Obrigatorio = p.Obrigatorio,
            Tipo = p.Tipo,
            Opcionais = p.Opcionais,
            SomenteLeitura = p.SomenteLeitura,
            Dimensao = p.Dimensao,
            Tamanho = p.Tamanho,
            Mascara = p.Mascara,
            PlaceHolder = p.PlaceHolder,
            NomeComponente = p.NomeComponente,
            CriadoEm = p.CriadoEm,
            CriadoPor = p.CriadoPor,
            CriadoRF = p.CriadoRF,
            AlteradoEm = p.AlteradoEm,
            AlteradoPor = p.AlteradoPor,
            AlteradoRF = p.AlteradoRF
        });
    }
}