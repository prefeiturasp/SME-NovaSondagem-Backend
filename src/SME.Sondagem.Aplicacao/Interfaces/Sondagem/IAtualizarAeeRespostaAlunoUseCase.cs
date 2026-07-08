namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface IAtualizarAeeRespostaAlunoUseCase
{
    Task<int> ExecutarAsync(CancellationToken cancellationToken);
}
