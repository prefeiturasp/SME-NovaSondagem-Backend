using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoAnoUseCase : ObterSondagemRelatorioConsolidadoBase, IObterSondagemRelatorioConsolidadoAnoUseCase
{
    public ObterSondagemRelatorioConsolidadoAnoUseCase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma) : base(repositorioSondagem, repositorioElasticTurma)
    {
    }

    protected override string TituloSemDados => "Relatório Consolidado por Ano - Sem Dados";
    protected override string ObterTitulo(int anoLetivo) => $"Relatório Consolidado de Sondagem por Ano - {anoLetivo}";

    protected override RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostas)
        => ConstruirQuestaoDto(
            questaoId,
            questaoNome,
            respostas,
            processarOpcao: (opcao, respostasQuestao, total) =>
                ConstruirRespostaDto(opcao, respostasQuestao, total,
                    (dto, respostasOpcao, totalQ) => dto.AnosTurma = AgruparPorAnoTurma(respostasOpcao, totalQ)),
            adicionarTotais: (dto, respostasQuestao, total) =>
                dto.TotaisPorAnoTurma = AgruparPorAnoTurma(respostasQuestao, total));

    private static List<RelatorioConsolidadoAnoTurmaDto> AgruparPorAnoTurma(List<RelatorioRespostaAlunoDto> respostas, int total)
        => [.. respostas
            .Where(r => r.AnoTurma.HasValue)
            .GroupBy(r => r.AnoTurma!.Value)
            .Select(g => new RelatorioConsolidadoAnoTurmaDto
            {
                AnoTurma = g.Key,
                Quantidade = g.Count(),
                Percentual = CalcularPercentual(g.Count(), total)
            })
            .OrderBy(a => a.AnoTurma)];
}
