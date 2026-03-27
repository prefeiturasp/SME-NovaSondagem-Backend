using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio.Integracao;

public class FiltroSolicitacaoRelatorioIntegracaoSgpBaseDto
{
    public TipoRelatorio TipoRelatorio { get; set; }
    public FormatoRelatorio ExtensaoRelatorio { get; set; }
    public string UsuarioQueSolicitou { get; set; }
    public StatusSolicitacao StatusSolicitacao { get; set; }
    public Guid CodigoCorrelacao { get; set; }
}