using SME.Sondagem.Aplicacao.Interfaces.QuestionarioBimestre;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.QuestionarioBimestre;

public class ExcluirVinculosPorQuestionarioUseCase : IExcluirVinculosPorQuestionarioUseCase
{
    private readonly IRepositorioQuestionarioBimestre _repositorio;

    public ExcluirVinculosPorQuestionarioUseCase(IRepositorioQuestionarioBimestre repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<bool> ExecutarAsync(int questionarioId, CancellationToken cancellationToken = default)
    {
        return await _repositorio.ExcluirPorQuestionarioIdAsync(questionarioId, cancellationToken);
    }
}