namespace SME.Sondagem.Aplicacao.Interfaces.Questionario;

public interface IOpcaoRespostaUseCase
{
    Task<IEnumerable<object>> ObterOpcoesRespostaAsync();
}
