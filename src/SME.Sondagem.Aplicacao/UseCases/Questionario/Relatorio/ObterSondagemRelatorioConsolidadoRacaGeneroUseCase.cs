using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoRacaGeneroUseCase : ObterSondagemRelatorioConsolidadoBase, IObterSondagemRelatorioConsolidadoRacaGeneroUseCase
{
    public ObterSondagemRelatorioConsolidadoRacaGeneroUseCase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma) : base(repositorioSondagem, repositorioElasticTurma)
    {
    }

    protected override string TituloSemDados => "Relatório Consolidado por Gênero e Raça - Sem Dados";
    protected override string ObterTitulo(int anoLetivo) => $"Relatório Consolidado de Sondagem por Gênero e Raça - {anoLetivo}";

    protected override RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostas)
        => ConstruirQuestaoDto(
            questaoId,
            questaoNome,
            respostas,
            processarOpcao: (opcao, respostasQuestao, total) =>
                ConstruirRespostaDto(opcao, respostasQuestao, total,
                    (dto, respostasOpcao, totalQ) => dto.GenerosComRacas = AgruparGeneroComRacas(respostasOpcao, totalQ)));

    private static List<RelatorioConsolidadoGeneroRacaDto> AgruparGeneroComRacas(List<RelatorioRespostaAlunoDto> respostas, int totalRespostasQuestao)
        => [.. respostas
            .GroupBy(r => r.GeneroSexo?.Descricao ?? "Não Informado")
            .Select(g => ConstruirGeneroRaca(g, totalRespostasQuestao))
            .OrderBy(g => g.Genero)];

    private static RelatorioConsolidadoGeneroRacaDto ConstruirGeneroRaca(IGrouping<string, RelatorioRespostaAlunoDto> grupoGenero, int totalRespostasQuestao)
    {
        int totalGenero = grupoGenero.Count();

        return new RelatorioConsolidadoGeneroRacaDto
        {
            Genero = grupoGenero.Key,
            Sigla = grupoGenero.FirstOrDefault()?.GeneroSexo?.Sigla,
            TotalGenero = totalGenero,
            PercentualGenero = CalcularPercentual(totalGenero, totalRespostasQuestao),
            Racas = AgruparPorRaca([.. grupoGenero], totalGenero)
        };
    }
}
