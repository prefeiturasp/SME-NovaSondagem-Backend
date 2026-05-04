namespace SME.Sondagem.Dominio.Strategies.Bimestre;

/// <summary>
/// Contrato para estratégias de filtragem e adaptação de bimestres por modalidade.
/// Cada implementação encapsula as regras de quais bimestres são válidos
/// e como devem ser exibidos para uma determinada modalidade.
/// </summary>
public interface IBimestreModalidadeStrategy
{
    /// <summary>
    /// Indica se esta estratégia é aplicável à modalidade informada.
    /// </summary>
    bool Aplicavel(int modalidade);

    /// <summary>
    /// Retorna os bimestres válidos para a modalidade, com descrições já
    /// adaptadas para exibição. Aplica o filtro por bimestreId quando informado.
    /// </summary>
    IEnumerable<ValueObjects.BimestreExibicao> AplicarRegras(IEnumerable<Entidades.Bimestre> bimestresCompletos, int? bimestreFiltrado);
}
