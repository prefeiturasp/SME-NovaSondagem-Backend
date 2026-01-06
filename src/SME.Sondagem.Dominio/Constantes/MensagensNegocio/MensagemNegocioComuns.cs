using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dominio.Constantes.MensagensNegocio;

[ExcludeFromCodeCoverage]
public static class MensagemNegocioComuns
{
    public const string REQUISICAO_CANCELADA = "Requisição cancelada pelo cliente"; 
    public const string CICLO_NAO_ENCONTRADO = "Ciclo com ID {0} não encontrado";
    public const string COMPONENTE_CURRICULAR_NAO_ENCONTRADO = "Componente curricular com ID {0} não encontrado";
    public const string COMPONENTE_CURRICULAR_EOL_NAO_ENCONTRADO = "Componente curricular com código EOL {0} não encontrado";
    public const string PROEFICIENCIA_NAO_ENCONTRADA = "Proficiência com ID {0} não encontrada";
}
