using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.Sondagem.Application.Interfaces.Questionario;
using SME.Sondagem.Application.UseCases.Questionario;
using SME.Sondagem.Infra.Contexto;
using SME.Sondagem.Infra.Interfaces;
using SME.Sondagem.Infra.Services;
using SME.Sondagem.IoC.Extensions;

namespace SME.Sondagem.IoC
{
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
        }

        private static void RegistrarContextos(IServiceCollection services)
        {
            services.TryAddScoped<IContextoAplicacao, ContextoHttp>();
        }
    }
}