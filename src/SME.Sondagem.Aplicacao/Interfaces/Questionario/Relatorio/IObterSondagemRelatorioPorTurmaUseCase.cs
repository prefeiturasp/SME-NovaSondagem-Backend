using Microsoft.AspNetCore.Mvc;
using SME.Sondagem.Infra.Dtos.Questionario;

namespace SME.Sondagem.Aplicacao.Interfaces.Questionario.Relatorio;

public interface IObterSondagemRelatorioPorTurmaUseCase
{
    public Task<Infra.Dtos.Questionario.QuestionarioSondagemRelatorioDto> ObterSondagemRelatorio([FromQuery] FiltroQuestionario filtro, CancellationToken cancellationToken);
}
