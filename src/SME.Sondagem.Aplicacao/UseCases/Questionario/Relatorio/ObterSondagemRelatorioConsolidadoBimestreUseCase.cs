using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoBimestreUseCase : ObterSondagemRelatorioConsolidadoBase, IObterSondagemRelatorioConsolidadoBimestreUseCase
{
    public ObterSondagemRelatorioConsolidadoBimestreUseCase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma) : base(repositorioSondagem, repositorioElasticTurma)
    {
    }

    protected override string TituloSemDados => "Relatório Consolidado por Bimestre - Sem Dados";
    protected override string ObterTitulo(int anoLetivo) => $"Relatório Consolidado de Sondagem por Bimestre - {anoLetivo}";

    protected override RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostas)
        => ConstruirQuestaoDto(
            questaoId,
            questaoNome,
            respostas,
            processarOpcao: (opcao, respostasQuestao, total) =>
                ConstruirRespostaDto(opcao, respostasQuestao, total,
                    (dto, respostasOpcao, totalQ) => dto.Bimestres = AgruparPorBimestre(respostasOpcao, totalQ)),
            adicionarTotais: (dto, respostasQuestao, total) =>
                dto.TotaisPorBimestre = AgruparPorBimestre(respostasQuestao, total));

    internal static List<RelatorioConsolidadoBimestreDto> AgruparPorBimestre(List<RelatorioRespostaAlunoDto> respostas, int total)
        => [.. respostas
            .GroupBy(r => r.Bimestre?.Descricao ?? "Não Informado")
            .Select(g => new RelatorioConsolidadoBimestreDto
            {
                Bimestre = g.Key,
                Quantidade = g.Count(),
                Percentual = CalcularPercentual(g.Count(), total)
            })
            .OrderBy(g => g.Bimestre)];
}
