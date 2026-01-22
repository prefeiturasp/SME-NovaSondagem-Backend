namespace SME.Sondagem.Aplicacao.Interfaces.Turma
{
    public interface IObterPermissaoTurmaUseCase
    {
        Task<bool> ObterPermissaoTurma(int turmaId, CancellationToken cancellationToken);
    }
}
