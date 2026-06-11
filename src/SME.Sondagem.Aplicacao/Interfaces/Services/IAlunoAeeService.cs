namespace SME.Sondagem.Aplicacao.Interfaces.Services;

public interface IAlunoAeeService
{
    Task<Dictionary<int, bool>> VerificarAlunosPossuemPlanoAeeAsync(
        IEnumerable<int> codigosAlunos,
        int codigoTurma,
        string? codigoUe,
        CancellationToken cancellationToken = default);
}
