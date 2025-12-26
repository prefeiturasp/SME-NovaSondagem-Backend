using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.Sondagem.Aplicacao.Interfaces.Autenticacao;
using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;
using SME.Sondagem.Aplicacao.Interfaces.Aluno;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Aplicacao.UseCases.Autenticacao;
using SME.Sondagem.Aplicacao.UseCases.Ciclo;
using SME.Sondagem.Aplicacao.UseCases.ComponenteCurricular;
using SME.Sondagem.Aplicacao.UseCases.Aluno;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Infra.Contexto;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.IoC.Extensions;

namespace SME.Sondagem.IoC;

public static class RegistraDependencias
{
    public static void Registrar(IServiceCollection services)
    {
        services.AdicionarValidadoresFluentValidation();

        RegistrarRepositorios(services);
        RegistrarServicos(services);
        RegistrarCasosDeUso(services);
        RegistrarContextos(services);
        //RegistraMapeamentos.Registrar();
    }

    private static void RegistrarRepositorios(IServiceCollection services)
    {
        //services.TryAddScoped<IRepositorioCache, RepositorioCache>();
    }

    private static void RegistrarServicos(IServiceCollection services)
    {
        services.TryAddScoped<IServicoTelemetria, ServicoTelemetria>();
        services.TryAddScoped<IServicoLog, ServicoLog>();

    }

    private static void RegistrarCasosDeUso(IServiceCollection services)
    {
        services.TryAddScoped<IObterQuestionarioSondagemUseCase, ObterQuestionarioSondagemUseCase>();
        services.TryAddScoped<ISondagemUseCase, SondagemUseCase>();
        services.TryAddScoped<IQuestionarioUseCase, QuestionarioUseCase>();
        services.TryAddScoped<ICicloUseCase, CicloUseCase>();
        services.TryAddScoped<IProficienciaUseCase, ProficienciaUseCase>();
        services.TryAddScoped<IComponenteCurricularUseCase, ComponenteCurricularUseCase>();
        services.TryAddScoped<IAlunoUseCase, AlunoUseCase>();
        services.TryAddScoped<IQuestaoUseCase, QuestaoUseCase>();
        services.TryAddScoped<IOpcaoRespostaUseCase, OpcaoRespostaUseCase>();
        services.TryAddScoped<IAutenticacaoUseCase, AutenticacaoUseCase>();
    }

    private static void RegistrarContextos(IServiceCollection services)
    {
        services.TryAddScoped<IContextoAplicacao, ContextoHttp>();
    }
}