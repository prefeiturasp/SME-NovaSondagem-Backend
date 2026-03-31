using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dominio.Entidades.Sondagem;

public class RespostaAluno : EntidadeBase
{
    public RespostaAluno(int sondagemId, int alunoId, int questaoId, int? opcaoRespostaId, DateTime dataResposta,
        int turmaId,int ueId,int dreId,int anoLetivo, int modalidadeId, int? bimestreId = null)
    {
        SondagemId = sondagemId;
        AlunoId = alunoId;
        QuestaoId = questaoId;
        OpcaoRespostaId = opcaoRespostaId;
        DataResposta = dataResposta;
        BimestreId = bimestreId;
        TurmaId = turmaId;
        UeId = ueId;
        DreId = dreId;
        AnoLetivo = anoLetivo;
        ModalidadeId = modalidadeId;
    }

    public int SondagemId { get; private set; }
    public int AlunoId { get; private set; }
    public int QuestaoId { get; private set; }
    public int? OpcaoRespostaId { get; private set; }
    public DateTime DataResposta { get; private set; }
    public int? BimestreId { get; private set; }
    public int? TurmaId { get; set; }
    public int? UeId { get; set; }
    public int? DreId { get; set; }
    public int? AnoLetivo { get; set; }
    public int? ModalidadeId { get; set; }

    public void AtualizarResposta(int? opcaoRespostaId, DateTime dataResposta,int turmaId,int dreId, int ueId,int modalidadeId)
    {
        OpcaoRespostaId = opcaoRespostaId;
        DataResposta = dataResposta;
        TurmaId = turmaId;
        DreId = dreId;
        UeId = ueId;
        ModalidadeId = modalidadeId;
    }

    // Navegação
    public virtual Sondagem Sondagem { get; private set; } = null!;
    public virtual Questao Questao { get; private set; } = null!;
    public virtual OpcaoResposta OpcaoResposta { get; private set; } = null!;
    public virtual Bimestre? Bimestre { get; private set; } = null!;
}