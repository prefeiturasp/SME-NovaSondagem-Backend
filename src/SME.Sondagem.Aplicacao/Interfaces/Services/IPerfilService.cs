using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IPerfilService
    {
        Task<PerfilInfoSondagemDto> ObterPerfilPorIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
