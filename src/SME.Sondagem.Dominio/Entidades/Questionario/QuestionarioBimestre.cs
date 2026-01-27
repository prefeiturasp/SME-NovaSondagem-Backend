namespace SME.Sondagem.Dominio.Entidades.Questionario;

public class QuestionarioBimestre : EntidadeBase
{
    public QuestionarioBimestre(int questionarioId, int bimestreId)
    {
        ValidarEAplicar(questionarioId, bimestreId);
    }

    public int QuestionarioId { get; private set; }
    public int BimestreId { get; private set; }

    // Navegação
    public virtual Questionario Questionario { get; private set; } = null!;
    public virtual Bimestre Bimestre { get; private set; } = null!;

    public void Atualizar(int questionarioId, int bimestreId)
    {
        ValidarEAplicar(questionarioId, bimestreId);
    }

    private void ValidarEAplicar(int questionarioId, int bimestreId)
    {
        if (questionarioId <= 0)
            throw new ArgumentException("Questionário ID deve ser maior que zero.", nameof(questionarioId));

        if (bimestreId <= 0)
            throw new ArgumentException("Bimestre ID deve ser maior que zero.", nameof(bimestreId));

        QuestionarioId = questionarioId;
        BimestreId = bimestreId;
    }
}