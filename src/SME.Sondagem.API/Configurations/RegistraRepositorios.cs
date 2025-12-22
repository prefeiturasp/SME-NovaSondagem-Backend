using SME.Sondagem.Infra.Repositories;
using SME.Sondagem.Infra.Repositories.Postgres;

namespace SME.SME.Sondagem.Api.Configurations
{
    public static class RegistraRepositorios
    {
        public static void Registrar(IServiceCollection services)
        {
            services.AddScoped<ISondagemRepository, SondagemRepository>();
            services.AddScoped<IQuestionarioRepository, QuestionarioRepository>();
            services.AddScoped<ICicloRepository, CicloRepository>();
            services.AddScoped<IProficienciaRepository, ProficienciaRepository>();
            services.AddScoped<IComponenteCurricularRepository, ComponenteCurricularRepository>();
            services.AddScoped<IEstudantesRepository, EstudantesRepository>();
            services.AddScoped<IQuestaoRepository, QuestaoRepository>();
            services.AddScoped<IOpcaoRespostaRepository, OpcaoRespostaRepository>();
        }
    }
}
