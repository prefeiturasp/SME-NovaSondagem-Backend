namespace SME.Sondagem.Aplicacao.Interfaces.ParametroSondagemQuestionario;

public interface IExcluirParametroSondagemQuestionarioUseCase
{
    Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}