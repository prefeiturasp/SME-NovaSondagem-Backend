using System.Reflection;
using SME.Sondagem.Infra.Services;
using Xunit;

namespace SME.Sondagem.Infra.Teste.Services;

public class ServicoEolConstantsTeste
{
    [Fact]
    public void TodasAsConstantesDevemSerAcessiveisENaoNulas()
    {
        // Arrange
        var type = typeof(ServicoEolConstants);

        // Act
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        // Assert
        Assert.NotEmpty(fields);
        foreach (var field in fields)
        {
            var valor = field.GetValue(null);
            Assert.False(valor is null, $"A constante {field.Name} não pode ser nula.");
            Assert.IsType<string>(valor);
            Assert.False(string.IsNullOrWhiteSpace((string)valor), $"A constante {field.Name} não pode ser vazia.");
        }
    }

    [Fact]
    public void ConstrutorProtegido_DeveSerInvocavelPorReflexao()
    {
        // Arrange
        var type = typeof(ServicoEolConstants);
        var ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                       .FirstOrDefault(c => c.GetParameters().Length == 0);

        // Act
        var instance = ctor?.Invoke(null);

        // Assert
        Assert.NotNull(ctor);
        Assert.NotNull(instance);
        Assert.IsType<ServicoEolConstants>(instance);
    }
}
