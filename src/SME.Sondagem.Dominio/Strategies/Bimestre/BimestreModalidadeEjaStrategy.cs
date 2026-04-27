using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Dominio.ValueObjects;

namespace SME.Sondagem.Dominio.Strategies.Bimestre;

/// <summary>
/// Estratégia para Modalidade 3 (EJA).
/// Regras:
///  - Apenas o 1° bimestre (Id=2) e o 4° bimestre (Id=5) são válidos.
///  - O 4° bimestre (Id=5) deve ser exibido como "2° bimestre".
/// </summary>
public sealed class BimestreModalidadeEjaStrategy : IBimestreModalidadeStrategy
{
    private const int ModalidadeEja = (int)Modalidade.EJA;

    /// <summary>
    /// Ids dos bimestres do banco que são válidos para a modalidade EJA.
    /// Id=2 → 1° bimestre | Id=5 → 4° bimestre (renomeado para "2° bimestre").
    /// </summary>
    private static readonly HashSet<int> BimestresPermitidos = [2, 5];

    private const int IdBimestreRenomeado = 5;
    private const string DescricaoBimestreRenomeado = "2° bimestre";

    public bool Aplicavel(int modalidade) => modalidade == ModalidadeEja;

    public IEnumerable<BimestreExibicao> AplicarRegras(IEnumerable<Entidades.Bimestre> bimestresCompletos, int? bimestreFiltrado)
    {
        var lista = bimestresCompletos
            .Where(b => BimestresPermitidos.Contains(b.Id))
            .Select(b => new BimestreExibicao(
                b.Id,
                b.Id == IdBimestreRenomeado ? DescricaoBimestreRenomeado : b.Descricao));

        return bimestreFiltrado.HasValue
            ? lista.Where(b => b.Id == bimestreFiltrado.Value)
            : lista;
    }
}
