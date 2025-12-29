namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface IObterQuestionarioSondagemUseCase
{
    public Task<Infra.Dtos.Questionario.QuestionarioDto> ObterQuestionarioSondagem();
}
