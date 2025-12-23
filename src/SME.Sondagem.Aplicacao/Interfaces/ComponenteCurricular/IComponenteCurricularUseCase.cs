namespace SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;

public interface IComponenteCurricularUseCase
{
    Task<IEnumerable<object>> ObterComponentesAsync();
}
