using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dominio.Constantes.MensagensNegocio;

[ExcludeFromCodeCoverage]
public static class MensagemNegocioComuns
{
    public const string REQUISICAO_CANCELADA = "Requisição cancelada pelo cliente"; 
    public const string BIMESTRE_NAO_ENCONTRADO = "Bimestre com ID {0} não encontrado";
    public const string COMPONENTE_CURRICULAR_NAO_ENCONTRADO = "Componente curricular com ID {0} não encontrado";
    public const string COMPONENTE_CURRICULAR_EOL_NAO_ENCONTRADO = "Componente curricular com código EOL {0} não encontrado";
    public const string OPCAO_RESPOSTA_NAO_ENCONTRADA = "Opção resposta com ID {0} não encontrada";
    public const string PROEFICIENCIA_NAO_ENCONTRADA = "Proficiência com ID {0} não encontrada";
    public const string QUESTIONARIO_NAO_ENCONTRADO = "Questionário com ID {0} não encontrado";
    public const string QUESTAO_NAO_ENCONTRADA = "Questão com ID {0} não encontrada";
    public const string QUESTAO_OPCAO_RESPOSTA_NAO_ENCONTRADA = "Questão opção resposta com ID {0} não encontrada";
    public const string NENHUM_SONDAGEM_ATIVA_ENCONRADA = "Nenhuma sondagem ativa encontrada para salvar as respostas.";
    public const string SALVAR_SOMENTE_PARA_SONDAGEM_ATIVA = "As respostas só podem ser salvas para a sondagem ativa.";
    public const string NENHUM_REGISTRO_ENCONTRADO = "Nenhum registro encontrado";
    public const string INFORMAR_ID_MAIOR_QUE_ZERO = "Informe um valor maior que zero";
    public const string QUESTOES_NAO_PERTENCEM_A_UM_QUESTIONARIO = "As questões informadas não pertencem a um questionário válido.";
}
