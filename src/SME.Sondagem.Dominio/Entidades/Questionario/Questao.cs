using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class Questao : EntidadeBase
{
    public int QuestionarioId { get; private set; }
    public int Ordem { get; private set; }
    public string Nome { get; private set; }
    public string Observacao { get; private set; }
    public bool Obrigatorio { get; private set; }
    public TipoQuestao Tipo { get; private set; }
    public string Opcionais { get; private set; }
    public bool SomenteLeitura { get; private set; }
    public int Dimensao { get; private set; }
    public int? GrupoQuestoesId { get; private set; }
    public int? Tamanho { get; private set; }
    public string? Mascara { get; private set; }
    public string? PlaceHolder { get; private set; }
    public string? NomeComponente { get; private set; }

    // Navegação
    public virtual Questionario Questionario { get; private set; } = null!;
    public virtual GrupoQuestoes? GrupoQuestoes { get; private set; } = null!;
    public virtual ICollection<QuestaoOpcaoResposta> QuestaoOpcoes { get; private set; } = new List<QuestaoOpcaoResposta>();
    public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();

    private Questao()
    {
        Nome = string.Empty;
        Observacao = string.Empty;
        Opcionais = string.Empty;
    }

    public Questao(
        int questionarioId,
        int ordem,
        string nome,
        string observacao,
        bool obrigatorio,
        TipoQuestao tipo,
        string opcionais,
        bool somenteLeitura,
        int dimensao,
        int? grupoQuestoesId = null,
        int? tamanho = null,
        string? mascara = null,
        string? placeHolder = null,
        string? nomeComponente = null)
    {
        QuestionarioId = questionarioId;
        Ordem = ordem;
        Nome = nome;
        Observacao = observacao;
        Obrigatorio = obrigatorio;
        Tipo = tipo;
        Opcionais = opcionais;
        SomenteLeitura = somenteLeitura;
        Dimensao = dimensao;
        GrupoQuestoesId = grupoQuestoesId;
        Tamanho = tamanho;
        Mascara = mascara;
        PlaceHolder = placeHolder;
        NomeComponente = nomeComponente;
    }

    public void Atualizar(
        int questionarioId,
        int? grupoQuestoesId,
        int ordem,
        string nome,
        string observacao,
        bool obrigatorio,
        TipoQuestao tipo,
        string opcionais,
        bool somenteLeitura,
        int dimensao,
        int? tamanho,
        string? mascara,
        string? placeHolder,
        string? nomeComponente)
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