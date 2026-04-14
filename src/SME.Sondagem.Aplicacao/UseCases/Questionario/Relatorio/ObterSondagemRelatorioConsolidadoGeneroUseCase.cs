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

    protected override string TituloSemDados => "Relatório Consolidado por Gênero - Sem Dados";
    protected override string ObterTitulo(int anoLetivo) => $"Relatório Consolidado de Sondagem por Gênero - {anoLetivo}";

    protected override RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostas)
        => ConstruirQuestaoDto(
            questaoId,
            questaoNome,
            respostas,
            processarOpcao: (opcao, respostasQuestao, total) =>
                ConstruirRespostaDto(opcao, respostasQuestao, total,
                    (dto, respostasOpcao, totalQ) => dto.Generos = AgruparPorGenero(respostasOpcao, totalQ)),
            adicionarTotais: (dto, respostasQuestao, total) =>
                dto.TotaisPorGenero = AgruparPorGenero(respostasQuestao, total));

    internal static List<RelatorioConsolidadoGeneroDto> AgruparPorGenero(List<RelatorioRespostaAlunoDto> respostas, int total)
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
}
