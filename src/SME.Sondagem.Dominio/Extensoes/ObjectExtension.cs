using System.Globalization;
using System.Text;

namespace SME.Sondagem.Dominio;

public static class ObjectExtension
{
    public static void LancarExcecaoNegocioSeEhNulo(this object objeto, string msgErro)
    {
        if (objeto == null)
            throw new NegocioException(msgErro);
    }

    public static string RemoverAcentuacao(this string valor)
    {
        if (valor.ItemSemPreenchimento())
            return valor;

        return new string(valor
            .Normalize(NormalizationForm.FormD)
            .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            .ToArray());
    }

    public static bool ItemSemPreenchimento(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool SaoDiferentes(this string valor, string valorAComparar)
    {
        return !valor.ToLower().Equals(valorAComparar.ToLower());
    }
}
