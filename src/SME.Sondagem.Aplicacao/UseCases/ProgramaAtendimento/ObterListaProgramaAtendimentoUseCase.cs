using SME.Sondagem.Aplicacao.Interfaces.ProgramaAtendimento;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Infrastructure.Dtos;

namespace SME.Sondagem.Aplicacao.UseCases.ProgramaAtendimento
{
    public class ObterListaProgramaAtendimentoUseCase : IObterListaProgramaAtendimentoUseCase
    {
        private readonly IRepositorioProgramaAtendimento _repositorioProgramaAtendimento;

        public ObterListaProgramaAtendimentoUseCase(IRepositorioProgramaAtendimento repositorioProgramaAtendimento)
        {
            _repositorioProgramaAtendimento = repositorioProgramaAtendimento ?? throw new ArgumentNullException(nameof(repositorioProgramaAtendimento));
        }

        public async Task<IEnumerable<ItemMenuDto>> Executar(CancellationToken cancellationToken = default)
        {
            var buscarListaProgramaAtendimento = (await _repositorioProgramaAtendimento.ListarAsync(cancellationToken)).Where(x => !x.Excluido);

            if (!buscarListaProgramaAtendimento.Any())
                return [];

            return buscarListaProgramaAtendimento.Select(x => new ItemMenuDto(x.Id, x.Descricao));
        }
    }
}
