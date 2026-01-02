using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;

public interface IAtualizarOpcaoRespostaUseCase
{
    Task<OpcaoRespostaDto?> ExecutarAsync(long id, OpcaoRespostaDto opcaoRespostaDto, CancellationToken cancellationToken = default);
}