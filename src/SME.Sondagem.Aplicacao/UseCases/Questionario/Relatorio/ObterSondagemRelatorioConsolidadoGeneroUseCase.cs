using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoGeneroUseCase : ObterSondagemRelatorioConsolidadoBase, IObterSondagemRelatorioConsolidadoGeneroUseCase
{
    public ObterSondagemRelatorioConsolidadoGeneroUseCase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma) : base(repositorioSondagem, repositorioElasticTurma)
    {
    }

    public async Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio([FromQuery] FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var respostas = await ObterRespostasFiltradasAsync(filtro, cancellationToken);

        if (respostas.Count == 0)
            return new RelatorioConsolidadoSondagemDto { Titulo = "Relatório Consolidado por Gênero - Sem Dados" };

        return ConstruirRelatorio(
            $"Relatório Consolidado de Sondagem por Gênero - {filtro.AnoLetivo}",
            respostas,
            ProcessarQuestao);
    }

    private static RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostas)
        => ConstruirQuestaoDto(
            questaoId,
            questaoNome,
            respostas,
            processarOpcao: (opcao, respostasQuestao, total) =>
                ConstruirRespostaDto(opcao, respostasQuestao, total,
                    (dto, respostasOpcao, totalQ) => dto.Generos = AgruparPorGenero(respostasOpcao, totalQ)),
            adicionarTotais: (dto, respostasQuestao, total) =>
                dto.TotaisPorGenero = AgruparPorGenero(respostasQuestao, total));
}
