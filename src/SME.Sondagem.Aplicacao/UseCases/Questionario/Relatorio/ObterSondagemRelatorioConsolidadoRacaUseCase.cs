using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoRacaUseCase : ObterSondagemRelatorioConsolidadoBase, IObterSondagemRelatorioConsolidadoRacaUseCase
{
    public ObterSondagemRelatorioConsolidadoRacaUseCase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma) : base(repositorioSondagem, repositorioElasticTurma)
    {
    }

    public async Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio([FromQuery] FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var respostas = await ObterRespostasFiltradasAsync(filtro, cancellationToken);

        if (respostas.Count == 0)
            return new RelatorioConsolidadoSondagemDto { Titulo = "Relatório Consolidado por Raça - Sem Dados" };

        var relatorio = new RelatorioConsolidadoSondagemDto
        {
            Titulo = $"Relatório Consolidado de Sondagem por Raça - {filtro.AnoLetivo}"
        };

        var agrupamentoPorQuestao = respostas
            .GroupBy(r => new { r.QuestaoId, r.QuestaoNome })
            .OrderBy(g => g.Key.QuestaoNome);

        relatorio.Questoes = [.. agrupamentoPorQuestao.Select(g => ProcessarQuestao(g.Key.QuestaoId, g.Key.QuestaoNome, [.. g]))];

        return relatorio;
    }

    private static RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostasQuestao)
    {
        int totalRespostasQuestao = respostasQuestao.Count;

        var questaoDto = new RelatorioConsolidadoQuestaoDto
        {
            QuestaoId = questaoId,
            QuestaoNome = questaoNome,
            TotalEstudantes = respostasQuestao.Select(r => r.AlunoId).Distinct().Count()
        };

        var primeiraComOpcoes = respostasQuestao.FirstOrDefault(r => r.OpcoesDisponiveis != null && r.OpcoesDisponiveis.Any());
        var opcoesDisponiveis = primeiraComOpcoes?.OpcoesDisponiveis?.OrderBy(o => o.Ordem).ToList()
                                ?? [];

        var listaRespostas = opcoesDisponiveis
            .Select(opcao => ProcessarRespostaOpcao(opcao, respostasQuestao, totalRespostasQuestao))
            .ToList();

        questaoDto.Respostas = listaRespostas;
        questaoDto.PercentualTotal = listaRespostas.Sum(r => r.Percentual);

        questaoDto.TotaisPorRaca = ObterTotaisPorRaca(respostasQuestao, totalRespostasQuestao);

        return questaoDto;
    }

    private static RelatorioConsolidadoRespostaDto ProcessarRespostaOpcao(RelatorioOpcaoRespostaDto opcao, List<RelatorioRespostaAlunoDto> respostasQuestao, int totalRespostasQuestao)
    {
        var respostasDestaOpcao = respostasQuestao.Where(r => r.OpcaoRespostaId == opcao.Id).ToList();
        int totalOpcao = respostasDestaOpcao.Count;

        return new RelatorioConsolidadoRespostaDto
        {
            Resposta = opcao.Descricao,
            Ordem = opcao.Ordem,
            CorFundo = opcao.CorFundo,
            CorTexto = opcao.CorTexto,
            Total = totalOpcao,
            Percentual = CalcularPercentual(totalOpcao, totalRespostasQuestao),
            Racas = ObterTotaisPorRaca(respostasDestaOpcao, totalRespostasQuestao)
        };
    }

    private static List<RelatorioConsolidadoRacaDto> ObterTotaisPorRaca(List<RelatorioRespostaAlunoDto> respostas, int totalRespostasQuestao)
    {
        return respostas
            .GroupBy(r => r.RacaCor?.Descricao ?? "Não Informado")
            .Select(g => new RelatorioConsolidadoRacaDto
            {
                Raca = g.Key,
                Quantidade = g.Count(),
                Percentual = CalcularPercentual(g.Count(), totalRespostasQuestao)
            })
            .OrderBy(r => r.Raca)
            .ToList();
    }
}
