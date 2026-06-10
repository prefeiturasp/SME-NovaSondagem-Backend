namespace SME.Sondagem.Aplicacao.Interfaces.Services
{
    public record AbrangenciaFiltroQuery(
        int AnoLetivo,
        int Modalidade,
        string? CodigoDre,
        string? CodigoUe = null,
        int Semestre = 0,
        string? Rf = null,
        string? Perfil = null);
}
