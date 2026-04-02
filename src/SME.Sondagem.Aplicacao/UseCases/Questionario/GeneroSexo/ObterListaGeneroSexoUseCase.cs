using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario.GeneroSexo
{
    public class ObterListaGeneroSexoUseCase : IObterListaGeneroSexoUseCase
    {
        private readonly IRepositorioGeneroSexo _repositorioGeneroSexo;

        public ObterListaGeneroSexoUseCase(IRepositorioGeneroSexo repositorioGeneroSexo)
        {
            _repositorioGeneroSexo = repositorioGeneroSexo ?? throw new ArgumentNullException(nameof(repositorioGeneroSexo));
        }

        public async Task<IEnumerable<ItemMenuDto>> Executar(CancellationToken cancellationToken = default)
        {
            var buscarNaBase = await _repositorioGeneroSexo.ListarAsync(cancellationToken);

            if(!buscarNaBase.Any())
                return Enumerable.Empty<ItemMenuDto>();

            return buscarNaBase.Select(generoSexo => new ItemMenuDto(generoSexo.Id, generoSexo.Descricao));

        }
    }
}
