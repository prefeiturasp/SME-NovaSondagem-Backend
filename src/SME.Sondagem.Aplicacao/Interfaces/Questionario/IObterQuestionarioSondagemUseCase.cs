namespace SME.Sondagem.Application.Interfaces.Questionario;

public interface IObterQuestionarioSondagemUseCase
{
    public Task<Infra.Dtos.Questionario.QuestionarioDto> ObterQuestionarioSondagem();
}
