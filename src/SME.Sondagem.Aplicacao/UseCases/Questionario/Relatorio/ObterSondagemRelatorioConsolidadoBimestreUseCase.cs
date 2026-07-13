using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Aplicacao.Interfaces.Services;
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
        IRepositorioElasticTurma repositorioElasticTurma,
        IAbrangenciaService abrangenciaService) : base(repositorioSondagem, repositorioElasticTurma, abrangenciaService)
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
        var totaisPorBimestre = respostas
            .Where(r => !string.Equals(r.OpcaoRespostaDescricao, "Sem preenchimento", StringComparison.OrdinalIgnoreCase))
            .GroupBy(r => r.BimestreId ?? 0)
            .ToDictionary(g => g.Key, g => g.Count());

        var questaoDto = ConstruirQuestaoDto(
            questaoId,
            questaoNome,
            respostas,
            processarOpcao: (opcao, respostasQuestao, total) =>
                ConstruirRespostaDto(opcao, respostasQuestao, total,
                    (dto, respostasOpcao, _) => dto.Bimestres = AgruparPorBimestre(respostasOpcao, totaisPorBimestre, _bimestresReferencia)),
            adicionarTotais: (dto, _, _) =>
                dto.TotaisPorBimestre = SomarTotaisPorBimestre(dto.Respostas, _bimestresReferencia));

        AjustarPercentuaisColunaPorBimestre(questaoDto, _bimestresReferencia);

        return questaoDto;
    }

    internal static List<RelatorioConsolidadoBimestreDto> AgruparPorBimestre(
        List<RelatorioRespostaAlunoDto> respostas,
        Dictionary<int, int> totaisPorBimestre,
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
                Percentual = CalcularPercentual(grupos.GetValueOrDefault(b.Id), totaisPorBimestre.GetValueOrDefault(b.Id))
            }).ToList();

        if (grupos.TryGetValue(0, out int qtdNaoInformado) && qtdNaoInformado > 0)
        {
            lista.Add(new RelatorioConsolidadoBimestreDto
            {
                Bimestre = "",
                Quantidade = qtdNaoInformado,
                Percentual = CalcularPercentual(qtdNaoInformado, totaisPorBimestre.GetValueOrDefault(0))
            });
        }

        return lista;
    }

    private static void AjustarPercentuaisColunaPorBimestre(
        RelatorioConsolidadoQuestaoDto questao,
        IEnumerable<BimestreExibicao> bimestresReferencia)
    {
        var respostas = (questao.Respostas ?? []).ToList();
        if (respostas.Count == 0) return;

        foreach (var bimestre in bimestresReferencia.OrderBy(b => b.Id))
        {
            var celulas = respostas
                .Select(r => r.Bimestres?.FirstOrDefault(b => b.Bimestre == bimestre.Descricao))
                .Where(c => c is not null)
                .Cast<RelatorioConsolidadoBimestreDto>()
                .ToList();

            int totalBimestre = celulas.Sum(c => c.Quantidade);
            if (totalBimestre == 0) continue;

            var exatos = celulas.Select(c => (double)c.Quantidade / totalBimestre * 100).ToList();
            var pisos = exatos.Select(e => Math.Truncate(e * 100) / 100).ToList();

            int qtdAjustes = (int)Math.Round((100.0 - pisos.Sum()) * 100);

            var indicesAjuste = exatos
                .Select((e, i) => (frac: e - pisos[i], i))
                .OrderByDescending(x => x.frac)
                .ThenBy(x => x.i)
                .Take(Math.Max(0, qtdAjustes))
                .Select(x => x.i)
                .ToHashSet();

            for (int i = 0; i < celulas.Count; i++)
                celulas[i].Percentual = Math.Round(pisos[i] + (indicesAjuste.Contains(i) ? 0.01 : 0), 2);
        }
    }

    private static List<RelatorioConsolidadoBimestreDto> SomarTotaisPorBimestre(
        IEnumerable<RelatorioConsolidadoRespostaDto>? respostas,
        IEnumerable<BimestreExibicao> bimestresReferencia)
    {
        var respostasList = (respostas ?? []).ToList();

        return bimestresReferencia
            .OrderBy(b => b.Id)
            .Select(b =>
            {
                var quantidade = respostasList
                    .SelectMany(r => r.Bimestres ?? [])
                    .Where(bim => bim.Bimestre == b.Descricao)
                    .Sum(bim => bim.Quantidade);

                return new RelatorioConsolidadoBimestreDto
                {
                    Bimestre = b.Descricao,
                    Quantidade = quantidade,
                    Percentual = quantidade > 0 ? 100.0 : 0.0
                };
            }).ToList();
    }
}
