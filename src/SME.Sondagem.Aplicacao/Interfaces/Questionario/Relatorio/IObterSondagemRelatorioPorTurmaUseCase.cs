using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infrastructure.Dtos.Questionario.Relatorio;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;

public interface IObterSondagemRelatorioPorTurmaUseCase
{
    public Task<QuestionarioSondagemRelatorioDto> ObterSondagemRelatorio([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken);
}
