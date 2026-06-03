namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public interface IAbrangenciaService
    {
        Task<bool> DeveIgnorarAbrangenciaAsync(string? perfil = null, CancellationToken cancellationToken = default);
        Task<(List<string> Dres, List<string> Ues, List<string> Turmas)> ObterAbrangenciaCompletaAsync(AbrangenciaFiltroQuery filtro, CancellationToken cancellationToken = default);
    }
}
