using SME.Sondagem.Dominio.ValueObjects;

namespace SME.Sondagem.Dominio.Strategies.Bimestre;

/// <summary>
/// Estratégia padrão: todos os bimestres do banco, sem alteração de descrição.
/// Atua como fallback para modalidades sem regra específica.
/// </summary>
public sealed class BimestreModalidadePadraoStrategy : IBimestreModalidadeStrategy
{
    public bool Aplicavel(int modalidade) => true;

    public IEnumerable<BimestreExibicao> AplicarRegras(IEnumerable<Entidades.Bimestre> bimestresCompletos, int? bimestreFiltrado)
    {
        var lista = bimestresCompletos
            .Select(b => new BimestreExibicao(b.Id, b.Descricao));

        return bimestreFiltrado.HasValue
            ? lista.Where(b => b.Id == bimestreFiltrado.Value)
            : lista;
    }
}
