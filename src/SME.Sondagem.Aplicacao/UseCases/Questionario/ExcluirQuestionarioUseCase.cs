using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class ExcluirQuestionarioUseCase : IExcluirQuestionarioUseCase
{
    private readonly IRepositorioQuestionario questionarioRepositorio;

    public ExcluirQuestionarioUseCase(IRepositorioQuestionario questionarioRepositorio)
    {
        this.questionarioRepositorio = questionarioRepositorio;
    }

    public async Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var questionarioExistente = await questionarioRepositorio.ObterPorIdAsync(id, cancellationToken: cancellationToken);

        if (questionarioExistente == null)
            return false;

        return await questionarioRepositorio.ExcluirAsync(id, cancellationToken: cancellationToken);
    }
}