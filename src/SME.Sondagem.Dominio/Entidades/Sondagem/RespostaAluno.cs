using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dominio.Entidades.Sondagem;

public class RespostaAluno : EntidadeBase
{
    public RespostaAluno(int sondagemId, int alunoId, int questaoId, int opcaoRespostaId, DateTime dataResposta)
    {
        SondagemId = sondagemId;
        AlunoId = alunoId;
        QuestaoId = questaoId;
        OpcaoRespostaId = opcaoRespostaId;
        DataResposta = dataResposta;
    }

    public int SondagemId { get; private set; }
    public int AlunoId { get; private set; }
    public int QuestaoId { get; private set; }
    public int OpcaoRespostaId { get; private set; }
    public DateTime DataResposta { get; private set; }

    // Navegação
    public virtual Sondagem Sondagem { get; private set; } = null!;
    public virtual Aluno Aluno { get; private set; } = null!;
    public virtual Questao Questao { get; private set; } = null!;
    public virtual OpcaoResposta OpcaoResposta { get; private set; } = null!;
}