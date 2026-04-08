using SME.Sondagem.Infrastructure.Dtos.Integracao;

namespace SME.Sondagem.Aplicacao.Interfaces.Integracao;

public interface IAtualizarContextosRespostasAlunoUseCase
{
    Task<ResumoAtualizacaoContextoDto> ExecutarAsync(CancellationToken cancellationToken = default);
}
