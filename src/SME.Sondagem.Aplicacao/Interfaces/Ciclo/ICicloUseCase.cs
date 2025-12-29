namespace SME.Sondagem.Aplicacao.Interfaces.Ciclo;

public interface ICicloUseCase
{
    Task<IEnumerable<object>> ObterCiclosAsync();
}
