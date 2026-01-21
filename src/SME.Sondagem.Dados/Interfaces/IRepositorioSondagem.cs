namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioSondagem
{
    Task<IEnumerable<Dominio.Entidades.Sondagem.Sondagem>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<Dominio.Entidades.Sondagem.Sondagem?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task InserirAsync(Dominio.Entidades.Sondagem.Sondagem entidade, CancellationToken cancellationToken = default);
    Task<Dominio.Entidades.Sondagem.Sondagem> ObterSondagemAtiva(CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(long id, CancellationToken cancellationToken);
    Task<bool> AtualizarAsync(Dominio.Entidades.Sondagem.Sondagem sondagem, CancellationToken cancellationToken = default);
}
