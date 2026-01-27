using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.Sondagem.Aplicacao.Agregadores;
using SME.Sondagem.Aplicacao.Interfaces.Autenticacao;
using SME.Sondagem.Aplicacao.Interfaces.Bimestre;
using SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;
using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Aplicacao.Interfaces.QuestionarioBimestre;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Aplicacao.Interfaces.Turma;
using SME.Sondagem.Aplicacao.Services.EOL;
using SME.Sondagem.Aplicacao.UseCases.Autenticacao;
using SME.Sondagem.Aplicacao.UseCases.Bimestre;
using SME.Sondagem.Aplicacao.UseCases.ComponenteCurricular;
using SME.Sondagem.Aplicacao.UseCases.OpcaoResposta;
using SME.Sondagem.Aplicacao.UseCases.Proficiencia;
using SME.Sondagem.Aplicacao.UseCases.Questao;
using SME.Sondagem.Aplicacao.UseCases.QuestaoOpcaoResposta;
using SME.Sondagem.Aplicacao.UseCases.Questionario;
using SME.Sondagem.Aplicacao.UseCases.QuestionarioBimestre;
using SME.Sondagem.Aplicacao.UseCases.Sondagem;
using SME.Sondagem.Aplicacao.UseCases.Turma;
using SME.Sondagem.Aplicacao.Validators.Bimestre;
using SME.Sondagem.Aplicacao.Validators.ComponenteCurricular;
using SME.Sondagem.Aplicacao.Validators.Proficiencia;
using SME.Sondagem.Aplicacao.Validators.Questao;
using SME.Sondagem.Aplicacao.Validators.QuestionarioBimestre;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Dados.Repositorio.Auditoria;
using SME.Sondagem.Dados.Repositorio.Elastic;
using SME.Sondagem.Dados.Repositorio.Postgres;
using SME.Sondagem.Dados.Services.Auditoria;
using SME.Sondagem.Infra.Contexto;
using SME.Sondagem.Infra.Dtos.Proficiencia;
using SME.Sondagem.Infra.Dtos.Questionario;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;
using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;
using SME.Sondagem.Infrastructure.Dtos.QuestionarioBimestre;
using SME.Sondagem.Infrastructure.Interfaces;
using SME.Sondagem.Infrastructure.Services;
using SME.Sondagem.IoC.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.IoC;

[ExcludeFromCodeCoverage]
public static class RegistraDependencias
{
    public static void Registrar(IServiceCollection services, IConfiguration configuration)
    {
        services.AdicionarValidadoresFluentValidation();

        services.AdicionarElasticSearch(configuration);
        RegistrarRepositorios(services);
        RegistrarServicos(services);
        RegistrarCasosDeUso(services);
        RegistrarValidadores(services);
        RegistrarContextos(services);
        RegistrarAgregadores(services);
    }

    private static void RegistrarRepositorios(IServiceCollection services)
    {
        services.TryAddScoped<IRepositorioAuditoria, RepositorioAuditoria>();
        services.TryAddScoped<IRepositorioBimestre, RepositorioBimestre>();
        services.TryAddScoped<IRepositorioProficiencia, RepositorioProficiencia>();
        services.TryAddScoped<IRepositorioQuestao, RepositorioQuestao>();
        services.TryAddScoped<IRepositorioQuestaoOpcaoResposta, RepositorioQuestaoOpcaoResposta>();
        services.TryAddScoped<IRepositorioQuestionario, RepositorioQuestionario>();
        services.TryAddScoped<IRepositorioSondagem, RepositorioSondagem>();
        services.TryAddScoped<IRepositorioQuestionarioBimestre, RepositorioQuestionarioBimestre>();

        //repositórios do Elastic
        services.AddScoped<IRepositorioElasticAluno, RepositorioElasticAluno>();
        services.AddScoped<IRepositorioElasticTurma, RepositorioElasticTurma>();
    }

    private static void RegistrarServicos(IServiceCollection services)
    {
        services.AddScoped<IAlunoPapService, AlunoPapService>();
        services.AddScoped<IControleAcessoService, ControleAcessoService>();
        services.TryAddScoped<IServicoTelemetria, ServicoTelemetria>();
        services.TryAddScoped<IServicoLog, ServicoLog>();
        services.TryAddScoped<IServicoUsuario, ServicoUsuario>();
        services.AddScoped<IServicoAuditoria, ServicoAuditoria>();
    }

