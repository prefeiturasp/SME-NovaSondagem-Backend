using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using GeneroDominio = SME.Sondagem.Dominio.Entidades.GeneroSexo;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoGeneroUseCase : ObterSondagemRelatorioConsolidadoBase, IObterSondagemRelatorioConsolidadoGeneroUseCase
{
    private IEnumerable<GeneroDominio> _generosReferencia = [];

    public ObterSondagemRelatorioConsolidadoGeneroUseCase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma) : base(repositorioSondagem, repositorioElasticTurma)
    {
    }

    protected override string TituloSemDados => "Relatório Consolidado por Gênero - Sem Dados";
    protected override string ObterTitulo(int anoLetivo) => $"Relatório Consolidado de Sondagem por Gênero - {anoLetivo}";

    public new async Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio(FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var generos = await RepositorioSondagem.RepositorioGeneroSexo.ListarAsync(cancellationToken);

        if (filtro.GeneroId.HasValue)
            _generosReferencia = generos.Where(g => g.Id == filtro.GeneroId.Value);
        else
            _generosReferencia = generos;

        return await base.ObterSondagemRelatorio(filtro, cancellationToken);
    }

    protected override RelatorioConsolidadoQuestaoDto ProcessarQuestao(int questaoId, string questaoNome, List<RelatorioRespostaAlunoDto> respostas)
        => ConstruirQuestaoDto(
            questaoId,
            questaoNome,
            respostas,
            processarOpcao: (opcao, respostasQuestao, total) =>
                ConstruirRespostaDto(opcao, respostasQuestao, total,
                    (dto, respostasOpcao, totalQ) => dto.Generos = AgruparPorGenero(respostasOpcao, totalQ, _generosReferencia)),
            adicionarTotais: (dto, respostasQuestao, total) =>
                dto.TotaisPorGenero = AgruparPorGenero(respostasQuestao, total, _generosReferencia));

    internal static List<RelatorioConsolidadoGeneroDto> AgruparPorGenero(List<RelatorioRespostaAlunoDto> respostas, int total, IEnumerable<GeneroDominio> generosReferencia)
    {
        var grupos = respostas
            .GroupBy(r => r.GeneroSexoId ?? 0)
            .ToDictionary(g => g.Key, g => g.Count());

        var lista = generosReferencia
            .Select(g => new RelatorioConsolidadoGeneroDto
            {
                Genero = g.Descricao,
                Sigla = g.Sigla,
                Quantidade = grupos.TryGetValue(g.Id, out int qtd) ? qtd : 0,
                Percentual = CalcularPercentual(grupos.TryGetValue(g.Id, out int q) ? q : 0, total)
            }).ToList();

        if (grupos.TryGetValue(0, out int qtdNaoInformado) && qtdNaoInformado > 0)
        {
            lista.Add(new RelatorioConsolidadoGeneroDto
            {
                Genero = "Não informado",
                Sigla = "NI",
                Quantidade = qtdNaoInformado,
                Percentual = CalcularPercentual(qtdNaoInformado, total)
            });
        }

        return [.. lista.OrderBy(g => g.Genero ?? "Não informado")];
    }
}
