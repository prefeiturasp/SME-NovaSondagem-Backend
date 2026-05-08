namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface ICorrigirAnoTurmaRespostasUseCase
{
    Task<CorrigirAnoTurmaRespostasOutput> ExecutarAsync(int pagina, int tamanhoLote, CancellationToken cancellationToken);
}

public sealed class CorrigirAnoTurmaRespostasOutput
{
    public int TotalDivergentes { get; set; }
    public int TotalCorrigidos { get; set; }
    public int TotalSemTurmaNoElastic { get; set; }
}
