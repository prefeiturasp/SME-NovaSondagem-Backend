using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class CriarQuestionarioUseCase : ICriarQuestionarioUseCase
{
    private readonly IRepositorioQuestionario questionarioRepositorio;

    public CriarQuestionarioUseCase(IRepositorioQuestionario questionarioRepositorio)
    {
        this.questionarioRepositorio = questionarioRepositorio;
    }

    public async Task<long> ExecutarAsync(QuestionarioDto questionarioDto, CancellationToken cancellationToken = default)
    {
        var questionario = new SME.Sondagem.Dominio.Entidades.Questionario.Questionario(
            questionarioDto.Nome,
            questionarioDto.Tipo,
            questionarioDto.AnoLetivo,
            questionarioDto.ComponenteCurricularId,
            questionarioDto.ProficienciaId,
            questionarioDto.SondagemId,
            questionarioDto.ModalidadeId,
            questionarioDto.SerieAno
        )
        {
            CriadoEm = questionarioDto.CriadoEm,
            CriadoPor = questionarioDto.CriadoPor,
            CriadoRF = questionarioDto.CriadoRF,
            AlteradoEm = questionarioDto.AlteradoEm,
            AlteradoPor = questionarioDto.AlteradoPor,
            AlteradoRF = questionarioDto.AlteradoRF
        };

        return await questionarioRepositorio.CriarAsync(questionario, cancellationToken: cancellationToken);
    }
}