namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;

public interface IExcluirQuestaoUseCase
{
    Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}