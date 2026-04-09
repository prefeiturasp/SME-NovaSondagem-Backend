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
        RacaCorId = contexto.RacaCorId;
        GeneroSexoId = contexto.GeneroSexoId;
        Aee = contexto.Aee;
        Pap = contexto.Pap;
        Deficiente = contexto.Deficiente;
        ModalidadeId = contexto.ModalidadeId;
    }

    public int SondagemId { get; private set; }
    public int AlunoId { get; private set; }
    public int QuestaoId { get; private set; }
    public int? OpcaoRespostaId { get; private set; }
    public DateTime DataResposta { get; private set; }
    public int? BimestreId { get; private set; }
    public int? RacaCorId { get;  set; }
    public int? GeneroSexoId { get;  set; }
    public bool Aee { get;  set; }
    public bool Pap { get;  set; }
    public bool Deficiente { get;  set; }
    public string? TurmaId { get; set; }
    public string? UeId { get; set; }
    public string? DreId { get; set; }
    public int? AnoLetivo { get; set; }
    public int? AnoTurma { get; set; }
    public int? ModalidadeId { get; set; }

    public void AtualizarResposta(int? opcaoRespostaId, DateTime dataResposta, ContextoEducacional contexto)
    {
        OpcaoRespostaId = opcaoRespostaId;
        DataResposta = dataResposta;
        AtualizarContextoEducacional(contexto);
    }

    private void AtualizarContextoEducacional(ContextoEducacional contexto)
    {
        TurmaId ??= contexto.TurmaId;
        UeId ??= contexto.UeId;
        DreId ??= contexto.DreId;
        AnoLetivo ??= contexto.AnoLetivo;
        RacaCorId ??= contexto.RacaCorId;
        GeneroSexoId ??= contexto.GeneroSexoId;
        Pap = contexto.Pap;
        Aee = contexto.Aee;
        Deficiente = contexto.Deficiente;
        ModalidadeId ??= contexto.ModalidadeId;
        AnoTurma ??= contexto.AnoTurma;
    }

    public virtual Sondagem Sondagem { get; private set; } = null!;
    public virtual Questao Questao { get; private set; } = null!;
    public virtual OpcaoResposta OpcaoResposta { get; private set; } = null!;
    public virtual Bimestre? Bimestre { get; private set; } = null!;
    public virtual RacaCor? RacaCor { get; set; }
    public virtual GeneroSexo? GeneroSexo { get; set; }
}