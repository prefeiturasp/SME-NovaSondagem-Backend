using SME.Sondagem.Aplicacao.Interfaces.GeneroSexo;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.UseCases.GeneroSexo
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
            var buscarNaBase =( await _repositorioGeneroSexo.ListarAsync(cancellationToken)).Where(x => !x.Excluido);

            if (!buscarNaBase.Any())
                return [];

            return buscarNaBase.Select(generoSexo => new ItemMenuDto(generoSexo.Id, generoSexo.Descricao));

        }
    }
}
