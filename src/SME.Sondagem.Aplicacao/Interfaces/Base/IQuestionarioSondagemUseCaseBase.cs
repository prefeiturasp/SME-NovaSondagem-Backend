using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Base;

public interface IQuestionarioSondagemUseCaseBase
{
    Task<object> ExecutarProcessamentoQuestionario(FiltroQuestionario filtro, bool ehRelatorio, CancellationToken cancellationToken);
}
