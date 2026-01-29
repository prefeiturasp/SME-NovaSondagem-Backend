using System.Reflection;
using SME.Sondagem.Infra.Policies;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Policies;

public class PoliticaPollyTeste
{
    [Fact]
    public void PublicaFila_DeveRetornarValorEsperado()
    {
        // Arrange & Act
        var valor = PoliticaPolly.PublicaFila;

        // Assert
        Assert.Equal("RetryPolicyFilasRabbit", valor);
    }

    [Fact]
    public void ConstrutorProtegido_DeveSerInvocavelPorReflexao()
    {
        // Arrange
        var type = typeof(PoliticaPolly);
        var ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                       .FirstOrDefault(c => c.GetParameters().Length == 0);

        // Act
        var instance = ctor?.Invoke(null);

        // Assert
        Assert.NotNull(ctor);
        Assert.NotNull(instance);
        Assert.IsType<PoliticaPolly>(instance);
    }
}
