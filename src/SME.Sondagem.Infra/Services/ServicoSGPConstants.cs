using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Services;

[ExcludeFromCodeCoverage]
public static class ServicoSgpConstants
{
    public const string SERVICO = "servicoSGP";
    public const string URL_REGISTRAR_SOLICITACAO_RELATORIO = "v1/solicitacao-relatorio/salvar";
    public const string URL_SOLICITACAO_RELATORIO = "v1/solicitacao-relatorio/obter-solicitacao-relatorio";
    public const string URL_ABRANGENCIA_COMPLETA = "v1/abrangencias/integracoes/false/login/{0}/perfis/{1}/abrangencia-completa?anoLetivo={2}&modalidade={3}";
    public const string URL_PLANO_AEE_TURMA_EXISTE = "v1/plano-aee/integracoes/turma/{0}/existe";
}
