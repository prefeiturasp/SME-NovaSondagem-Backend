namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface IAtualizarAeeRespostaAlunoUseCase
{
    Task<(int UltimoId, int TotalAtualizado)> ExecutarAsync(int ultimoIdInicial, CancellationToken cancellationToken);
}
