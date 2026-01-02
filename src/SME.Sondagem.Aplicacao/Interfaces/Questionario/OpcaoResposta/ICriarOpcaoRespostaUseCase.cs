using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;

public interface ICriarOpcaoRespostaUseCase
{
    Task<long> ExecutarAsync(OpcaoRespostaDto proficienciaDto, CancellationToken cancellationToken = default);
}