using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Repositorio.Postgres;

namespace SME.Sondagem.API.Configuracoes;

public static class RegistraRepositorios
{
    public static void Registrar(IServiceCollection services)
    {
        services.AddScoped<IRepositorioSondagem, RepositorioSondagem>();
        services.AddScoped<IRepositorioQuestionario, RepositorioQuestionario>();
        services.AddScoped<IRepositorioBimestre, RepositorioBimestre>();
        services.AddScoped<IRepositorioProficiencia, RepositorioProficiencia>();
        services.AddScoped<IRepositorioComponenteCurricular, RepositorioComponenteCurricular>();
        services.AddScoped<IRepositorioAluno, RepositorioAluno>();
        services.AddScoped<IRepositorioQuestao, RepositorioQuestao>();
        services.AddScoped<IRepositorioOpcaoResposta, RepositorioOpcaoResposta>();
    }
}
