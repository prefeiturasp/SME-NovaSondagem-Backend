using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class Questao : EntidadeBase
{
    public required int QuestionarioId { get; init; }
    public required int Ordem { get; init; }
    public required string Nome { get; init; }
    public required string Observacao { get; init; }
    public required bool Obrigatorio { get; init; }
    public required TipoQuestao Tipo { get; init; }
    public required string Opcionais { get; init; }
    public required bool SomenteLeitura { get; init; }
    public required int Dimensao { get; init; }
    public int? GrupoQuestoesId { get; init; }
    public int? Tamanho { get; init; }
    public string? Mascara { get; init; }
    public string? PlaceHolder { get; init; }
    public string? NomeComponente { get; init; }

    // Navegação
    public virtual Questionario Questionario { get; private set; } = null!;
    public virtual GrupoQuestoes? GrupoQuestoes { get; private set; }
    public virtual ICollection<QuestaoOpcaoResposta> QuestaoOpcoes { get; private set; } = new List<QuestaoOpcaoResposta>();
    public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();

    public void Atualizar(int questionarioId, int? grupoQuestoesId, int ordem, string nome, string observacao, bool obrigatorio, TipoQuestao tipo, string opcionais, bool somenteLeitura, int dimensao, int? tamanho, string? mascara, string? placeHolder, string? nomeComponente)
    {
        QuestionarioId = questionarioId;
        GrupoQuestoesId = grupoQuestoesId;
        Ordem = ordem;
        Nome = nome;
        Observacao = observacao;
        Obrigatorio = obrigatorio;
        Tipo = tipo;
        Opcionais = opcionais;
        SomenteLeitura = somenteLeitura;
        Dimensao = dimensao;
        Tamanho = tamanho;
        Mascara = mascara;
        PlaceHolder = placeHolder;
        NomeComponente = nomeComponente;
    }
}