using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using RacaDominio = SME.Sondagem.Dominio.Entidades.RacaCor;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoRacaUseCase : ObterSondagemRelatorioConsolidadoBase, IObterSondagemRelatorioConsolidadoRacaUseCase
{
    private IEnumerable<RacaDominio> _racasReferencia = [];

    public ObterSondagemRelatorioConsolidadoRacaUseCase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma) : base(repositorioSondagem, repositorioElasticTurma)
    {
    }

    protected override string TituloSemDados => "Relatório Consolidado por Raça - Sem Dados";
    protected override string ObterTitulo(int anoLetivo) => $"Relatório Consolidado de Sondagem por Raça - {anoLetivo}";

    public new async Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio(FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var racas = await RepositorioSondagem.RepositorioRacaCor.ListarAsync(cancellationToken) ?? [];

        if (filtro.RacaId.HasValue)
            _racasReferencia = racas.Where(r => r.Id == filtro.RacaId.Value);
        else
            _racasReferencia = racas;

        return await base.ObterSondagemRelatorio(filtro, cancellationToken);
    }

    protected override RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostas)
        => ConstruirQuestaoDto(
            questaoId,
            questaoNome,
            respostas,
            processarOpcao: (opcao, respostasQuestao, total) =>
                ConstruirRespostaDto(opcao, respostasQuestao, total,
                    (dto, respostasOpcao, totalQ) => dto.Racas = AgruparPorRaca(respostasOpcao, totalQ, _racasReferencia)),
            adicionarTotais: (dto, respostasQuestao, total) =>
                dto.TotaisPorRaca = AgruparPorRaca(respostasQuestao, total, _racasReferencia));

    internal static List<RelatorioConsolidadoRacaDto> AgruparPorRaca(List<RelatorioRespostaAlunoDto> respostas, int total, IEnumerable<RacaDominio> racasReferencia)
    {
        var grupos = respostas
            .GroupBy(r => r.RacaCorId ?? 0)
            .ToDictionary(g => g.Key, g => g.Count());

        var lista = racasReferencia
            .Select(r => new RelatorioConsolidadoRacaDto
            {
                Raca = r.Descricao,
                Quantidade = grupos.TryGetValue(r.Id, out int qtd) ? qtd : 0,
                Percentual = CalcularPercentual(grupos.TryGetValue(r.Id, out int q) ? q : 0, total)
            }).ToList();

        if (grupos.TryGetValue(0, out int qtdNaoInformado) && qtdNaoInformado > 0)
        {
            lista.Add(new RelatorioConsolidadoRacaDto
            {
                Raca = "NÃO PREENCHIDO",
                Quantidade = qtdNaoInformado,
                Percentual = CalcularPercentual(qtdNaoInformado, total)
            });
        }

        return [.. lista.OrderBy(r => r.Raca ?? "NÃO PREENCHIDO")];
    }
}
