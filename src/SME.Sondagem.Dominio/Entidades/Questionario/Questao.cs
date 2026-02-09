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
    public int? QuestaoVinculoId { get; }
    public int? Tamanho { get; private set; }
    public string? Mascara { get; private set; }
    public string? PlaceHolder { get; private set; }
    public string? NomeComponente { get; private set; }

    // Navegação
    public virtual Questionario Questionario { get; private set; } = null!;
    public virtual GrupoQuestoes? GrupoQuestoes { get; private set; } = null!;
    public virtual ICollection<QuestaoOpcaoResposta> QuestaoOpcoes { get; private set; } = new List<QuestaoOpcaoResposta>();
    public virtual ICollection<RespostaAluno> Respostas { get; private set; } = new List<RespostaAluno>();
    public virtual ICollection<Questao> QuestoesVinculadas { get; private set; } = new List<Questao>();
    public virtual Questao? QuestaoVinculo { get; private set; } = null!;

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

    public void AtualizarQuestionarioId(int questionarioId)
    {
        QuestionarioId = questionarioId;
    }

    public void AtualizarGrupoQuestoesId(int? grupoQuestoesId)
    {
        GrupoQuestoesId = grupoQuestoesId;
    }

    public void AtualizarOrdem(int ordem)
    {
        Ordem = ordem;
    }

    public void AtualizarNome(string nome)
    {
        Nome = nome;
    }

    public void AtualizarObservacao(string observacao)
    {
        Observacao = observacao;
    }

    public void AtualizarObrigatorio(bool obrigatorio)
    {
        Obrigatorio = obrigatorio;
    }

    public void AtualizarTipo(TipoQuestao tipo)
    {
        Tipo = tipo;
    }

    public void AtualizarOpcionais(string opcionais)
    {
        Opcionais = opcionais;
    }

    public void AtualizarSomenteLeitura(bool somenteLeitura)
    {
        SomenteLeitura = somenteLeitura;
    }

    public void AtualizarDimensao(int dimensao)
    {
        Dimensao = dimensao;
    }

    public void AtualizarTamanho(int? tamanho)
    {
        Tamanho = tamanho;
    }

    public void AtualizarMascara(string? mascara)
    {
        Mascara = mascara;
    }

    public void AtualizarPlaceHolder(string? placeHolder)
    {
        PlaceHolder = placeHolder;
    }

    public void AtualizarNomeComponente(string? nomeComponente)
    {
        NomeComponente = nomeComponente;
    }
}