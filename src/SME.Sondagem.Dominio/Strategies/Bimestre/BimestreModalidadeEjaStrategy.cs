using SME.Sondagem.Dominio.Enums;
using SME.Sondagem.Dominio.ValueObjects;

namespace SME.Sondagem.Dominio.Strategies.Bimestre;

/// <summary>
/// Estratégia para Modalidade 3 (EJA).
/// EJA usa apenas 2 bimestres por semestre: 1° semestre = Id 2/3, 2° semestre = Id 4/5
/// (renomeados na exibição para "1° bimestre"/"2° bimestre", já que são o 1°/2° bimestre do semestre atual).
/// </summary>
public sealed class BimestreModalidadeEjaStrategy : IBimestreModalidadeStrategy
{
    private const int ModalidadeEja = (int)Modalidade.EJA;

    private static readonly HashSet<int> BimestresPrimeiroSemestre = [2, 3];
    private static readonly HashSet<int> BimestresSegundoSemestre = [4, 5];

    public bool Aplicavel(int modalidade) => modalidade == ModalidadeEja;

    public IEnumerable<BimestreExibicao> AplicarRegras(IEnumerable<Entidades.Bimestre> bimestresCompletos, int? bimestreFiltrado, int? semestre = null)
    {
        var ehSegundoSemestre = semestre == 2;
        var bimestresPermitidos = ehSegundoSemestre ? BimestresSegundoSemestre : BimestresPrimeiroSemestre;

        var lista = bimestresCompletos
            .Where(b => bimestresPermitidos.Contains(b.Id))
            .Select(b => new BimestreExibicao(b.Id, ehSegundoSemestre ? RenomearSegundoSemestre(b.Id, b.Descricao) : b.Descricao));

        return bimestreFiltrado.HasValue
            ? lista.Where(b => b.Id == bimestreFiltrado.Value)
            : lista;
    }

    /// <summary>
    /// Rótulo de exibição pro 2° semestre EJA — reaproveita os ids 4/5 (3°/4° bimestre do
    /// calendário padrão) como "1°"/"2° bimestre" do semestre atual. Fonte única dessa regra:
    /// qualquer outro lugar que precise desse rótulo (ex: dropdown de `GET api/Bimestre`) deve
    /// chamar este método em vez de reimplementar o switch.
    /// </summary>
    public static string RenomearSegundoSemestre(int id, string descricaoOriginal) => id switch
    {
        4 => "1° bimestre",
        5 => "2° bimestre",
        _ => descricaoOriginal
    };
}
