using Xunit;

namespace SME.Sondagem.Dominio.Teste.Extensoes;

public class NumberExtensionTeste
{
    [Fact]
    public void ToArray_DeveConverterLongEmArray()
    {
        long numero = 42;

        var resultado = numero.ToArray<long>();

        Assert.NotNull(resultado);
        Assert.Single(resultado);
        Assert.Equal(numero, resultado[0]);
    }

    [Fact]
    public void ToArray_DeveConverterZeroEmArray()
    {
        long numero = 0;

        var resultado = numero.ToArray<long>();

        Assert.NotNull(resultado);
        Assert.Single(resultado);
        Assert.Equal(0, resultado[0]);
    }

    [Fact]
    public void ToArray_DeveConverterNumeroNegativoEmArray()
    {
        long numero = -100;

        var resultado = numero.ToArray<long>();

        Assert.NotNull(resultado);
        Assert.Single(resultado);
        Assert.Equal(-100, resultado[0]);
    }

    [Fact]
    public void ToArray_DeveConverterValorMaximoEmArray()
    {
        long numero = long.MaxValue;

        var resultado = numero.ToArray<long>();

        Assert.NotNull(resultado);
        Assert.Single(resultado);
        Assert.Equal(long.MaxValue, resultado[0]);
    }

    [Fact]
    public void ToArray_DeveConverterValorMinimoEmArray()
    {
        long numero = long.MinValue;

        var resultado = numero.ToArray<long>();

        Assert.NotNull(resultado);
        Assert.Single(resultado);
        Assert.Equal(long.MinValue, resultado[0]);
    }
}