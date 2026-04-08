using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public abstract class ObterSondagemRelatorioConsolidadoBase
{
    protected readonly RepositoriosSondagem RepositorioSondagem;
    protected readonly IRepositorioElasticTurma RepositorioElasticTurma;

    protected ObterSondagemRelatorioConsolidadoBase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma)
    {
        RepositorioSondagem = repositorioSondagem ?? throw new ArgumentNullException(nameof(repositorioSondagem));
        RepositorioElasticTurma = repositorioElasticTurma ?? throw new ArgumentNullException(nameof(repositorioElasticTurma));
    }

    protected async Task<List<RelatorioRespostaAlunoDto>> ObterRespostasFiltradasAsync(FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var respostasBrutas = await RepositorioSondagem.RepositorioRespostaAluno.ObterRespostasParaRelatorioConsolidadoAsync(filtro, cancellationToken);
        var respostas = respostasBrutas?.ToList() ?? [];

        if (respostas.Count > 0 && filtro.AnoTurma != null && filtro.AnoTurma.Count > 0)
        {
            var anosTurmaStr = filtro.AnoTurma.Select(a => a.ToString()).ToList();
            var turmasElastic = await RepositorioElasticTurma.ObterTurmasPorAnoLetivoModalidadeEAnoTurmaAsync(
                filtro.AnoLetivo, 
                filtro.Modalidade, 
                anosTurmaStr, 
                cancellationToken);

            var codigosTurmaFiltro = turmasElastic.Select(t => t.CodigoTurma.ToString()).ToHashSet();

            respostas = [.. respostas.Where(r => r.TurmaId != null && codigosTurmaFiltro.Contains(r.TurmaId))];
        }

        return respostas;
    }

    protected static double CalcularPercentual(int quantidade, int total)
    {
        return total > 0 ? Math.Round((double)quantidade / total * 100, 2) : 0;
    }
}
