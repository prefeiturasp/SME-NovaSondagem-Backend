using SME.Sondagem.Dominio.Entidades.Sondagem;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    public class ContextoProcessamentoDto
    {
        public required IEnumerable<Dominio.Entidades.Questionario.Questao> QuestoesAtivas { get; set; }
        public Dominio.Entidades.Questionario.Questao? QuestaoLinguaPortuguesa { get; set; }
        public required List<ColunaQuestionarioDto> Colunas { get; set; }
        public required IEnumerable<dynamic> Alunos { get; set; }
        public required List<int> CodigosAlunos { get; set; }
        public required Dictionary<(long CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> RespostasAlunosPorQuestoes { get; set; }
        public required long QuestaoIdPrincipal { get; set; }
    }
}
