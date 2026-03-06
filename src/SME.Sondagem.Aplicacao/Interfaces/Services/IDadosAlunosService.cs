using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IDadosAlunosService
    {
        public Task<IEnumerable<AlunoEolDto>> ObterDadosAlunosPorCodigoUe(string codigoUe,CancellationToken cancellationToken = default);
    }
}