namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface IQuestionarioUseCase
{
    Task<IEnumerable<object>> ObterQuestionariosAsync();
}
