using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class Questao : EntidadeBase
{
    private int _questionarioId;
    private int? _grupoQuestoesId;
    private int _ordem;
    private string _nome = string.Empty;
    private string _observacao = string.Empty;
    private bool _obrigatorio;
    private TipoQuestao _tipo;
    private string _opcionais = string.Empty;
    private bool _somenteLeitura;
    private int _dimensao;
    private int? _tamanho;
    private string? _mascara;
    private string? _placeHolder;
    private string? _nomeComponente;

    public required int QuestionarioId
    {
        get => _questionarioId;
        init => _questionarioId = value;
    }

    public int? GrupoQuestoesId
    {
        get => _grupoQuestoesId;
        init => _grupoQuestoesId = value;
    }

    public required int Ordem
    {
        get => _ordem;
        init => _ordem = value;
    }

    public required string Nome
    {
        get => _nome;
        init => _nome = value;
    }

    public required string Observacao
    {
        get => _observacao;
        init => _observacao = value;
    }

    public required bool Obrigatorio
    {
        get => _obrigatorio;
        init => _obrigatorio = value;
    }

    public required TipoQuestao Tipo
    {
        get => _tipo;
        init => _tipo = value;
    }

    public required string Opcionais
    {
        get => _opcionais;
        init => _opcionais = value;
    }

    public required bool SomenteLeitura
    {
        get => _somenteLeitura;
        init => _somenteLeitura = value;
    }

    public required int Dimensao
    {
        get => _dimensao;
        init => _dimensao = value;
    }

    public int? Tamanho
    {
        get => _tamanho;
        init => _tamanho = value;
    }

    public string? Mascara
    {
        get => _mascara;
        init => _mascara = value;
    }

    public string? PlaceHolder
    {
        get => _placeHolder;
        init => _placeHolder = value;
    }

    public string? NomeComponente
    {
        get => _nomeComponente;
        init => _nomeComponente = value;
    }

    // Navegação
    public virtual Questionario Questionario { get; private set; } = null!;
    public virtual GrupoQuestoes? GrupoQuestoes { get; private set; }
    public virtual ICollection<QuestaoOpcaoResposta> QuestaoOpcoes { get; private set; } = new List<QuestaoOpcaoResposta>();
    public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();

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
        _questionarioId = questionarioId;
        _grupoQuestoesId = grupoQuestoesId;
        _ordem = ordem;
        _nome = nome;
        _observacao = observacao;
        _obrigatorio = obrigatorio;
        _tipo = tipo;
        _opcionais = opcionais;
        _somenteLeitura = somenteLeitura;
        _dimensao = dimensao;
        _tamanho = tamanho;
        _mascara = mascara;
        _placeHolder = placeHolder;
        _nomeComponente = nomeComponente;
    }
}