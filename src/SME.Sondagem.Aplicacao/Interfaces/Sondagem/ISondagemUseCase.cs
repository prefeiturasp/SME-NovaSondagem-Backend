using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem;

public interface ISondagemUseCase
{
    Task<IEnumerable<object>> ObterTodasSondagensAsync();
    Task<IEnumerable<long>> SalvarOuAtualizarSondagemAsync(SondagemSalvarDto dto);


}
