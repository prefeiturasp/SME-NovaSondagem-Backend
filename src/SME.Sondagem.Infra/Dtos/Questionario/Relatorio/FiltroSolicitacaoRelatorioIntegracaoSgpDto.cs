using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

public class FiltroSolicitacaoRelatorioIntegracaoSgpDto
{
    public string FiltrosUsados { get; set; }
    public TipoRelatorio Relatorio { get; set; }
    public FormatoRelatorio ExtensaoRelatorio { get; set; }
    public string UsuarioQueSolicitou { get; set; }
    public StatusSolicitacao StatusSolicitacao { get; set; }
}
