using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class ObterQuestionarioPorIdUseCase : IObterQuestionarioPorIdUseCase
{
    private readonly IRepositorioQuestionario questionarioRepositorio;

    public ObterQuestionarioPorIdUseCase(IRepositorioQuestionario questionarioRepositorio)
    {
        this.questionarioRepositorio = questionarioRepositorio;
    }

    public async Task<QuestionarioDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var questionario = await questionarioRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (questionario == null)
            return null;

        return new QuestionarioDto
        {
            Id = questionario.Id,
            Nome = questionario.Nome,
            Tipo = questionario.Tipo,
            AnoLetivo = questionario.AnoLetivo,
            ProficienciaId = questionario.ProficienciaId,
            SondagemId = questionario.SondagemId,
            ModalidadeId = questionario.ModalidadeId,
            SerieAno = questionario.SerieAno,
            ComponenteCurricularId = questionario.ComponenteCurricularId,
            CriadoEm = questionario.CriadoEm,
            CriadoPor = questionario.CriadoPor,
            CriadoRF = questionario.CriadoRF,
            AlteradoEm = questionario.AlteradoEm,
            AlteradoPor = questionario.AlteradoPor,
            AlteradoRF = questionario.AlteradoRF
        };
    }
}