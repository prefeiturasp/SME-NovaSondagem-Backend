using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Repositorio.Postgres;

namespace SME.Sondagem.API.Configuracoes;

public static class RegistraRepositorios
{
    public static void Registrar(IServiceCollection services)
    {
        services.AddScoped<ISondagemRepository, SondagemRepository>();
        services.AddScoped<IQuestionarioRepository, QuestionarioRepository>();
        services.AddScoped<ICicloRepository, CicloRepository>();
        services.AddScoped<IProficienciaRepository, ProficienciaRepository>();
        services.AddScoped<IComponenteCurricularRepository, ComponenteCurricularRepository>();
        services.AddScoped<IAlunoRepository, AlunoRepository>();
        services.AddScoped<IQuestaoRepository, QuestaoRepository>();
        services.AddScoped<IOpcaoRespostaRepository, OpcaoRespostaRepository>();
    }
}
