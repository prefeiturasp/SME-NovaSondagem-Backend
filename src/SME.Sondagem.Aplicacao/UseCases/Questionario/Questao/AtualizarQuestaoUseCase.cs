using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

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

        questaoExistente.AtualizarQuestionarioId(questaoDto.QuestionarioId);
        questaoExistente.AtualizarGrupoQuestoesId(questaoDto.GrupoQuestoesId);
        questaoExistente.AtualizarOrdem(questaoDto.Ordem);
        questaoExistente.AtualizarNome(questaoDto.Nome);
        questaoExistente.AtualizarObservacao(questaoDto.Observacao);
        questaoExistente.AtualizarObrigatorio(questaoDto.Obrigatorio);
        questaoExistente.AtualizarTipo(questaoDto.Tipo);
        questaoExistente.AtualizarOpcionais(questaoDto.Opcionais);
        questaoExistente.AtualizarSomenteLeitura(questaoDto.SomenteLeitura);
        questaoExistente.AtualizarDimensao(questaoDto.Dimensao);
        questaoExistente.AtualizarTamanho(questaoDto.Tamanho);
        questaoExistente.AtualizarMascara(questaoDto.Mascara);
        questaoExistente.AtualizarPlaceHolder(questaoDto.PlaceHolder);
        questaoExistente.AtualizarNomeComponente(questaoDto.NomeComponente);

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