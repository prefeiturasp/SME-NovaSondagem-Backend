using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Dados.Interfaces;

namespace SME.Sondagem.Aplicacao.UseCases.Questionario;

public class OpcaoRespostaUseCase : IOpcaoRespostaUseCase
{
    private readonly IRepositorioOpcaoResposta opcaoRespostaRepository;

    public OpcaoRespostaUseCase(IRepositorioOpcaoResposta opcaoRespostaRepository)
    {
        this.opcaoRespostaRepository = opcaoRespostaRepository;
    }

    public async Task<IEnumerable<object>> ObterOpcoesRespostaAsync()
    {
        return await opcaoRespostaRepository.ObterTodosAsync();
    }
}
