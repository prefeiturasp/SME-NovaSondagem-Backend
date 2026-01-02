using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace SME.Sondagem.IoC.Extensions
{
    public static class RegistrarFluentValidation
    {
        public static void AdicionarValidadoresFluentValidation(this IServiceCollection services)
        {
            var assemblyInfra = AppDomain.CurrentDomain.Load("SME.Sondagem.Infra");

            AssemblyScanner
                .FindValidatorsInAssembly(assemblyInfra)
                .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));

            var assembly = AppDomain.CurrentDomain.Load("SME.Sondagem.Aplicacao");

            AssemblyScanner
                .FindValidatorsInAssembly(assembly)
                .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
        }
    }
}