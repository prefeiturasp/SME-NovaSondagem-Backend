namespace SME.Sondagem.Aplicacao.Interfaces.Aluno;

public interface IAlunoUseCase
{
    Task<IEnumerable<object>> ObterAlunosAsync();
}
