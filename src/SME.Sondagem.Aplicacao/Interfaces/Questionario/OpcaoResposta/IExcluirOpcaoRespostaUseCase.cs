namespace SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;

public interface IExcluirOpcaoRespostaUseCase
{
    Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}