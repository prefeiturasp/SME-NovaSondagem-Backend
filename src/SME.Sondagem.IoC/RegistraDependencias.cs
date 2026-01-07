using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.Sondagem.Aplicacao.Interfaces.Aluno;
using SME.Sondagem.Aplicacao.Interfaces.Autenticacao;
using SME.Sondagem.Aplicacao.Interfaces.Ciclo;
using SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;
using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Aplicacao.UseCases.Aluno;
using SME.Sondagem.Aplicacao.UseCases.Autenticacao;
using SME.Sondagem.Aplicacao.UseCases.Ciclo;
using SME.Sondagem.Aplicacao.UseCases.ComponenteCurricular;
using SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;
using SME.Sondagem.Aplicacao.UseCases.Proficiencia;
using SME.Sondagem.Aplicacao.UseCases.Questao;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Aplicacao.Validators.Ciclo;
using SME.Sondagem.Aplicacao.Validators.ComponenteCurricular;
using SME.Sondagem.Aplicacao.Validators.Proficiencia;
using SME.Sondagem.Aplicacao.Validators.Questao;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Infra.Contexto;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos.Ciclo;
using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;
using SME.Sondagem.Infrastructure.Interfaces;
using SME.Sondagem.Infrastructure.Services;
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
        RegistrarValidadores(services);
        RegistrarContextos(services);
    }

    private static void RegistrarRepositorios(IServiceCollection services)
    {
        services.TryAddScoped<IRepositorioCiclo, RepositorioCiclo>();
        services.TryAddScoped<IRepositorioProficiencia, RepositorioProficiencia>();
        services.TryAddScoped<IRepositorioQuestao, RepositorioQuestao>();
    }

    private static void RegistrarServicos(IServiceCollection services)
    {
        services.TryAddScoped<IServicoTelemetria, ServicoTelemetria>();
        services.TryAddScoped<IServicoLog, ServicoLog>();
        services.TryAddScoped<IServicoUsuario, ServicoUsuario>();
    }

    private static void RegistrarCasosDeUso(IServiceCollection services)
    {
        services.TryAddScoped<ICriarCicloUseCase, CriarCicloUseCase>();
        services.TryAddScoped<IAtualizarCicloUseCase, AtualizarCicloUseCase>();
        services.TryAddScoped<IExcluirCicloUseCase, ExcluirCicloUseCase>();
        services.TryAddScoped<IObterCiclosUseCase, ObterCiclosUseCase>();
        services.TryAddScoped<IObterCicloPorIdUseCase, ObterCicloPorIdUseCase>();
        services.TryAddScoped<ICriarOpcaoRespostaUseCase, CriarOpcaoRespostaUseCase>();
        services.TryAddScoped<IAtualizarOpcaoRespostaUseCase, AtualizarOpcaoRespostaUseCase>();
        services.TryAddScoped<IExcluirOpcaoRespostaUseCase, ExcluirOpcaoRespostaUseCase>();
        services.TryAddScoped<IObterOpcaoRespostaUseCase, ObterOpcaoRespostasUseCase>();
        services.TryAddScoped<IObterOpcaoRespostaPorIdUseCase, ObterOpcaoRespostaPorIdUseCase>();
        services.TryAddScoped<IObterQuestionarioSondagemUseCase, ObterQuestionarioSondagemUseCase>();
        services.TryAddScoped<ISondagemUseCase, SondagemUseCase>();
        services.TryAddScoped<IQuestionarioUseCase, QuestionarioUseCase>();
        services.TryAddScoped<ICriarProficienciaUseCase, CriarProficienciaUseCase>();
        services.TryAddScoped<IAtualizarProficienciaUseCase, AtualizarProficienciaUseCase>();
        services.TryAddScoped<IExcluirProficienciaUseCase, ExcluirProficienciaUseCase>();
        services.TryAddScoped<IObterProficienciasUseCase, ObterProficienciasUseCase>();
        services.TryAddScoped<IObterProficienciaPorIdUseCase, ObterProficienciaPorIdUseCase>();
        services.TryAddScoped<IComponenteCurricularUseCase, ComponenteCurricularUseCase>();
        services.TryAddScoped<IAlunoUseCase, AlunoUseCase>();
        services.TryAddScoped<ICriarQuestaoUseCase, CriarQuestaoUseCase>();
        services.TryAddScoped<IAtualizarQuestaoUseCase, AtualizarQuestaoUseCase>();
        services.TryAddScoped<IExcluirQuestaoUseCase, ExcluirQuestaoUseCase>();
        services.TryAddScoped<IObterQuestoesUseCase, ObterQuestoesUseCase>();
        services.TryAddScoped<IObterQuestaoPorIdUseCase, ObterQuestaoPorIdUseCase>();
        services.TryAddScoped<IOpcaoRespostaUseCase, OpcaoRespostaUseCase>();
        services.TryAddScoped<IAutenticacaoUseCase, AutenticacaoUseCase>();
    }

    private static void RegistrarValidadores(IServiceCollection services)
    {
        services.TryAddScoped<IValidator<CriarComponenteCurricularDto>, CriarComponenteCurricularValidator>();
        services.TryAddScoped<IValidator<AtualizarComponenteCurricularDto>, AtualizarComponenteCurricularValidator>();
        services.TryAddScoped<IValidator<ProficienciaDto>, CriarProficienciaValidator>();
        services.TryAddScoped<IValidator<ProficienciaDto>, AtualizarProficienciaValidator>();
        services.TryAddScoped<IValidator<CicloDto>, CriarCicloValidator>();
        services.TryAddScoped<IValidator<CicloDto>, AtualizarCicloValidator>();
        services.TryAddScoped<IValidator<QuestaoDto>, CriarQuestaoValidator>();
        services.TryAddScoped<IValidator<QuestaoDto>, AtualizarQuestaoValidator>();
    }

    private static void RegistrarContextos(IServiceCollection services)
    {
        services.TryAddScoped<IContextoAplicacao, ContextoHttp>();
    }
}