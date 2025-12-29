namespace SME.Sondagem.Aplicacao.Interfaces.Proficiencia;

public interface IExcluirProficienciaUseCase
{
    Task<bool> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}