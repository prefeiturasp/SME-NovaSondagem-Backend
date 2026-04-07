using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoGeneroUseCase : IObterSondagemRelatorioConsolidadoGeneroUseCase
{
    private readonly RepositoriosSondagem _repositorioSondagem;

    public ObterSondagemRelatorioConsolidadoGeneroUseCase(RepositoriosSondagem repositorioSondagem)
    {
        _repositorioSondagem = repositorioSondagem ?? throw new ArgumentNullException(nameof(repositorioSondagem));
    }

    public async Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio([FromQuery] FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var respostasBrutas = await _repositorioSondagem.RepositorioRespostaAluno.ObterRespostasParaRelatorioConsolidadoAsync(filtro, cancellationToken);

        if (respostasBrutas == null || !respostasBrutas.Any())
            return new RelatorioConsolidadoSondagemDto { Titulo = "Relatório Consolidado por Gênero - Sem Dados" };

        var relatorio = new RelatorioConsolidadoSondagemDto
        {
            Titulo = $"Relatório Consolidado de Sondagem por Gênero - {filtro.AnoLetivo}"
        };

        var agrupamentoPorQuestao = respostasBrutas
            .GroupBy(r => new { r.QuestaoId, r.QuestaoNome })
            .OrderBy(g => g.Key.QuestaoNome);

        var listaQuestoes = new List<RelatorioConsolidadoQuestaoDto>();

        foreach (var grupoQuestao in agrupamentoPorQuestao)
        {
            int totalRespostasQuestao = grupoQuestao.Count();

            var questaoDto = new RelatorioConsolidadoQuestaoDto
            {
                QuestaoId = grupoQuestao.Key.QuestaoId,
                QuestaoNome = grupoQuestao.Key.QuestaoNome,
                TotalEstudantes = grupoQuestao.Select(r => r.AlunoId).Distinct().Count()
            };

            var primeiraComOpcoes = grupoQuestao.FirstOrDefault(r => r.OpcoesDisponiveis != null && r.OpcoesDisponiveis.Any());
            var opcoesDisponiveis = primeiraComOpcoes?.OpcoesDisponiveis?.OrderBy(o => o.Ordem).ToList()
                                    ?? new List<RelatorioOpcaoRespostaDto>();

            var listaRespostas = new List<RelatorioConsolidadoRespostaDto>();

            foreach (var opcao in opcoesDisponiveis)
            {
                var respostasDestaOpcao = grupoQuestao.Where(r => r.OpcaoRespostaId == opcao.Id).ToList();
                int totalOpcao = respostasDestaOpcao.Count;

                var respostaDto = new RelatorioConsolidadoRespostaDto
                {
                    Resposta = opcao.Descricao,
                    Ordem = opcao.Ordem,
                    CorFundo = opcao.CorFundo,
                    CorTexto = opcao.CorTexto,
                    Total = totalOpcao,
                    Percentual = totalRespostasQuestao > 0 ? Math.Round((double)totalOpcao / totalRespostasQuestao * 100, 2) : 0
                };

                respostaDto.Generos = respostasDestaOpcao
                    .GroupBy(r => r.GeneroSexo?.Descricao ?? "Não Informado")
                    .Select(g => new RelatorioConsolidadoGeneroDto
                    {
                        Genero = g.Key,
                        Sigla = g.FirstOrDefault()?.GeneroSexo?.Sigla,
                        Quantidade = g.Count(),
                        Percentual = totalRespostasQuestao > 0 ? Math.Round((double)g.Count() / totalRespostasQuestao * 100, 2) : 0
                    })
                    .OrderBy(g => g.Genero)
                    .ToList();

                listaRespostas.Add(respostaDto);
            }

            questaoDto.Respostas = listaRespostas;
            questaoDto.PercentualTotal = listaRespostas.Sum(r => r.Percentual);

            questaoDto.TotaisPorGenero = grupoQuestao
                .GroupBy(r => r.GeneroSexo?.Descricao ?? "Não Informado")
                .Select(g => new RelatorioConsolidadoGeneroDto
                {
                    Genero = g.Key,
                    Sigla = g.FirstOrDefault()?.GeneroSexo?.Sigla,
                    Quantidade = g.Count(),
                    Percentual = totalRespostasQuestao > 0 ? Math.Round((double)g.Count() / totalRespostasQuestao * 100, 2) : 0
                })
                .OrderBy(g => g.Genero)
                .ToList();

            listaQuestoes.Add(questaoDto);
        }

        relatorio.Questoes = listaQuestoes;

        return relatorio;
    }
}
