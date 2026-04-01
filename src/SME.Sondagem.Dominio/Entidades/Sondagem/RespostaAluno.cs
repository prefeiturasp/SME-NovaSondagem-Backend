using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dominio.Entidades.Sondagem;

public class RespostaAluno : EntidadeBase
{
    public RespostaAluno(int sondagemId, int alunoId, int questaoId, int? opcaoRespostaId, DateTime dataResposta,
        string? turmaId, string? ueId, string? dreId, int? anoLetivo, string? modalidadeId,string? raca, string? genero,int? bimestreId = null)
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
        Raca = raca;
        Genero = genero;
        ModalidadeId = modalidadeId;
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

    public void AtualizarResposta(int? opcaoRespostaId, DateTime dataResposta,string? turmaId, string? ueId, string? dreId, int? anoLetivo, string? raca, string? genero,string? modalidadeId)
    {
        OpcaoRespostaId = opcaoRespostaId;
        DataResposta = dataResposta;
        TurmaId = TurmaId is null && turmaId is not null ? turmaId : TurmaId;
        UeId = UeId is null && ueId is not null ? ueId : UeId;
        DreId = DreId is null && dreId is not null ? dreId : DreId;
        AnoLetivo = AnoLetivo is null && anoLetivo is not null ? anoLetivo : AnoLetivo;
        Raca = Raca is null && raca is not null ? raca : Raca;
        Genero = Genero is null && genero is not null ? genero : Genero;
        ModalidadeId = ModalidadeId is null && modalidadeId is not null ? modalidadeId : ModalidadeId;
    }

    // Navegação
    public virtual Sondagem Sondagem { get; private set; } = null!;
    public virtual Questao Questao { get; private set; } = null!;
    public virtual OpcaoResposta OpcaoResposta { get; private set; } = null!;
    public virtual Bimestre? Bimestre { get; private set; } = null!;
}