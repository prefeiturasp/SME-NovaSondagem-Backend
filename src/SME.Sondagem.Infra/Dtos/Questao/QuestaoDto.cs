using SME.Sondagem.Dominio.Enums;

namespace SME.Sondagem.Infrastructure.Dtos.Questao
{
    public class QuestaoDto
    {
        public int Id { get; set; }
        public int QuestionarioId { get; set; }
        public int? GrupoQuestoesId { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public bool Obrigatorio { get; set; }
        public TipoQuestao Tipo { get; set; }
        public string Opcionais { get; set; } = string.Empty;
        public bool SomenteLeitura { get; set; }
        public int Dimensao { get; set; }
        public int? Tamanho { get; set; }
        public string? Mascara { get; set; }
        public string? PlaceHolder { get; set; }
        public string? NomeComponente { get; set; }
        public DateTime CriadoEm { get; set; }
        public string CriadoPor { get; set; } = string.Empty;
        public string CriadoRF { get; set; } = string.Empty;
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? AlteradoRF { get; set; }
    }
}
