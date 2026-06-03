using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Dominio.ValueObjects;

namespace SME.Sondagem.Dominio.Strategies.Bimestre;

/// <summary>
/// Estratégia para Modalidade 3 (EJA).
/// EJA usa apenas o 1° bimestre (Id=2) e o 2° bimestre (Id=3).
/// </summary>
public sealed class BimestreModalidadeEjaStrategy : IBimestreModalidadeStrategy
{
    private const int ModalidadeEja = (int)Modalidade.EJA;

    private static readonly HashSet<int> BimestresPermitidos = [2, 3];

    public bool Aplicavel(int modalidade) => modalidade == ModalidadeEja;

    public IEnumerable<BimestreExibicao> AplicarRegras(IEnumerable<Entidades.Bimestre> bimestresCompletos, int? bimestreFiltrado)
    {
        var lista = bimestresCompletos
            .Where(b => BimestresPermitidos.Contains(b.Id))
            .Select(b => new BimestreExibicao(b.Id, b.Descricao));

        return bimestreFiltrado.HasValue
            ? lista.Where(b => b.Id == bimestreFiltrado.Value)
            : lista;
    }
}
