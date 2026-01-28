using SME.Sondagem.Aplicacao.Interfaces.QuestionarioBimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;

namespace SME.Sondagem.Aplicacao.UseCases.QuestionarioBimestre;

public class ObterBimestresPorQuestionarioUseCase : IObterBimestresPorQuestionarioUseCase
{
    private readonly IRepositorioQuestionarioBimestre _repositorio;

    public ObterBimestresPorQuestionarioUseCase(IRepositorioQuestionarioBimestre repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<QuestionarioBimestreDto>> ExecutarAsync(int questionarioId, CancellationToken cancellationToken = default)
    {
        var questionariosBimestres = await _repositorio.ObterPorQuestionarioIdAsync(questionarioId, cancellationToken);

        return questionariosBimestres.Select(qb => new QuestionarioBimestreDto
        {
            Id = qb.Id,
            QuestionarioId = qb.QuestionarioId,
            BimestreId = qb.BimestreId,
            DescricaoBimestre = qb.Bimestre?.Descricao,
            CodBimestreEnsinoEol = qb.Bimestre?.CodBimestreEnsinoEol,
            CriadoEm = qb.CriadoEm,
            CriadoPor = qb.CriadoPor,
            CriadoRF = qb.CriadoRF,
            AlteradoEm = qb.AlteradoEm,
            AlteradoPor = qb.AlteradoPor,
            AlteradoRF = qb.AlteradoRF
        });
    }
}