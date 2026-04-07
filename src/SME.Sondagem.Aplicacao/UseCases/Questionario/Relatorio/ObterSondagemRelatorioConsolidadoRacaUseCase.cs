using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoRacaUseCase : IObterSondagemRelatorioConsolidadoRacaUseCase
{
    private readonly RepositoriosSondagem _repositorioSondagem;

    public ObterSondagemRelatorioConsolidadoRacaUseCase(RepositoriosSondagem repositorioSondagem)
    {
        _repositorioSondagem = repositorioSondagem ?? throw new ArgumentNullException(nameof(repositorioSondagem));
    }

    public async Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio([FromQuery] FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var respostasBrutas = await _repositorioSondagem.RepositorioRespostaAluno.ObterRespostasParaRelatorioConsolidadoAsync(filtro, cancellationToken);

        if (respostasBrutas == null || !respostasBrutas.Any())
            return new RelatorioConsolidadoSondagemDto { Titulo = "Relatório Consolidado - Sem Dados" };

        var relatorio = new RelatorioConsolidadoSondagemDto
        {
            Titulo = $"Relatório Consolidado de Sondagem - {filtro.AnoLetivo}"
        };

        var agrupamentoPorAno = respostasBrutas
            .GroupBy(r => r.AnoLetivo ?? filtro.AnoLetivo)
            .OrderBy(g => g.Key);

        var listaAnos = new List<RelatorioConsolidadoAnoDto>();

        foreach (var grupoAno in agrupamentoPorAno)
        {
            var anoDto = new RelatorioConsolidadoAnoDto
            {
                Ano = grupoAno.Key,
                TotalEstudantes = grupoAno.Select(r => r.AlunoId).Distinct().Count()
            };

            var primeiraRespostaComOpcoes = grupoAno.FirstOrDefault(r => r.OpcoesDisponiveis != null && r.OpcoesDisponiveis.Any());
            var opcoesDisponiveis = primeiraRespostaComOpcoes?.OpcoesDisponiveis?.OrderBy(o => o.Ordem).ToList() 
                                    ?? new List<RelatorioOpcaoRespostaDto>();

            var listaRespostas = new List<RelatorioConsolidadoRespostaDto>();
            int totalRespostasAno = grupoAno.Count();

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

                var agrupamentoRaca = respostasDestaOpcao
                    .GroupBy(r => r.RacaCor?.Descricao ?? r?.RacaCor?.Descricao ?? "Não Informado")
                    .Select(g => new RelatorioConsolidadoRacaDto
                    {
                        Raca = g.Key,
                        Quantidade = g.Count(),
                        Percentual = totalOpcao > 0 ? Math.Round((double)g.Count() / totalOpcao * 100, 2) : 0
                    })
                    .OrderBy(r => r.Raca)
                    .ToList();

                respostaDto.Racas = agrupamentoRaca;
                listaRespostas.Add(respostaDto);
            }

            anoDto.Respostas = listaRespostas;
            anoDto.PercentualTotal = listaRespostas.Sum(r => r.Percentual);
            listaAnos.Add(anoDto);
        }

        relatorio.Anos = listaAnos;

        return relatorio;
    }
}
