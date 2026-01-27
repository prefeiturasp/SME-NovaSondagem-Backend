using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.Sondagem.Aplicacao.Interfaces.Autenticacao;
using SME.Sondagem.Aplicacao.Interfaces.Bimestre;
using SME.Sondagem.Aplicacao.Interfaces.ComponenteCurricular;
using SME.Sondagem.Aplicacao.Interfaces.OpcaoResposta;
using SME.Sondagem.Aplicacao.Interfaces.Proficiencia;
using SME.Sondagem.Aplicacao.Interfaces.QuestaoOpcaoResposta;
using SME.Sondagem.Aplicacao.Interfaces.Questionario;
using SME.Sondagem.Aplicacao.Interfaces.Questionario.Questao;
using SME.Sondagem.Aplicacao.Interfaces.Services;
using SME.Sondagem.Aplicacao.Interfaces.Sondagem;
using SME.Sondagem.Aplicacao.Interfaces.Turma;
using SME.Sondagem.Dados.Interfaces;
using SME.Sondagem.Dados.Interfaces.Auditoria;
using SME.Sondagem.Dados.Interfaces.Elastic;
using SME.Sondagem.Infra.Contexto;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infrastructure.Dtos.Bimestre;
using SME.Sondagem.Infrastructure.Dtos.ComponenteCurricular;
using SME.Sondagem.Infrastructure.Interfaces;
using Xunit;

namespace SME.Sondagem.IoC.Teste;

public class RegistraDependenciasTeste
{
    [Fact]
    public void Registrar_DeveRegistrarTodosOsRepositorios()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioAuditoria)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioBimestre)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioProficiencia)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioQuestao)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioQuestaoOpcaoResposta)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioQuestionario)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioSondagem)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioElasticAluno)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioElasticTurma)));
    }

    [Fact]
    public void Registrar_DeveRegistrarServicosBasicos()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IAlunoPapService)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IControleAcessoService)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IServicoTelemetria)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IServicoLog)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IServicoUsuario)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IServicoAuditoria)));
    }

    [Fact]
    public void Registrar_DeveRegistrarUseCasesDeBimestre()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(ICriarBimestreUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IAtualizarBimestreUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IExcluirBimestreUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterBimestresUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterBimestrePorIdUseCase)));
    }

    [Fact]
    public void Registrar_DeveRegistrarUseCasesDeOpcaoResposta()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(ICriarOpcaoRespostaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IAtualizarOpcaoRespostaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IExcluirOpcaoRespostaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterOpcaoRespostaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterOpcaoRespostaPorIdUseCase)));
    }

    [Fact]
    public void Registrar_DeveRegistrarUseCasesDeProficiencia()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(ICriarProficienciaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IAtualizarProficienciaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IExcluirProficienciaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterProficienciasUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterProficienciaPorIdUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterProficienciasPorComponenteCurricularUseCase)));
    }

    [Fact]
    public void Registrar_DeveRegistrarUseCasesDeQuestao()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(ICriarQuestaoUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IAtualizarQuestaoUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IExcluirQuestaoUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterQuestoesUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterQuestaoPorIdUseCase)));
    }

    [Fact]
    public void Registrar_DeveRegistrarUseCasesDeQuestaoOpcaoResposta()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(ICriarQuestaoOpcaoRespostaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IAtualizarQuestaoOpcaoRespostaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IExcluirQuestaoOpcaoRespostaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterQuestaoOpcaoRespostaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterQuestaoOpcaoRespostaPorIdUseCase)));
    }

    [Fact]
    public void Registrar_DeveRegistrarUseCasesDeQuestionario()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(ICriarQuestionarioUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IAtualizarQuestionarioUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IExcluirQuestionarioUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterQuestionariosUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterQuestionarioPorIdUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterQuestionarioSondagemUseCase)));
    }

    [Fact]
    public void Registrar_DeveRegistrarUseCasesDeSondagem()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(ISondagemSalvarRespostasUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(ICriarSondagemUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IAtualizarSondagemUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IExcluirSondagemUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterSondagensUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterSondagemPorIdUseCase)));
    }

    [Fact]
    public void Registrar_DeveRegistrarUseCasesDeAutenticacaoETurma()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IAutenticacaoUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IObterPermissaoTurmaUseCase)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IComponenteCurricularUseCase)));
    }

    [Fact]
    public void Registrar_DeveRegistrarValidadores()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IValidator<CriarComponenteCurricularDto>)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IValidator<AtualizarComponenteCurricularDto>)));
        Assert.NotNull(services.FirstOrDefault(d => d.ServiceType == typeof(IValidator<BimestreDto>)));
    }

    [Fact]
    public void Registrar_DeveRegistrarContextos()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        var descriptorContextoAplicacao = services.FirstOrDefault(d => d.ServiceType == typeof(IContextoAplicacao));
        Assert.NotNull(descriptorContextoAplicacao);

        var descriptorContextoBase = services.FirstOrDefault(d => d.ServiceType == typeof(ContextoBase));
        Assert.NotNull(descriptorContextoBase);
    }

    [Fact]
    public void Registrar_DeveRegistrarServicosComoScoped()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        var servicosScoped = services.Where(d => 
            d.ServiceType == typeof(IAlunoPapService) ||
            d.ServiceType == typeof(IServicoUsuario) ||
            d.ServiceType == typeof(IServicoAuditoria));

        Assert.All(servicosScoped, descriptor => 
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime));
    }

    [Fact]
    public void Registrar_DeveRegistrarRepositoriosComoScoped()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        var repositoriosScoped = services.Where(d => 
            d.ServiceType == typeof(IRepositorioBimestre) ||
            d.ServiceType == typeof(IRepositorioQuestao) ||
            d.ServiceType == typeof(IRepositorioSondagem));

        Assert.All(repositoriosScoped, descriptor => 
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime));
    }

    [Fact]
    public void Registrar_DeveRegistrarUseCasesComoScoped()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        var useCasesScoped = services.Where(d => 
            d.ServiceType == typeof(ICriarBimestreUseCase) ||
            d.ServiceType == typeof(ICriarProficienciaUseCase) ||
            d.ServiceType == typeof(IAutenticacaoUseCase));

        Assert.All(useCasesScoped, descriptor => 
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime));
    }

    [Fact]
    public void Registrar_DeveConfigurarFluentValidation()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        var validadores = services.Where(d => 
            d.ServiceType.IsGenericType && 
            d.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>));

        Assert.NotEmpty(validadores);
    }

    [Fact]
    public void Registrar_DeveRegistrarRepositoriosElastic()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        RegistraDependencias.Registrar(services, configuration);

        var descriptorAluno = services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioElasticAluno));
        Assert.NotNull(descriptorAluno);
        Assert.Equal(ServiceLifetime.Scoped, descriptorAluno.Lifetime);

        var descriptorTurma = services.FirstOrDefault(d => d.ServiceType == typeof(IRepositorioElasticTurma));
        Assert.NotNull(descriptorTurma);
        Assert.Equal(ServiceLifetime.Scoped, descriptorTurma.Lifetime);
    }

    private static IConfiguration CriarConfigurationMock()
    {
        var configurationData = new Dictionary<string, string>
        {
            { "ElasticSearch:Url", "http://localhost:9200" },
            { "ElasticSearch:Usuario", "elastic" },
            { "ElasticSearch:Senha", "elastic123" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData!)
            .Build();
    }
}
