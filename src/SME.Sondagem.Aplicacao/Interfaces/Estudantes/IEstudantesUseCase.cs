namespace SME.Sondagem.Aplicacao.Interfaces.Estudantes;

public interface IEstudantesUseCase
{
    Task<IEnumerable<object>> ObterEstudantesAsync();
}
