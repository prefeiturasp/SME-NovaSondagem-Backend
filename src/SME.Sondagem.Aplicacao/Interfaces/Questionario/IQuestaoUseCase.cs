namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface IQuestaoUseCase
{
    Task<IEnumerable<object>> ObterQuestoesAsync();
}
