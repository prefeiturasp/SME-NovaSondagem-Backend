using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Questao;

namespace SME.Sondagem.Aplicacao.UseCases.Questao;

public class AtualizarQuestaoUseCase : IAtualizarQuestaoUseCase
{
    private readonly IRepositorioQuestao questaoRepositorio;

    public AtualizarQuestaoUseCase(IRepositorioQuestao questaoRepositorio)
    {
        this.questaoRepositorio = questaoRepositorio;
    }

    public async Task<QuestaoDto?> ExecutarAsync(long id, QuestaoDto questaoDto, CancellationToken cancellationToken = default)
    {
        var questaoExistente = await questaoRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (questaoExistente == null)
            return null;

        questaoExistente.Atualizar(
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
        questaoExistente.AlteradoEm = DateTime.Now;
        questaoExistente.AlteradoPor = questaoDto.AlteradoPor;
        questaoExistente.AlteradoRF = questaoDto.AlteradoRF;

        var sucesso = await questaoRepositorio.AtualizarAsync(questaoExistente, cancellationToken: cancellationToken);
        
        if (!sucesso)
            return null;

        return new QuestaoDto
        {
            Id = questaoExistente.Id,
            QuestionarioId = questaoExistente.QuestionarioId,
            GrupoQuestoesId = questaoExistente.GrupoQuestoesId,
            Ordem = questaoExistente.Ordem,
            Nome = questaoExistente.Nome,
            Observacao = questaoExistente.Observacao,
            Obrigatorio = questaoExistente.Obrigatorio,
            Tipo = questaoExistente.Tipo,
            Opcionais = questaoExistente.Opcionais,
            SomenteLeitura = questaoExistente.SomenteLeitura,
            Dimensao = questaoExistente.Dimensao,
            Tamanho = questaoExistente.Tamanho,
            Mascara = questaoExistente.Mascara,
            PlaceHolder = questaoExistente.PlaceHolder,
            NomeComponente = questaoExistente.NomeComponente,
            CriadoEm = questaoExistente.CriadoEm,
            CriadoPor = questaoExistente.CriadoPor,
            CriadoRF = questaoExistente.CriadoRF,
            AlteradoEm = questaoExistente.AlteradoEm,
            AlteradoPor = questaoExistente.AlteradoPor,
            AlteradoRF = questaoExistente.AlteradoRF
        };
    }
}