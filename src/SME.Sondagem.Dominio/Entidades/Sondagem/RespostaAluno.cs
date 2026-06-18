using SME.Sondagem.Dominio.Entidades.Questionario;
using SME.Sondagem.Dominio.ValueObjects;
using SME.Sondagem.Dominio.Entidades;

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
        AnoTurma = contexto.AnoTurma;
        RacaCorId = contexto.RacaCorId;
        GeneroSexoId = contexto.GeneroSexoId;
        Aee = contexto.Aee;
        Pap = contexto.Pap;
        Deficiente = contexto.Deficiente;
        ModalidadeId = contexto.ModalidadeId;
        SemestreId = contexto.SemestreId;
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
    public int? SemestreId { get; set; }

    public bool AtualizarResposta(int? opcaoRespostaId, DateTime dataResposta, ContextoEducacional contexto)
    {
        if (!PossuiAlteracoes(opcaoRespostaId, contexto))
            return false;

        OpcaoRespostaId = opcaoRespostaId;
        DataResposta = dataResposta;
        AtualizarContextoEducacional(contexto);
        return true;
    }

    private bool PossuiAlteracoes(int? opcaoRespostaId, ContextoEducacional contexto)
    {
        return OpcaoRespostaId != opcaoRespostaId
               || (TurmaId is null && contexto.TurmaId is not null)
               || (UeId is null && contexto.UeId is not null)
               || (DreId is null && contexto.DreId is not null)
               || (AnoLetivo is null && contexto.AnoLetivo is not null)
               || (RacaCorId is null && contexto.RacaCorId is not null)
               || (GeneroSexoId is null && contexto.GeneroSexoId is not null)
               || Pap != contexto.Pap
               || Aee != contexto.Aee
               || Deficiente != contexto.Deficiente
               || (ModalidadeId is null && contexto.ModalidadeId is not null)
               || (AnoTurma is null && contexto.AnoTurma is not null)
               || (SemestreId is null && contexto.SemestreId is not null);
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
        SemestreId ??= contexto.SemestreId;
    }

    public virtual Sondagem Sondagem { get; private set; } = null!;
    public virtual Questao Questao { get; private set; } = null!;
    public virtual OpcaoResposta OpcaoResposta { get; private set; } = null!;
    public virtual Bimestre? Bimestre { get; private set; } = null!;
    public virtual RacaCor? RacaCor { get; set; }
    public virtual GeneroSexo? GeneroSexo { get; set; }
    public virtual Semestre? Semestre { get; }
}
