using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class AtualizarQuestionarioUseCase : IAtualizarQuestionarioUseCase
{
    private readonly IRepositorioQuestionario questionarioRepositorio;

    public AtualizarQuestionarioUseCase(IRepositorioQuestionario questionarioRepositorio)
    {
        this.questionarioRepositorio = questionarioRepositorio;
    }

    public async Task<QuestionarioDto?> ExecutarAsync(long id, QuestionarioDto questionarioDto, CancellationToken cancellationToken = default)
    {
        var questionarioExistente = await questionarioRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (questionarioExistente == null)
            return null;

        questionarioExistente.Atualizar(
            questionarioDto.Nome,
            questionarioExistente.Tipo,
            questionarioExistente.AnoLetivo,
            questionarioDto.ComponenteCurricularId,
            questionarioDto.ProficienciaId,
            questionarioExistente.SondagemId,
            questionarioDto.ModalidadeId,
            questionarioDto.SerieAno);

        var sucesso = await questionarioRepositorio.SalvarAsync(questionarioExistente, cancellationToken: cancellationToken);
        
        if (sucesso == 0)
            return null;

        return new QuestionarioDto
        {
            Id = questionarioExistente.Id,
            Nome = questionarioExistente.Nome,
            Tipo = questionarioExistente.Tipo,
            AnoLetivo = questionarioExistente.AnoLetivo,
            ProficienciaId = questionarioExistente.ProficienciaId,
            SondagemId = questionarioExistente.SondagemId,
            ModalidadeId = questionarioExistente.ModalidadeId,
            SerieAno = questionarioExistente.SerieAno,
            ComponenteCurricularId = questionarioExistente.ComponenteCurricularId,
            CriadoEm = questionarioExistente.CriadoEm,
            CriadoPor = questionarioExistente.CriadoPor,
            CriadoRF = questionarioExistente.CriadoRF,
            AlteradoEm = questionarioExistente.AlteradoEm,
            AlteradoPor = questionarioExistente.AlteradoPor,
            AlteradoRF = questionarioExistente.AlteradoRF
        };
    }
}