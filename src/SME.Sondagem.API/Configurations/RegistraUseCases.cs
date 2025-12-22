using SME.Sondagem.API.Application.Interfaces;
using SME.Sondagem.API.Application.UseCases;
using SME.Sondagem.Application.Interfaces;
using SME.Sondagem.Application.UseCases;

namespace SME.SME.Sondagem.Api.Configurations
{
    public static class RegistraUseCases
    {
        public static void Registrar(IServiceCollection services)
        {
            services.AddScoped<ISondagemUseCase, SondagemUseCase>();
            services.AddScoped<IQuestionarioUseCase, QuestionarioUseCase>();
            services.AddScoped<ICicloUseCase, CicloUseCase>();
            services.AddScoped<IProficienciaUseCase, ProficienciaUseCase>();
            services.AddScoped<IComponenteCurricularUseCase, ComponenteCurricularUseCase>();
            services.AddScoped<IEstudantesUseCase, EstudantesUseCase>();
            services.AddScoped<IQuestaoUseCase, QuestaoUseCase>();
            services.AddScoped<IOpcaoRespostaUseCase, OpcaoRespostaUseCase>();
            services.AddScoped<IAutenticacaoUseCase, AutenticacaoUseCase>();
        }
    }
}
