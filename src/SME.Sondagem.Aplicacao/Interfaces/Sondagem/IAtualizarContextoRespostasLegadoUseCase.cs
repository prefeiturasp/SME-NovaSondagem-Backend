namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface IAtualizarContextoRespostasLegadoUseCase
{
    Task<int> ExecutarAsync(int respostaIdInicial, int pagina, int tamanhoLote, CancellationToken cancellationToken);
}
