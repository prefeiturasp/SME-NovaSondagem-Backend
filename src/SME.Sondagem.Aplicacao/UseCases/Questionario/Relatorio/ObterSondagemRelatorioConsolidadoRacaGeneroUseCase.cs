using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infrastructure.Dtos.Relatorio;
using SME.Sondagem.Dominio.Entidades;
using RacaDominio = SME.Sondagem.Dominio.Entidades.RacaCor;
using GeneroDominio = SME.Sondagem.Dominio.Entidades.GeneroSexo;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.Relatorio;

public class ObterSondagemRelatorioConsolidadoRacaGeneroUseCase : ObterSondagemRelatorioConsolidadoBase, IObterSondagemRelatorioConsolidadoRacaGeneroUseCase
{
    private IEnumerable<RacaDominio> _racasReferencia = [];
    private IEnumerable<GeneroDominio> _generosReferencia = [];

    public ObterSondagemRelatorioConsolidadoRacaGeneroUseCase(
        RepositoriosSondagem repositorioSondagem,
        IRepositorioElasticTurma repositorioElasticTurma) : base(repositorioSondagem, repositorioElasticTurma)
    {
    }

    protected override string TituloSemDados => "Relatório Consolidado por Gênero e Raça - Sem Dados";
    protected override string ObterTitulo(int anoLetivo) => $"Relatório Consolidado de Sondagem por Gênero e Raça - {anoLetivo}";

    public new async Task<RelatorioConsolidadoSondagemDto> ObterSondagemRelatorio(FiltroConsolidadoDto filtro, CancellationToken cancellationToken)
    {
        var racas = await RepositorioSondagem.RepositorioRacaCor.ListarAsync(cancellationToken) ?? [];
        var generos = await RepositorioSondagem.RepositorioGeneroSexo.ListarAsync(cancellationToken) ?? [];

        if (filtro.RacaId.HasValue)
            _racasReferencia = racas.Where(r => r.Id == filtro.RacaId.Value);
        else
            _racasReferencia = racas;

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
                    (dto, respostasOpcao, totalQ) => dto.GenerosComRacas = AgruparGeneroComRacas(respostasOpcao, totalQ, _generosReferencia, _racasReferencia)),
            adicionarTotais: (dto, respostasQuestao, total) =>
            {
                dto.TotaisPorGenero = ObterSondagemRelatorioConsolidadoGeneroUseCase.AgruparPorGenero(respostasQuestao, total, _generosReferencia);
                dto.TotaisPorRaca = ObterSondagemRelatorioConsolidadoRacaUseCase.AgruparPorRaca(respostasQuestao, total, _racasReferencia);
                dto.TotaisPorGeneroComRacas = AgruparGeneroComRacas(respostasQuestao, total, _generosReferencia, _racasReferencia);
            });

    private static List<RelatorioConsolidadoGeneroRacaDto> AgruparGeneroComRacas(List<RelatorioRespostaAlunoDto> respostas, int totalRespostasQuestao, IEnumerable<GeneroDominio> generosReferencia, IEnumerable<RacaDominio> racasReferencia)
    {
        var grupos = respostas
            .GroupBy(r => r.GeneroSexoId ?? 0)
            .ToDictionary(g => g.Key, g => g.ToList());

        var lista = generosReferencia
            .Select(g => ConstruirGeneroRaca(g.Descricao, grupos.TryGetValue(g.Id, out var resps) ? resps : [], totalRespostasQuestao, racasReferencia))
            .ToList();

        if (grupos.TryGetValue(0, out var respostasNaoInformado) && respostasNaoInformado.Count > 0)
        {
            lista.Add(ConstruirGeneroRaca("NÃO PREENCHIDO", respostasNaoInformado, totalRespostasQuestao, racasReferencia));
        }

        return [.. lista.OrderBy(g => g.Genero ?? "NÃO PREENCHIDO")];
    }

    private static RelatorioConsolidadoGeneroRacaDto ConstruirGeneroRaca(string generoDescricao, List<RelatorioRespostaAlunoDto> respostasGenero, int totalRespostasQuestao, IEnumerable<RacaDominio> racasReferencia)
    {
        int totalGenero = respostasGenero.Count;

        return new RelatorioConsolidadoGeneroRacaDto
        {
            Genero = generoDescricao,
            TotalGenero = totalGenero,
            PercentualGenero = CalcularPercentual(totalGenero, totalRespostasQuestao),
            Racas = ObterSondagemRelatorioConsolidadoRacaUseCase.AgruparPorRaca(respostasGenero, totalGenero, racasReferencia)
        };
    }
}
