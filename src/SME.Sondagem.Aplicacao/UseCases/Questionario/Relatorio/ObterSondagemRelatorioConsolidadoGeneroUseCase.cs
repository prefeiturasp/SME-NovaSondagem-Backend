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

        var agrupamentoPorAno = respostasBrutas
            .GroupBy(r => r.AnoLetivo ?? filtro.AnoLetivo)
            .OrderBy(g => g.Key);

        var listaAnos = new List<RelatorioConsolidadoAnoDto>();

        foreach (var grupoAno in agrupamentoPorAno)
        {
            int totalRespostasAno = grupoAno.Count();

            var anoDto = new RelatorioConsolidadoAnoDto
            {
                Ano = grupoAno.Key,
                TotalEstudantes = grupoAno.Select(r => r.AlunoId).Distinct().Count()
            };

            var primeiraComOpcoes = grupoAno.FirstOrDefault(r => r.OpcoesDisponiveis != null && r.OpcoesDisponiveis.Any());
            var opcoesDisponiveis = primeiraComOpcoes?.OpcoesDisponiveis?.OrderBy(o => o.Ordem).ToList()
                                    ?? new List<RelatorioOpcaoRespostaDto>();

            var listaRespostas = new List<RelatorioConsolidadoRespostaDto>();

            foreach (var opcao in opcoesDisponiveis)
            {
                var respostasDestaOpcao = grupoAno.Where(r => r.OpcaoRespostaId == opcao.Id).ToList();
                int totalOpcao = respostasDestaOpcao.Count;

                var respostaDto = new RelatorioConsolidadoRespostaDto
                {
                    Resposta = opcao.Descricao,
                    Ordem = opcao.Ordem,
                    CorFundo = opcao.CorFundo,
                    CorTexto = opcao.CorTexto,
                    Total = totalOpcao,
                    Percentual = totalRespostasAno > 0 ? Math.Round((double)totalOpcao / totalRespostasAno * 100, 2) : 0
                };

                // Percentual por célula = quantidade_genero_nessa_opcao / totalRespostasAno
                respostaDto.Generos = respostasDestaOpcao
                    .GroupBy(r => r.GeneroSexo?.Descricao ?? "Não Informado")
                    .Select(g => new RelatorioConsolidadoGeneroDto
                    {
                        Genero = g.Key,
                        Sigla = g.FirstOrDefault()?.GeneroSexo?.Sigla,
                        Quantidade = g.Count(),
                        Percentual = totalRespostasAno > 0 ? Math.Round((double)g.Count() / totalRespostasAno * 100, 2) : 0
                    })
                    .OrderBy(g => g.Genero)
                    .ToList();

                listaRespostas.Add(respostaDto);
            }

            anoDto.Respostas = listaRespostas;
            anoDto.PercentualTotal = listaRespostas.Sum(r => r.Percentual);

            // Linha "Total" do rodapé: total por gênero em todas as opções
            anoDto.TotaisPorGenero = grupoAno
                .GroupBy(r => r.GeneroSexo?.Descricao ?? "Não Informado")
                .Select(g => new RelatorioConsolidadoGeneroDto
                {
                    Genero = g.Key,
                    Sigla = g.FirstOrDefault()?.GeneroSexo?.Sigla,
                    Quantidade = g.Count(),
                    Percentual = totalRespostasAno > 0 ? Math.Round((double)g.Count() / totalRespostasAno * 100, 2) : 0
                })
                .OrderBy(g => g.Genero)
                .ToList();

            listaAnos.Add(anoDto);
        }

        relatorio.Anos = listaAnos;

        return relatorio;
    }
}
