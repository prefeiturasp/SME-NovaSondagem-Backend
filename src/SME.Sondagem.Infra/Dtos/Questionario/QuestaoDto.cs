using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Infra.Dtos.Questionario;

public class QuestaoDto
{
    public QuestaoDto()
    {
        OpcoesRespostas = new List<OpcaoResposta>();
    }

    public QuestionarioDto Questionario { get; set; }
    public long QuestionarioId { get; set; }

    public int Ordem { get; set; }
    public string Nome { get; set; }
    public string Observacao { get; set; }
    public bool Obrigatorio { get; set; }
    public TipoQuestao Tipo { get; set; }
    public string Opcionais { get; set; }
    public bool SomenteLeitura { get; set; }
    public int Dimensao { get; set; }
    public int? Tamanho { get; set; }
    public string? Mascara { get; set; }
    public string? PlaceHolder { get; set; }
    public string? NomeComponente { get; set; }
    public List<OpcaoResposta> OpcoesRespostas { get; set; }
}
