using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using BimestreDominio = SME.Sondagem.Dominio.Entidades.Bimestre;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoBimestreUseCase : ObterSondagemRelatorioConsolidadoBase, IObterSondagemRelatorioConsolidadoBimestreUseCase
{
    private IEnumerable<BimestreDominio> _bimestresReferencia = [];

    public ObterSondagemRelatorioConsolidadoBimestreUseCase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma) : base(repositorioSondagem, repositorioElasticTurma)
    {
    }

    protected override string TituloSemDados => "Relatório Consolidado por Bimestre - Sem Dados";
    protected override string ObterTitulo(int anoLetivo) => $"Relatório Consolidado de Sondagem por Bimestre - {anoLetivo}";

    public new async Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio(FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var bimestres = await RepositorioSondagem.RepositorioBimestre.ListarAsync(cancellationToken);

        if (filtro.BimestreId.HasValue)
            _bimestresReferencia = bimestres.Where(b => b.Id == filtro.BimestreId.Value);
        else
            _bimestresReferencia = bimestres;

        return await base.ObterSondagemRelatorio(filtro, cancellationToken);
    }

    protected override RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostas)
        => ConstruirQuestaoDto(
            questaoId,
            questaoNome,
            respostas,
            processarOpcao: (opcao, respostasQuestao, total) =>
                ConstruirRespostaDto(opcao, respostasQuestao, total,
                    (dto, respostasOpcao, totalQ) => dto.Bimestres = AgruparPorBimestre(respostasOpcao, totalQ, _bimestresReferencia)),
            adicionarTotais: (dto, respostasQuestao, total) =>
                dto.TotaisPorBimestre = AgruparPorBimestre(respostasQuestao, total, _bimestresReferencia));

    internal static List<RelatorioConsolidadoBimestreDto> AgruparPorBimestre(List<RelatorioRespostaAlunoDto> respostas, int total, IEnumerable<BimestreDominio> bimestresReferencia)
    {
        var grupos = respostas
            .GroupBy(r => r.BimestreId ?? 0)
            .ToDictionary(g => g.Key, g => g.Count());

        return [.. bimestresReferencia
            .OrderBy(b => b.Id)
            .Select(b => new RelatorioConsolidadoBimestreDto
            {
                Bimestre = b.Descricao,
                Quantidade = grupos.TryGetValue(b.Id, out int qtd) ? qtd : 0,
                Percentual = CalcularPercentual(grupos.TryGetValue(b.Id, out int q) ? q : 0, total)
            })];
    }
}
