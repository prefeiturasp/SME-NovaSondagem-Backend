using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dominio.Entidades.Sondagem;

public class RespostaAluno : EntidadeBase
{
    public RespostaAluno(int sondagemId, int? alunoId, int questaoId, int? opcaoRespostaId, DateTime dataResposta, int? bimestreId = null)
    {
        SondagemId = sondagemId;
        AlunoId = alunoId;
        QuestaoId = questaoId;
        OpcaoRespostaId = opcaoRespostaId;
        DataResposta = dataResposta;
        BimestreId = bimestreId;
    }

    public int SondagemId { get; private set; }
    public int? AlunoId { get; private set; }
    public int QuestaoId { get; private set; }
    public int? OpcaoRespostaId { get; private set; }
    public DateTime DataResposta { get; private set; }
    public int? BimestreId { get; private set; }

    public void AtualizarResposta(int? opcaoRespostaId, DateTime dataResposta)
    {
        OpcaoRespostaId = opcaoRespostaId;
        DataResposta = dataResposta;
    }

    // Navegação
    public virtual Sondagem Sondagem { get; private set; } = null!;
    public virtual Questao Questao { get; private set; } = null!;
    public virtual OpcaoResposta OpcaoResposta { get; private set; } = null!;
    public virtual Bimestre? Bimestre { get; private set; } = null!;
}