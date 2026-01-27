using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SME.Sondagem.IoC.Extensions;
using Xunit;

namespace SME.Sondagem.IoC.Teste.Extensions;

public class RegistrarFluentValidationTeste
{
    [Fact]
    public void AdicionarValidadoresFluentValidation_DeveRegistrarValidadoresDasAssemblies()
    {
        var services = new ServiceCollection();

        // Act
        RegistrarFluentValidation.AdicionarValidadoresFluentValidation(services);

        // Assert
        var validadoresRegistrados = services
            .Where(d => d.ServiceType.IsGenericType &&
                        d.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>))
            .ToList();

        Assert.NotEmpty(validadoresRegistrados);
        Assert.All(validadoresRegistrados, v => Assert.Equal(ServiceLifetime.Scoped, v.Lifetime));
    }
}
