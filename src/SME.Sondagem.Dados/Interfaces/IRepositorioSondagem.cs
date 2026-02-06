namespace SME.Sondagem.Dados.Interfaces;

public interface IRepositorioSondagem : IRepositorioBase<Dominio.Entidades.Sondagem.Sondagem>
{
    Task<Dominio.Entidades.Sondagem.Sondagem> ObterSondagemAtiva(CancellationToken cancellationToken = default);
}
