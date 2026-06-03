namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IAbrangenciaService
    {
        Task<bool> DeveIgnorarAbrangenciaAsync(string? perfil = null, CancellationToken cancellationToken = default);
        Task<(List<string> Dres, List<string> Ues, List<string> Turmas)> ObterAbrangenciaCompletaAsync(int anoLetivo, int modalidade, string? codigoDre, string? codigoUe = null, int semestre = 0, string? rf = null, string? perfil = null, CancellationToken cancellationToken = default);
    }
}
