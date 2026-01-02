using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;

public interface IObterOpcaoRespostaUseCase
{
    Task<IEnumerable<OpcaoRespostaDto>> ExecutarAsync(CancellationToken cancellationToken = default);
}