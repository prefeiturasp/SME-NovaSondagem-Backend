using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;

public interface IObterOpcaoRespostaPorIdUseCase
{
    Task<OpcaoRespostaDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default);
}