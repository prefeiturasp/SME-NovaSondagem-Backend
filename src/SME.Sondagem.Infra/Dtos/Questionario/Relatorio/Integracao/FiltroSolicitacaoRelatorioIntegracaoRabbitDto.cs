namespace SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio.Integracao;

public class FiltroSolicitacaoRelatorioIntegracaoRabbitDto : FiltroSolicitacaoRelatorioIntegracaoSgpBaseDto
{
    public object? FiltrosUsados { get; set; }
    public long SolicitacaoRelatorioId { get; set; }
}