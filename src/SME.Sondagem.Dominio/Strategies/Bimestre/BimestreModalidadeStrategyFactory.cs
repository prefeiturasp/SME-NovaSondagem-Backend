using SME.Sondagem.Dominio.ValueObjects;

namespace SME.Sondagem.Dominio.Strategies.Bimestre;

/// <summary>
/// Seleciona a estratégia de bimestre adequada para a modalidade informada.
/// As estratégias são avaliadas em ordem de especificidade; a estratégia padrão (fallback)
/// é sempre a última da lista e aceita qualquer modalidade.
/// </summary>
public static class BimestreModalidadeStrategyFactory
{
    // Ordem importa: estratégias específicas primeiro, fallback por último.
    private static readonly IReadOnlyList<IBimestreModalidadeStrategy> Strategies =
    [
        new BimestreModalidadeEjaStrategy(),
        new BimestreModalidadePadraoStrategy(),
    ];

    /// <summary>
    /// Retorna a estratégia aplicável à modalidade informada.
    /// </summary>
    public static IBimestreModalidadeStrategy ObterPara(int modalidade) => Strategies.First(s => s.Aplicavel(modalidade));

    /// <summary>
    /// Atalho: aplica diretamente a estratégia e devolve os bimestres adaptados.
    /// </summary>
    public static IEnumerable<BimestreExibicao> AplicarRegras(int modalidade, IEnumerable<Entidades.Bimestre> bimestresCompletos, int? bimestreFiltrado = null)
        => ObterPara(modalidade).AplicarRegras(bimestresCompletos, bimestreFiltrado);
}
