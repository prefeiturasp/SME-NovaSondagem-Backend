using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.ValueObjects;

namespace SME.Sondagem.Dominio.Entidades.Sondagem;

public class RespostaAluno : EntidadeBase
{

    protected RespostaAluno() { }
    public RespostaAluno(int sondagemId, int alunoId, int questaoId, int? opcaoRespostaId,
        DateTime dataResposta, ContextoEducacional contexto)
    {
        SondagemId = sondagemId;
        AlunoId = alunoId;
        QuestaoId = questaoId;
        OpcaoRespostaId = opcaoRespostaId;
        DataResposta = dataResposta;
        BimestreId = contexto.BimestreId;
        TurmaId = contexto.TurmaId;
        UeId = contexto.UeId;
        DreId = contexto.DreId;
        AnoLetivo = contexto.AnoLetivo;
        Raca = contexto.Raca;
        Genero = contexto.Genero;
        ModalidadeId = contexto.ModalidadeId;
    }

    public int SondagemId { get; private set; }
    public int AlunoId { get; private set; }
    public int QuestaoId { get; private set; }
    public int? OpcaoRespostaId { get; private set; }
    public DateTime DataResposta { get; private set; }
    public int? BimestreId { get; private set; }
    public string? TurmaId { get; set; }
    public string? UeId { get; set; }
    public string? DreId { get; set; }
    public int? AnoLetivo { get; set; }
    public string? ModalidadeId { get; set; }
    public string? Raca { get; set; }
    public string? Genero { get; set; }

    public void AtualizarResposta(int? opcaoRespostaId, DateTime dataResposta, ContextoEducacional contexto)
    {
        OpcaoRespostaId = opcaoRespostaId;
        DataResposta = dataResposta;
        TurmaId = TurmaId is null && contexto.TurmaId is not null ? contexto.TurmaId : TurmaId;
        UeId = UeId is null && contexto.UeId is not null ? contexto.UeId : UeId;
        DreId = DreId is null && contexto.DreId is not null ? contexto.DreId : DreId;
        AnoLetivo = AnoLetivo is null && contexto.AnoLetivo is not null ? contexto.AnoLetivo : AnoLetivo;
        Raca = Raca is null && contexto.Raca is not null ? contexto.Raca : Raca;
        Genero = Genero is null && contexto.Genero is not null ? contexto.Genero : Genero;
        ModalidadeId = ModalidadeId is null && contexto.ModalidadeId is not null ? contexto.ModalidadeId : ModalidadeId;
    }

    // Navegação
    public virtual Sondagem Sondagem { get; private set; } = null!;
    public virtual Questao Questao { get; private set; } = null!;
    public virtual OpcaoResposta OpcaoResposta { get; private set; } = null!;
    public virtual Bimestre? Bimestre { get; private set; } = null!;
}