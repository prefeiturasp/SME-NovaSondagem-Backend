namespace SME.Sondagem.Dominio.Entidades.Sondagem
{
    public class SondagemPeriodoBimestre : EntidadeBase
    {
        public SondagemPeriodoBimestre(int sondagemId, int bimestreId, DateTime dataInicio, DateTime dataFim)
        {
            SondagemId = sondagemId;
            BimestreId = bimestreId;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }

        public int SondagemId { get; private set; }
        public int BimestreId { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }

        public virtual Sondagem Sondagem { get; private set; } = null!;
        public virtual Bimestre Bimestre { get; private set; } = null!;
    }
}
