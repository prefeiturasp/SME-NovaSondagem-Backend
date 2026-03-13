using SME.Sondagem.Dominio.Entidades.Sondagem;

namespace SME.Sondagem.Infrastructure.Dtos.Questionario
{
    public record ContextoColunaDto(
        Dominio.Entidades.Sondagem.Sondagem SondagemAtiva,
        long QuestaoIdPrincipal,
        bool ExibirBimestreNaDescricaoColuna,
        Dictionary<(int CodigoAluno, int? BimestreId, long QuestaoId), RespostaAluno> RespostasAlunosPorQuestoes,
        Dictionary<int, string> DescricoesBimestre,
        bool EhRelatorio = false
    );
}
