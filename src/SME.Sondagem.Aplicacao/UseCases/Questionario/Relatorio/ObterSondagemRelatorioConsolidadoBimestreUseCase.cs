using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dominio.Strategies.Bimestre;
using SME.Sondagem.Dominio.ValueObjects;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoBimestreUseCase : ObterSondagemRelatorioConsolidadoBase, IObterSondagemRelatorioConsolidadoBimestreUseCase
{
    private IEnumerable<BimestreExibicao> _bimestresReferencia = [];

    public ObterSondagemRelatorioConsolidadoBimestreUseCase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma) : base(repositorioSondagem, repositorioElasticTurma)
    {
    }

    protected override string TituloSemDados => "Relatório Consolidado por Bimestre - Sem Dados";
    protected override string ObterTitulo(int anoLetivo) => $"Relatório Consolidado de Sondagem por Bimestre - {anoLetivo}";

    public new async Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio(FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var bimestresCompletos = await RepositorioSondagem.RepositorioBimestre.ListarAsync(cancellationToken);

        _bimestresReferencia = BimestreModalidadeStrategyFactory.AplicarRegras(filtro.Modalidade, bimestresCompletos, filtro.BimestreId);

        return await base.ObterSondagemRelatorio(filtro, cancellationToken);
    }

    protected override RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostas)
    {
        var totalPorBimestre = ObterTotaisPorBimestre(respostas);

        return ConstruirQuestaoDto(
            questaoId,
            questaoNome,
            respostas,
            processarOpcao: (opcao, respostasQuestao, total) =>
                ConstruirRespostaDto(opcao, respostasQuestao, total,
                    (dto, respostasOpcao, totalQ) => dto.Bimestres = AgruparPorBimestre(respostasOpcao, totalPorBimestre, _bimestresReferencia)),
            adicionarTotais: (dto, respostasQuestao, total) =>
                dto.TotaisPorBimestre = AgruparPorBimestre(respostasQuestao, totalPorBimestre, _bimestresReferencia));
    }

    internal static List<RelatorioConsolidadoBimestreDto> AgruparPorBimestre(
        List<RelatorioRespostaAlunoDto> respostas,
        IReadOnlyDictionary<int, int> totalPorBimestre,
        IEnumerable<BimestreExibicao> bimestresReferencia)
    {
        var grupos = respostas
            .GroupBy(r => r.BimestreId ?? 0)
            .ToDictionary(g => g.Key, g => g.Count());

        var lista = bimestresReferencia
            .OrderBy(b => b.Id)
            .Select(b => new RelatorioConsolidadoBimestreDto
            {
                Bimestre = b.Descricao,
                Quantidade = grupos.GetValueOrDefault(b.Id),
                Percentual = CalcularPercentual(grupos.GetValueOrDefault(b.Id), totalPorBimestre.GetValueOrDefault(b.Id))
            }).ToList();

        if (grupos.TryGetValue(0, out int qtdNaoInformado) && qtdNaoInformado > 0)
        {
            lista.Add(new RelatorioConsolidadoBimestreDto
            {
                Bimestre = "",
                Quantidade = qtdNaoInformado,
                Percentual = CalcularPercentual(qtdNaoInformado, totalPorBimestre.GetValueOrDefault(0))
            });
        }

        return lista;
    }

    private static Dictionary<int, int> ObterTotaisPorBimestre(List<RelatorioRespostaAlunoDto> respostas)
        => respostas
            .GroupBy(r => r.BimestreId ?? 0)
            .ToDictionary(g => g.Key, g => g.Count());
}