    private static void RegistrarCasosDeUso(IServiceCollection services)
    {
        services.TryAddScoped<ICriarBimestreUseCase, CriarBimestreUseCase>();
        services.TryAddScoped<IAtualizarBimestreUseCase, AtualizarBimestreUseCase>();
        services.TryAddScoped<IExcluirBimestreUseCase, ExcluirBimestreUseCase>();
        services.TryAddScoped<IObterBimestresUseCase, ObterBimestresUseCase>();
        services.TryAddScoped<IObterBimestrePorIdUseCase, ObterBimestrePorIdUseCase>();
        services.TryAddScoped<ICriarOpcaoRespostaUseCase, CriarOpcaoRespostaUseCase>();
        services.TryAddScoped<IAtualizarOpcaoRespostaUseCase, AtualizarOpcaoRespostaUseCase>();
        services.TryAddScoped<IExcluirOpcaoRespostaUseCase, ExcluirOpcaoRespostaUseCase>();
        services.TryAddScoped<IObterOpcaoRespostaUseCase, ObterOpcaoRespostasUseCase>();
        services.TryAddScoped<IObterOpcaoRespostaPorIdUseCase, ObterOpcaoRespostaPorIdUseCase>();
        services.TryAddScoped<ICriarQuestaoOpcaoRespostaUseCase, CriarQuestaoOpcaoRespostaUseCase>();
        services.TryAddScoped<IAtualizarQuestaoOpcaoRespostaUseCase, AtualizarQuestaoOpcaoRespostaUseCase>();
        services.TryAddScoped<IExcluirQuestaoOpcaoRespostaUseCase, ExcluirQuestaoOpcaoRespostaUseCase>();
        services.TryAddScoped<IObterQuestaoOpcaoRespostaUseCase, ObterQuestaoOpcaoRespostasUseCase>();
        services.TryAddScoped<IObterQuestaoOpcaoRespostaPorIdUseCase, ObterQuestaoOpcaoRespostaPorIdUseCase>();
        services.TryAddScoped<ICriarQuestionarioUseCase, CriarQuestionarioUseCase>();
        services.TryAddScoped<IAtualizarQuestionarioUseCase, AtualizarQuestionarioUseCase>();
        services.TryAddScoped<IExcluirQuestionarioUseCase, ExcluirQuestionarioUseCase>();
        services.TryAddScoped<IObterQuestionariosUseCase, ObterQuestionariosUseCase>();
        services.TryAddScoped<IObterQuestionarioPorIdUseCase, ObterQuestionarioPorIdUseCase>();
        services.TryAddScoped<IObterQuestionarioSondagemUseCase, ObterQuestionarioSondagemUseCase>();
        services.TryAddScoped<ICriarProficienciaUseCase, CriarProficienciaUseCase>();
        services.TryAddScoped<IAtualizarProficienciaUseCase, AtualizarProficienciaUseCase>();
        services.TryAddScoped<IExcluirProficienciaUseCase, ExcluirProficienciaUseCase>();
        services.TryAddScoped<IObterProficienciasUseCase, ObterProficienciasUseCase>();
        services.TryAddScoped<IObterProficienciaPorIdUseCase, ObterProficienciaPorIdUseCase>();
        services.TryAddScoped<IComponenteCurricularUseCase, ComponenteCurricularUseCase>();
        services.TryAddScoped<ICriarQuestaoUseCase, CriarQuestaoUseCase>();
        services.TryAddScoped<IAtualizarQuestaoUseCase, AtualizarQuestaoUseCase>();
        services.TryAddScoped<IExcluirQuestaoUseCase, ExcluirQuestaoUseCase>();
        services.TryAddScoped<IObterQuestoesUseCase, ObterQuestoesUseCase>();
        services.TryAddScoped<IObterQuestaoPorIdUseCase, ObterQuestaoPorIdUseCase>();
        services.TryAddScoped<IAutenticacaoUseCase, AutenticacaoUseCase>();
        services.TryAddScoped<ISondagemSalvarRespostasUseCase, SondagemSalvarRespostasUseCase>();
        services.TryAddScoped<ICriarSondagemUseCase, CriarSondagemUseCase>();
        services.TryAddScoped<IAtualizarSondagemUseCase, AtualizarSondagemUseCase>();
        services.TryAddScoped<IExcluirSondagemUseCase, ExcluirSondagemUseCase>();
        services.TryAddScoped<IObterSondagensUseCase, ObterSondagemUseCase>();
        services.TryAddScoped<IObterSondagemPorIdUseCase, ObterSondagemPorIdUseCase>();
        services.TryAddScoped<IObterPermissaoTurmaUseCase, ObterPermissaoTurmaUseCase>();
        services.TryAddScoped<IObterProficienciasPorComponenteCurricularUseCase,ObterProficienciasPorComponenteCurricularUseCase>();
        services.TryAddScoped<IObterBimestresPorQuestionarioUseCase, ObterBimestresPorQuestionarioUseCase>();
        services.TryAddScoped<IObterQuestionariosBimestresUseCase, ObterQuestionariosBimestresUseCase>();
        services.TryAddScoped<IVincularBimestresUseCase, VincularBimestresUseCase>();
        services.TryAddScoped<IExcluirVinculosPorQuestionarioUseCase, ExcluirVinculosPorQuestionarioUseCase>();
    }

    private static void RegistrarValidadores(IServiceCollection services)
    {
        services.TryAddScoped<IValidator<CriarComponenteCurricularDto>, CriarComponenteCurricularValidator>();
        services.TryAddScoped<IValidator<AtualizarComponenteCurricularDto>, AtualizarComponenteCurricularValidator>();
        services.TryAddScoped<IValidator<ProficienciaDto>, CriarProficienciaValidator>();
        services.TryAddScoped<IValidator<ProficienciaDto>, AtualizarProficienciaValidator>();
        services.TryAddScoped<IValidator<BimestreDto>, CriarBimestreValidator>();
        services.TryAddScoped<IValidator<BimestreDto>, AtualizarBimestreValidator>();
        services.TryAddScoped<IValidator<QuestaoDto>, CriarQuestaoValidator>();
        services.TryAddScoped<IValidator<QuestaoDto>, AtualizarQuestaoValidator>();
        services.TryAddScoped<IValidator<VincularBimestresDto>, VincularBimestresValidator>();
        services.TryAddScoped<IValidator<AtualizarVinculosBimestresDto>, AtualizarVinculosBimestresValidator>();
    }

    private static void RegistrarContextos(IServiceCollection services)
    {
        services.TryAddScoped<IContextoAplicacao, ContextoHttp>();
        services.TryAddScoped<ContextoBase>(provider => 
            (ContextoBase)provider.GetRequiredService<IContextoAplicacao>());
    }

    private static void RegistrarAgregadores(IServiceCollection services)
    {
        services.AddScoped<RepositoriosElastic>();
        services.AddScoped<RepositoriosSondagem>();
    }
}