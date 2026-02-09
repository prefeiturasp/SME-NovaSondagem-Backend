namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IAlunoPapService
    {
        Task<Dictionary<int, bool>> VerificarAlunosPossuemProgramaPapAsync(IEnumerable<int> codigosAlunos, int anoLetivo, CancellationToken cancellationToken = default);
    }
}