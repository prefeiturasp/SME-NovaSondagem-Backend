using SME.Sondagem.Infrastructure.Dtos.Sondagem;

namespace SME.Sondagem.Aplicacao.Interfaces.Sondagem
{
    public interface ISondagemSalvarRespostasUseCase
    {
        Task<ResultadoSalvamentoSondagemDto> SalvarOuAtualizarSondagemAsync(SondagemSalvarDto dto);
    }
}
