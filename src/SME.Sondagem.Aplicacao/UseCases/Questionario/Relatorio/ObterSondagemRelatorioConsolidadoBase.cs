using Microsoft.AspNetCore.Mvc;
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

    protected abstract string TituloSemDados { get; }
    protected abstract string ObterTitulo(int anoLetivo);
    protected abstract RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostas);

    public async Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio([FromQuery] FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var respostas = await ObterRespostasFiltradasAsync(filtro, cancellationToken);

        if (respostas.Count == 0)
            return new RelatorioConsolidadoSondagemDto { Titulo = TituloSemDados };

        return ConstruirRelatorio(ObterTitulo(filtro.AnoLetivo), respostas, ProcessarQuestao);
    }

    private async Task<List<RelatorioRespostaAlunoDto>> ObterRespostasFiltradasAsync(FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var respostasBrutas = await RepositorioSondagem.RepositorioRespostaAluno.ObterRespostasParaRelatorioConsolidadoAsync(filtro, cancellationToken);
        var respostas = respostasBrutas?.ToList() ?? [];

        if (respostas.Count > 0 && filtro.AnoTurma != null && filtro.AnoTurma.Count > 0)
            respostas = await FiltrarPorAnoTurmaAsync(respostas, filtro, cancellationToken);

        return respostas;
    }

    private static RelatorioConsolidadoSondagemDto ConstruirRelatorio(
        string titulo,
        List<RelatorioRespostaAlunoDto> respostas,
        Func<int, string, List<RelatorioRespostaAlunoDto>, RelatorioConsolidadoQuestaoDto> processarQuestao)
    {
        var relatorio = new RelatorioConsolidadoSondagemDto { Titulo = titulo };

        relatorio.Questoes = [.. respostas
            .GroupBy(r => new { r.QuestaoId, r.QuestaoNome })
            .OrderBy(g => g.Key.QuestaoNome)
            .Select(g => processarQuestao(g.Key.QuestaoId, g.Key.QuestaoNome, [.. g]))];

        return relatorio;
    }

    protected static RelatorioConsolidadoQuestaoDto ConstruirQuestaoDto(
        int questaoId,
        string questaoNome,
        List<RelatorioRespostaAlunoDto> respostasQuestao,
        Func<RelatorioOpcaoRespostaDto, List<RelatorioRespostaAlunoDto>, int, RelatorioConsolidadoRespostaDto> processarOpcao,
        Action<RelatorioConsolidadoQuestaoDto, List<RelatorioRespostaAlunoDto>, int>? adicionarTotais = null)
    {
        int total = respostasQuestao.Count;

        var questaoDto = new RelatorioConsolidadoQuestaoDto
        {
            QuestaoId = questaoId,
            QuestaoNome = questaoNome,
            TotalEstudantes = respostasQuestao.Select(r => r.AlunoId).Distinct().Count()
        };

        var opcoes = respostasQuestao
            .FirstOrDefault(r => r.OpcoesDisponiveis != null && r.OpcoesDisponiveis.Any())
            ?.OpcoesDisponiveis?.OrderBy(o => o.Ordem).ToList() ?? [];

        var listaRespostas = opcoes
            .Select(opcao => processarOpcao(opcao, respostasQuestao, total))
            .ToList();

        questaoDto.Respostas = listaRespostas;
        questaoDto.PercentualTotal = listaRespostas.Sum(r => r.Percentual);

        adicionarTotais?.Invoke(questaoDto, respostasQuestao, total);

        return questaoDto;
    }

    protected static RelatorioConsolidadoRespostaDto ConstruirRespostaDto(
        RelatorioOpcaoRespostaDto opcao,
        List<RelatorioRespostaAlunoDto> respostasQuestao,
        int totalRespostasQuestao,
        Action<RelatorioConsolidadoRespostaDto, List<RelatorioRespostaAlunoDto>, int>? adicionarAgrupamento = null)
    {
        var respostasDestaOpcao = respostasQuestao.Where(r => r.OpcaoRespostaId == opcao.Id).ToList();
        int totalOpcao = respostasDestaOpcao.Count;

        var dto = new RelatorioConsolidadoRespostaDto
        {
            Resposta = opcao.Descricao,
            Ordem = opcao.Ordem,
            CorFundo = opcao.CorFundo,
            CorTexto = opcao.CorTexto,
            Total = totalOpcao,
            Percentual = CalcularPercentual(totalOpcao, totalRespostasQuestao)
        };

        adicionarAgrupamento?.Invoke(dto, respostasDestaOpcao, totalRespostasQuestao);

        return dto;
    }

    protected static List<RelatorioConsolidadoRacaDto> AgruparPorRaca(List<RelatorioRespostaAlunoDto> respostas, int total)
        => [.. respostas
            .GroupBy(r => r.RacaCor?.Descricao ?? "Não Informado")
            .Select(g => new RelatorioConsolidadoRacaDto
            {
                Raca = g.Key,
                Quantidade = g.Count(),
                Percentual = CalcularPercentual(g.Count(), total)
            })
            .OrderBy(r => r.Raca)];

    protected static List<RelatorioConsolidadoGeneroDto> AgruparPorGenero(List<RelatorioRespostaAlunoDto> respostas, int total)
        => [.. respostas
            .GroupBy(r => r.GeneroSexo?.Descricao ?? "Não Informado")
            .Select(g => new RelatorioConsolidadoGeneroDto
            {
                Genero = g.Key,
                Sigla = g.FirstOrDefault()?.GeneroSexo?.Sigla,
                Quantidade = g.Count(),
                Percentual = CalcularPercentual(g.Count(), total)
            })
            .OrderBy(g => g.Genero)];

    protected static double CalcularPercentual(int quantidade, int total)
        => total > 0 ? Math.Round((double)quantidade / total * 100, 2) : 0;

    private async Task<List<RelatorioRespostaAlunoDto>> FiltrarPorAnoTurmaAsync(
        List<RelatorioRespostaAlunoDto> respostas,
        FiltroConsolidadoDto filtro,
        CancellationToken cancellationToken)
    {
        var anosTurmaStr = filtro.AnoTurma!.Select(a => a.ToString()).ToList();

        var turmasElastic = await RepositorioElasticTurma.ObterTurmasPorAnoLetivoModalidadeEAnoTurmaAsync(
            filtro.AnoLetivo,
            filtro.Modalidade,
            anosTurmaStr,
            cancellationToken);

        var codigosTurmaFiltro = turmasElastic.Select(t => t.CodigoTurma.ToString()).ToHashSet();

        return [.. respostas.Where(r => r.TurmaId != null && codigosTurmaFiltro.Contains(r.TurmaId))];
    }
}
